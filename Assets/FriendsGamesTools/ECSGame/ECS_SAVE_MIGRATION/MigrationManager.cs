#if ECS_SAVE_MIGRATION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public static class MigrationManager
    {
        static MigrationSettings settings => MigrationSettings.instance;

        public static void IterateMetaDataDiffs(WorldMetaData oldMeta, WorldMetaData newMeta, Action<MetaDataChange> onChange)
        {
            var change = new MetaDataChange();
            // Create short names
            var oldToShort = oldMeta.CreateShortNames();
            var newToShort = newMeta.CreateShortNames();
            change.oldToShort = oldToShort;
            change.newToShort = newToShort;

            // Find components added.
            newMeta.components.ForEach(newComp =>
            {
                var oldComp = oldMeta.components.Find(c => c.fullName == newComp.fullName);
                if (oldComp != null)
                    return;
                change.changeType = ChangeType.AddComponent;
                change.parentTypeFullName = newComp.fullName;
                change.oldTypeFullName = "";
                change.newTypeFullName = newComp.fullName;
                onChange(change);
            });

            // Find components removed.
            oldMeta.components.ForEach(oldComp =>
            {
                var newComp = newMeta.components.Find(c => c.fullName == oldComp.fullName);
                if (newComp != null)
                    return;
                change.changeType = ChangeType.RemoveComponent;
                change.parentTypeFullName = oldComp.fullName;
                change.oldTypeFullName = oldComp.fullName;
                change.newTypeFullName = "";
                onChange(change);
            });


            // Find buffers added.
            newMeta.buffers.ForEach(newBuffer =>
            {
                var oldBuffer = oldMeta.buffers.Find(c => c.fullName == newBuffer.fullName);
                if (oldBuffer != null)
                    return;
                change.changeType = ChangeType.AddBuffer;
                change.parentTypeFullName = newBuffer.fullName;
                change.oldTypeFullName = "";
                change.newTypeFullName = newBuffer.fullName;
                onChange(change);
            });

            // Find buffers removed.
            oldMeta.buffers.ForEach(oldBuffer =>
            {
                var newBuffer = newMeta.buffers.Find(c => c.fullName == oldBuffer.fullName);
                if (newBuffer != null)
                    return;
                change.changeType = ChangeType.RemoveBuffer;
                change.parentTypeFullName = oldBuffer.fullName;
                change.oldTypeFullName = oldBuffer.fullName;
                change.newTypeFullName = "";
                onChange(change);
            });

            // Find fields diffs.
            IterateFieldsDiffsForStructsList(meta => meta.components);
            IterateFieldsDiffsForStructsList(meta => meta.buffers);
            void IterateFieldsDiffsForStructsList(Func<WorldMetaData, List<SerializedStructMetaData>> getItems)
            {
                var oldItems = getItems(oldMeta);
                var newItems = getItems(newMeta);
                change.parentFields.Clear();
                newItems.ForEach(newStruct =>
                {
                    var oldStruct = oldItems.Find(c => c.fullName == newStruct.fullName);
                    IterateFieldsDiffsForStructs(oldStruct, newStruct);
                });
            }
            void IterateFieldsDiffsForStructs(SerializedStructMetaData oldStruct, SerializedStructMetaData newStruct, List<SerializedStructMetaData> newParents = null)
            {
                if (oldStruct == null)
                    return;
                if (newParents == null)
                {
                    newParents = new List<SerializedStructMetaData>();
                    change.parentTypeFullName = newStruct.fullName;
                }
                newParents.Insert(0, newStruct);
                var removedFields = new List<(string typeFullName, string name)>();
                var addedFields = new List<(string typeFullName, string name)>();
                var changedFields = new List<(string oldTypeFullName, string newTypeFullName, string name)>();
                foreach (var (newTypeFullName, newName) in newStruct.fields)
                    addedFields.Add((newTypeFullName, newName));
                foreach (var (oldTypeFullName, oldName) in oldStruct.fields)
                {
                    var newField = addedFields.Find(a => a.name == oldName);
                    if (newField != default)
                    {
                        addedFields.Remove(newField);
                        var wasEnum = oldMeta.IsEnum(oldTypeFullName);
                        var isEnum = newMeta.IsEnum(newField.typeFullName);
                        if (oldTypeFullName != newField.typeFullName || wasEnum != isEnum)
                            changedFields.Add((oldTypeFullName, newField.typeFullName, oldName));
                        else
                        {
                            // Check field metadata changed.
                            if (isEnum)
                            {
                                var oldEnum = oldMeta.enums.Find(s => s.fullName == oldTypeFullName);
                                var newEnum = newMeta.enums.Find(s => s.fullName == newField.typeFullName);
                                if (!newEnum.SameAs(oldEnum))
                                {
                                    change.changeType = ChangeType.EnumChanged;
                                    change.oldTypeFullName = oldEnum.fullName;
                                    change.newTypeFullName = newEnum.fullName;
                                    change.parentFields.Insert(0, oldName);
                                    onChange(change);
                                    change.parentFields.RemoveAt(0);
                                }

                            } else if (newMeta.IsSerializedStruct(newField.typeFullName))
                            {
                                var oldSerializedStruct = oldMeta.serializedStructs.Find(s => s.fullName == oldTypeFullName);
                                var newSerializedStruct = newMeta.serializedStructs.Find(s => s.fullName == newField.typeFullName);
                                change.parentFields.Insert(0, newField.name);
                                IterateFieldsDiffsForStructs(oldSerializedStruct, newSerializedStruct, newParents);
                                change.parentFields.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        removedFields.Add((oldTypeFullName, oldName));
                    }
                }

                
                // Find fields added.
                addedFields.ForEach(t =>
                {
                    change.changeType = ChangeType.AddField;
                    change.oldTypeFullName = "";
                    change.newTypeFullName = t.typeFullName;
                    change.parentFields.Insert(0, t.name);
                    onChange(change);
                    change.parentFields.RemoveAt(0);
                });
                // Find fields removed.
                removedFields.ForEach(t =>
                {
                    change.changeType = ChangeType.RemoveField;
                    change.oldTypeFullName = t.typeFullName;
                    change.newTypeFullName = "";
                    change.parentFields.Insert(0, t.name);
                    onChange(change);
                    change.parentFields.RemoveAt(0);
                });
                // Find fields type changed.
                changedFields.ForEach(t =>
                {
                    change.changeType = ChangeType.ConvertField;
                    change.oldTypeFullName = t.oldTypeFullName;
                    change.newTypeFullName = t.newTypeFullName;
                    change.parentFields.Insert(0, t.name);
                    onChange(change);
                    change.parentFields.RemoveAt(0);
                });
            }
        }
        public static int LogDiffs(WorldMetaData oldMeta, WorldMetaData newMeta, StringBuilder sb)
        {
            int count = 0;
            IterateMetaDataDiffs(oldMeta, newMeta, change =>
            {
                count++;
                var parentFieldsNoCurr = change.parentFields.Clone();
                var fieldName = "";
                if (parentFieldsNoCurr.Count > 0)
                {
                    fieldName = parentFieldsNoCurr[0];
                    parentFieldsNoCurr.RemoveAt(0);
                }
                var parentPath = $"{parentFieldsNoCurr.PrintCollection("_", "")}{(parentFieldsNoCurr.Count > 0 ? "_" : "")}{change.parentTypeFullName}";
                var oldType = !string.IsNullOrEmpty(change.oldTypeFullName) ? change.oldToShort[change.oldTypeFullName] : "";
                var newType = !string.IsNullOrEmpty(change.newTypeFullName) ? change.newToShort[change.newTypeFullName] : "";
                switch (change.changeType)
                {
                    case ChangeType.AddComponent: sb.AppendLine($"component {newType} added"); break;
                    case ChangeType.RemoveComponent: sb.AppendLine($"component {oldType} removed"); break;
                    case ChangeType.AddBuffer: sb.AppendLine($"buffer {newType} added"); break;
                    case ChangeType.RemoveBuffer: sb.AppendLine($"buffer {oldType} removed"); break;
                    case ChangeType.AddField: sb.AppendLine($"{newType} {fieldName} field added in {parentPath}"); break;
                    case ChangeType.RemoveField: sb.AppendLine($"{oldType} {fieldName} field removed in {parentPath}"); break;
                    case ChangeType.ConvertField: sb.AppendLine($"{oldType} -> {newType} {fieldName} field converted in {parentPath}"); break;
                    case ChangeType.EnumChanged:
                        sb.AppendLine($"{fieldName}'s enum {oldType} values changed " +
                            $"from {oldMeta.GetEnumMeta(change.oldTypeFullName).PrintValues()} " +
                            $"to {newMeta.GetEnumMeta(change.newTypeFullName).PrintValues()}" +
                            $" in {parentPath}"); break;
                }
            });
            return count;
        }
    }
    public enum ChangeType { AddComponent, RemoveComponent, AddBuffer, RemoveBuffer, AddField, RemoveField, ConvertField, EnumChanged }
    public class MetaDataChange
    {
        public ChangeType changeType;
        public Dictionary<string, string> newToShort, oldToShort;
        public string oldTypeFullName, newTypeFullName;
        /// <summary>
        /// First is direct owner field, last is root owner - component or buffer field.
        /// </summary>
        public List<string> parentFields = new List<string>();
        public string parentTypeFullName;
    }
}
#endif