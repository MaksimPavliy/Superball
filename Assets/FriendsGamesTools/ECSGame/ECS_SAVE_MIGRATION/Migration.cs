#if ECS_SAVE_MIGRATION

using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public abstract class Migration
    {
        public static bool EnsureUpToDate(ref UnversionedWorldData versionIndependantData, Action<int, int> onMigratedFromTo)
        {
            ShowMigrationForDebug(onMigratedFromTo);
            var versionInSave = versionIndependantData.meta.version;
            if (versionInSave == DataVersion.versionInd)
                return true;
            if (versionInSave > DataVersion.versionInd)
            {
                Debug.LogError($"saved data (v{versionInSave}) is newer than current game version (v{DataVersion.versionInd})");
                return false;
            }

            bool success;
            (success, versionIndependantData) = Migrate(versionIndependantData);
            if (success)
                onMigratedFromTo?.Invoke(versionInSave, DataVersion.versionInd);
            return success;
        }

        private static void ShowMigrationForDebug(Action<int, int> onMigratedFromTo)
        {
            if (BuildModeSettings.release || settings.debugShowMigrationFrom == -1)
                return;
            onMigratedFromTo?.Invoke(settings.debugShowMigrationFrom, DataVersion.versionInd);
        }

        public static string GetMigrationBaseClassName(int versionFrom, int versionTo)
            => $"MigrationV{versionFrom}V{versionTo}Base";
        static MigrationSettings settings => MigrationSettings.instance;
        static bool inited;
        static List<Type> migrationClasses = new List<Type>();
        static void InitIfNeeded()
        {
            if (inited) return;
            inited = true;
            migrationClasses = ReflectionUtils.GetAllDerivedTypes(typeof(Migration));
        }

        public static Type GetMigrationBaseClassType(string baseClassName)
        {
            InitIfNeeded();
            var baseClass = migrationClasses.Find(t => t.Name == baseClassName);
            return baseClass;
        }
        public static Type GetMigrationClass(Type baseClass, StringBuilder sb)
        {
            InitIfNeeded();
            var currVersionMigrationTypes = migrationClasses.Filter(t => t.IsSubclassOf(baseClass));
            if (currVersionMigrationTypes.Count == 0)
            {
                sb.AppendLine($"Derive your migration class from {baseClass.Name} to implement migration");
                return null;
            }
            else if (currVersionMigrationTypes.Count > 1)
            {
                sb.AppendLine($"Only one class should be derived from {baseClass.Name}, but found {currVersionMigrationTypes.Count} {currVersionMigrationTypes.ConvertAll(t => t.Name).PrintCollection(",")}");
                return null;
            }
            return currVersionMigrationTypes[0];
        }
        private static (bool success, UnversionedWorldData migratedData) 
            Migrate(UnversionedWorldData dataToMigrate)
        {
            if (dataToMigrate?.meta == null)
                return (false, null);
            var versionFrom = dataToMigrate.meta.version;
            var versionTo = versionFrom + 1;
            if (Application.isPlaying)
                Debug.Log($"starting migration from v{versionFrom} to v{versionTo}");
            var newMeta = settings.GetMetaData(versionTo, true);
            if (newMeta == null)
            {
                Debug.LogError($"cant find meta for version {versionTo}");
                return (false, null);
            }
            var oldMeta = settings.GetMetaData(versionFrom, true);
            if (oldMeta != dataToMigrate.meta)
            {
                Debug.LogError($"save is done not on verson {versionFrom}");
                Debug.Log($"from settings = {oldMeta.version}, {oldMeta.EncodeToString().GetHashCode()}, meta=\n{oldMeta}");
                Debug.Log($"from save = {dataToMigrate.meta.version}, {dataToMigrate.meta.EncodeToString().GetHashCode()}, meta=\n{dataToMigrate.meta}");
                return (false, null);
            }
            var baseClassName = GetMigrationBaseClassName(versionFrom, versionTo);
            var baseClass = GetMigrationBaseClassType(baseClassName);
            if (baseClass == null)
            {
                Debug.LogError($"cant find class with name {baseClassName}");
                return (false, null);
            }
            var sb = new StringBuilder();
            var currVersionMigrationType = GetMigrationClass(baseClass, sb);
            if (sb.Length > 0)
            {
                Debug.LogError(sb.ToString());
                return (false, null);
            }
            var currVersionMigration = ReflectionUtils.CreateInstance<Migration>(currVersionMigrationType);
            var (success, migratedData, errors) = currVersionMigration.Migrate(dataToMigrate, newMeta);
            if (!success)
            {
                Debug.LogError(errors);
                return (false, null);
            }
            if (DataVersion.versionInd == versionTo)
                return (true, migratedData);
            else
                return Migrate(migratedData);
        }
        public (bool success, UnversionedWorldData newWorld, string errors) Migrate(UnversionedWorldData oldWorld, WorldMetaData newMeta)
        {
            var newWorld = new UnversionedWorldData();
            newWorld.meta = newMeta;
            var (success, errors) = Migrate(oldWorld, newWorld);
            if (success)
                return (true, newWorld, string.Empty);
            else
                return (false, null, errors);
        }
        object CloneUnversioned(object unversioned)
        {
            if (unversioned is Dictionary<string, object> dict)
            {
                var clone = new Dictionary<string, object>();
                dict.ForEach(i => clone.Add(i.Key, CloneUnversioned(i.Value)));
                return clone;
            }
            else
                return unversioned;
        }
        Dictionary<UnversionedEntity, UnversionedEntity> getOldEntity = new Dictionary<UnversionedEntity, UnversionedEntity>();
        Dictionary<UnversionedEntity, UnversionedEntity> getNewEntity = new Dictionary<UnversionedEntity, UnversionedEntity>();
        protected UnversionedWorldData oldWorld { get; private set; }
        protected UnversionedWorldData newWorld { get; private set; }
        protected UnversionedEntity GetOldEntity(UnversionedEntity newEntity)
        {
            getOldEntity.TryGetValue(newEntity, out var oldEntity);
            return oldEntity;
        }
        protected UnversionedEntity GetNewEntity(UnversionedEntity oldEntity)
        {
            getNewEntity.TryGetValue(oldEntity, out var newEntity);
            return newEntity;
        }

        public static string AddComponentMethodName(string newCompShortName) => $"OnComponent_{newCompShortName}_Added";
        public static string RemoveComponentMethodName(string oldCompShortName) => $"OnComponent_{oldCompShortName}_Removed";
        public static string AddBufferMethodName(string newBufferItemShortName) => $"OnBuffer_{newBufferItemShortName}_Added";
        public static string RemoveBufferMethodName(string oldBufferItemShortName) => $"OnBuffer_{oldBufferItemShortName}_Removed";
        public static string AddFieldMethodName(string newTypeShort, string fieldName, MetaDataChange metaChange)
            => $"OnField_{newTypeShort}_{fieldName}_Added_In_{WhereFieldChanged(metaChange)}";
        public static string RemoveFieldMethodName(string oldTypeShort, string fieldName, MetaDataChange metaChange)
            => $"OnField_{oldTypeShort}_{fieldName}_Removed_In_{WhereFieldChanged(metaChange)}";
        public static string ConvertFieldMethodName(string oldTypeShort, string newTypeShort, string fieldName, MetaDataChange metaChange)
            => $"OnField_{fieldName}_ConvertFrom_{oldTypeShort}_To_{newTypeShort}_In_{WhereFieldChanged(metaChange)}";
        public static string EnumValueRemovedMethodName(string typeShort, string fieldName, string removedValue, MetaDataChange metaChange)
            => $"On_Field_{typeShort}_{fieldName}_Value_{removedValue}_Removed_In_{WhereFieldChanged(metaChange)}";

        public static string WhereFieldChanged(MetaDataChange metaChange)
        {
            var parentType = metaChange.newToShort[metaChange.parentTypeFullName];
            if (metaChange.parentFields.Count == 0)
                return parentType;
            var fieldsToPrint = metaChange.parentFields;
            fieldsToPrint = fieldsToPrint.Clone();
            fieldsToPrint.RemoveAt(0);
            if (fieldsToPrint.Count == 0)
                return parentType;
            return $"{fieldsToPrint.PrintCollection("_", "")}_{parentType}";
        }
        MethodInfo GetMigrationMethod(string name)
            => GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        public (bool success, string errors) Migrate(UnversionedWorldData oldWorld, UnversionedWorldData newWorld)
        {
            // Copy all.
            this.oldWorld = oldWorld;
            this.newWorld = newWorld;
            getOldEntity.Clear();
            getNewEntity.Clear();
            oldWorld.entities.ForEach(oldEntity => {
                var newEntity = new UnversionedEntity();
                oldEntity.components.ForEach(oldComp => newEntity.components.Add(oldComp.Key, CloneUnversioned(oldComp.Value)));
                oldEntity.buffers.ForEach(oldBuff => newEntity.buffers.Add(oldBuff.Key, oldBuff.Value.ConvertAll(oldItem => CloneUnversioned(oldItem))));
                newWorld.entities.Add(newEntity);
                getOldEntity.Add(newEntity, oldEntity);
                getNewEntity.Add(oldEntity, newEntity);
            });
            // Iterate meta changes.
            Dictionary<string, object> oldOwner, newOwner;
            List<Dictionary<string, object>> oldParents, newParents;
            var type = GetType();
            MethodInfo methodInfo;
            string name;
                object[] parameters;
            MigrationManager.IterateMetaDataDiffs(oldWorld.meta, newWorld.meta, metaChange =>
            {
                string fieldName = metaChange.parentFields.FirstOrDefault();
                var newToShort = metaChange.newToShort;
                var oldToShort = metaChange.oldToShort;
                switch (metaChange.changeType)
                {
                    case ChangeType.AddComponent:
                        var newCompMeta = newWorld.meta.GetComponentMeta(metaChange.newTypeFullName);
                        name = AddComponentMethodName(newToShort[metaChange.newTypeFullName]);
                        methodInfo = GetMigrationMethod(name);
                        methodInfo.Invoke(this, new object[] { newCompMeta, oldWorld, newWorld });
                        break;
                    case ChangeType.RemoveComponent:
                        name = RemoveComponentMethodName(oldToShort[metaChange.oldTypeFullName]);
                        methodInfo = GetMigrationMethod(name);
                        newWorld.entities.ForEach(newEntity => {
                            if (!newEntity.components.TryGetValue(metaChange.oldTypeFullName, out var oldComponent))
                                return;
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var oldValue = newEntity.components[metaChange.oldTypeFullName];
                            newEntity.components.Remove(metaChange.oldTypeFullName);
                            methodInfo.Invoke(this, new object[] { oldValue, oldEntity, newEntity, oldWorld, newWorld });
                        });
                        break;
                    case ChangeType.AddBuffer:
                        var newBufferMeta = newWorld.meta.GetBufferMeta(metaChange.newTypeFullName);
                        name = AddBufferMethodName(newToShort[metaChange.newTypeFullName]);
                        methodInfo = GetMigrationMethod(name);
                        methodInfo.Invoke(this, new object[] { newBufferMeta, oldWorld, newWorld });
                        break;
                    case ChangeType.RemoveBuffer:
                        name = RemoveBufferMethodName(oldToShort[metaChange.oldTypeFullName]);
                        methodInfo = GetMigrationMethod(name);
                        newWorld.entities.ForEach(newEntity => {
                            if (!newEntity.buffers.TryGetValue(metaChange.oldTypeFullName, out var oldBuffer))
                                return;
                            newEntity.buffers.Remove(metaChange.oldTypeFullName);
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var oldBufferDict = oldBuffer.ConvertAll(i => (Dictionary<string, object>)i);
                            methodInfo.Invoke(this, new object[] { oldBufferDict, oldEntity, newEntity, oldWorld, newWorld });
                        });
                        break;
                    case ChangeType.AddField:
                        name = AddFieldMethodName(newToShort[metaChange.newTypeFullName], fieldName, metaChange);
                        methodInfo = GetMigrationMethod(name);
                        IterateEntititesWithParent(metaChange.parentTypeFullName, newWorld, i =>
                        {
                            var newEntity = i.newEntity;
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var bufferInd = i.newBufferInd;
                            (oldParents, newParents)
                               = GetFieldEditing(oldEntity, newEntity, metaChange.parentTypeFullName, bufferInd, metaChange.parentFields);
                            var newParents2 = newParents;
                            var oldParents2 = oldParents;
                            var fieldName2 = fieldName;
                            newOwner = newParents.Last();
                            parameters = GetFieldEditingParams(null, oldEntity, newEntity);
                            var newValue = methodInfo.Invoke(this, parameters);
                            newOwner[fieldName] = newValue;
                        });
                        break;
                    case ChangeType.RemoveField:
                        name = RemoveFieldMethodName(oldToShort[metaChange.oldTypeFullName], fieldName, metaChange);
                        methodInfo = GetMigrationMethod(name);
                        IterateEntititesWithParent(metaChange.parentTypeFullName, newWorld, i =>
                        {
                            var newEntity = i.newEntity;
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var bufferInd = i.newBufferInd;
                            (oldParents, newParents)
                               = GetFieldEditing(oldEntity, newEntity, metaChange.parentTypeFullName, bufferInd, metaChange.parentFields);
                            newOwner = newParents.Last();
                            oldOwner = oldParents.Last();
                            newOwner.Remove(fieldName);
                            var name2 = name;
                            var newParents2 = newParents;
                            var oldParents2 = oldParents;
                            var oldValue = oldOwner[fieldName];
                            parameters = GetFieldEditingParams(oldValue, oldEntity, newEntity);
                            methodInfo.Invoke(this, parameters);
                        });
                        break;
                    case ChangeType.ConvertField:
                        name = ConvertFieldMethodName(oldToShort[metaChange.oldTypeFullName], newToShort[metaChange.newTypeFullName],
                            fieldName, metaChange);
                        methodInfo = GetMigrationMethod(name);
                        IterateEntititesWithParent(metaChange.parentTypeFullName, newWorld, i =>
                        {
                            var newEntity = i.newEntity;
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var bufferInd = i.newBufferInd;
                            (oldParents, newParents)
                                = GetFieldEditing(oldEntity, newEntity, metaChange.parentTypeFullName, bufferInd, metaChange.parentFields);
                            newOwner = newParents.Last();
                            oldOwner = oldParents.Last();                           
                            var oldValue = oldOwner[fieldName];
                            parameters = GetFieldEditingParams(oldValue, oldEntity, newEntity);
                            var newValue = methodInfo.Invoke(this, parameters);
                            newOwner[fieldName] = newValue;
                        });
                        break;
                    case ChangeType.EnumChanged:
                        IterateEntititesWithParent(metaChange.parentTypeFullName, newWorld, i =>
                        {
                            var newEntity = i.newEntity;
                            var oldEntity = GetOldEntity(newEntity);
                            if (oldEntity == null)
                                return;
                            var bufferInd = i.newBufferInd;
                            (oldParents, newParents)
                                = GetFieldEditing(oldEntity, newEntity, metaChange.parentTypeFullName, bufferInd, metaChange.parentFields);
                            newOwner = newParents.Last();
                            oldOwner = oldParents.Last();
                            var oldValue = oldOwner[fieldName];
                            if (oldValue.GetType() == typeof(string))
                            {
                                var oldValueString = (string)oldValue;
                                var newEnumMeta = newWorld.meta.GetEnumMeta(metaChange.newTypeFullName);
                                if (!newEnumMeta.values.Contains(oldValueString))
                                {
                                    name = EnumValueRemovedMethodName(oldToShort[metaChange.oldTypeFullName], fieldName, oldValueString, metaChange);
                                    methodInfo = GetMigrationMethod(name);
                                    parameters = GetFieldEditingParams(null, oldEntity, newEntity);
                                    var newValue = methodInfo.Invoke(this, parameters);
                                    newOwner[fieldName] = newValue;
                                }
                            }
                        });
                        break;
                }
                OnMetaDataChange(metaChange, oldWorld, newWorld);
            });
            OnAfterMigration(oldWorld, newWorld);
            // Verify with new meta
            return newWorld.Verify();

            object[] GetFieldEditingParams(object oldValue, UnversionedEntity oldEntity, UnversionedEntity newEntity)
            {
                var hasOldValue = oldValue != null;
                var currParams = new object[newParents.Count * 2 + 4 + (hasOldValue ? 1 : 0)];
                int j = 0;
                if (hasOldValue)
                {
                    currParams[j] = oldValue;
                    j++;
                }
                for (int i = newParents.Count - 1; i >= 0; i--)
                {
                    currParams[j] = oldParents[i]; j++;
                    currParams[j] = newParents[i]; j++;
                }
                currParams[j] = oldEntity; j++;
                currParams[j] = newEntity; j++;
                currParams[j] = oldWorld; j++;
                currParams[j] = newWorld; j++;
                return currParams;
            }
        }
        

        void IterateEntititesWithParent(string parentTypeFullName, UnversionedWorldData newWorld,
            Action<(UnversionedEntity newEntity, int newBufferInd)> onFound)
        {
            newWorld.entities.ForEach(newEntity =>
            {
                if (newEntity.components.ContainsKey(parentTypeFullName))
                    onFound((newEntity, -1));
                else if (newEntity.buffers.ContainsKey(parentTypeFullName))
                {
                    var count = newEntity.buffers[parentTypeFullName].Count;
                    for (int i = 0; i < count; i++)
                        onFound((newEntity, i));
                }
            });
        }
        (List<Dictionary<string, object>> oldParents, List<Dictionary<string, object>> newParents)
            GetFieldEditing(UnversionedEntity oldEntity, UnversionedEntity newEntity,
            string parentTypeFullName, int bufferInd, List<string> parentFields)
        {
            var oldParents = GetFieldParents(oldEntity, parentTypeFullName, bufferInd, parentFields);
            var newParents = GetFieldParents(newEntity, parentTypeFullName, bufferInd, parentFields);
            return (oldParents, newParents);
        }
        List<Dictionary<string, object>> 
            GetFieldParents(UnversionedEntity entity, string parentTypeFullName, int bufferInd, List<string> parentFields)
        {
            if (entity == null)
                return default;
            if (entity.components.TryGetValue(parentTypeFullName, out var comp))
                return GetFieldParents((Dictionary<string, object>)comp, parentFields);
            if (entity.buffers.TryGetValue(parentTypeFullName, out var buff))
            {
                if (!buff.IndIsValid(bufferInd))
                    return default;
                //Debug.LogError($"error in migration, cant do GetFieldEditing");
                return GetFieldParents((Dictionary<string, object>)buff[bufferInd], parentFields);
            }
            //Debug.LogError($"error in migration, cant do GetFieldEditing");
            return default;
        }
        List<Dictionary<string, object>>
            GetFieldParents(Dictionary<string, object> structDict, List<string> parentFields)
        {
            var parents = new List<Dictionary<string, object>>();
            object curr = structDict;
            for (int i = parentFields.Count - 1; i >= 0; i--)
            {
                var fieldName = parentFields[i];
                var currDict = (Dictionary<string, object>)curr;
                parents.Add(currDict);
                if (!currDict.TryGetValue(fieldName, out var fieldValue))
                    break;
                curr = fieldValue;
            }
            if (parents.Count != parentFields.Count)
                return default;
            return parents;
        }

        protected virtual void OnMetaDataChange(MetaDataChange metaChange, UnversionedWorldData oldWorld, UnversionedWorldData newWorld) { }
        protected virtual void OnAfterMigration(UnversionedWorldData oldWorld, UnversionedWorldData newWorld) { }

        #region Modifying in migration
        protected bool ChangeComponentField<T>(string compFullName, string fieldName, Func<T, T> change)
        {
            var success = false;
            object value;
            oldWorld.entities.ForEach(oldEntity =>
            {
                if (!oldEntity.components.TryGetValue(compFullName, out value))
                    return;
                var oldComp = (Dictionary<string, object>)value;
                var oldFieldValue = (T)oldComp[fieldName];
                var newFieldValue = change(oldFieldValue);
                var newEntity = GetNewEntity(oldEntity);
                var newComp = (Dictionary<string, object>)newEntity.components[compFullName];
                newComp[fieldName] = newFieldValue;
                success = true;
            });
            return success;
        }
        protected T GetFieldRenamedFrom<T>(string oldFieldName, Dictionary<string, object> oldOwner) => (T)oldOwner[oldFieldName];
        protected void RenameComponent(string oldFullName, string newFullName)
        {
            newWorld.entities.ForEach(newEntity =>
            {
                if (newEntity.components.ContainsKey(oldFullName))
                {
                    newEntity.components.Add(newFullName, newEntity.components[oldFullName]);
                    newEntity.components.Remove(oldFullName);
                }
            });
        }
        #endregion
    }
}
#endif