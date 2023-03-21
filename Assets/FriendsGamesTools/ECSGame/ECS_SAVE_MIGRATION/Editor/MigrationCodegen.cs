#if ECS_SAVE_MIGRATION
using FriendsGamesTools.CodeGeneration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public static class MigrationCodegen
    {
        public static void RemoveAll()
        {
            var codeGen = new CodeGenerator();
            var folder = RequireSaveMigrationFolder(codeGen);
            var folderPath = folder.folder;
            Directory.Delete(folderPath, true);
        }
        public static void Generate(WorldMetaData oldMeta, WorldMetaData newMeta, bool migrationClassNeeded, bool aotClassNeeded)
        {
            var codeGen = new CodeGenerator();
            var folder = RequireSaveMigrationFolder(codeGen);
            if (migrationClassNeeded)
                RequireMigrationClass(folder, oldMeta, newMeta);
            if (aotClassNeeded)
                AOTForMobilesCodegen.RequireAOT(codeGen);
            codeGen.Generate();
        }
        private static void RequireMigrationClass(FolderRequirement folder, WorldMetaData oldMeta, WorldMetaData newMeta)
        {
            // Generate migration.
            var className = Migration.GetMigrationBaseClassName(oldMeta.version, newMeta.version);
            var file = folder.RequireFile(className).RequireUsing("System.Collections.Generic").RequireDefineWrapper(SaveMigrationModule.define);
            var @namespace = file.RequireNameSpace(namespaceName);
            var @class = @namespace.RequireClass(className).virtualization.RequireAbstract()
                .RequireInheritance("Migration").visibility.RequirePublic();
            MigrationManager.IterateMetaDataDiffs(oldMeta, newMeta, metaChange =>
            {
                var oldToShort = metaChange.oldToShort;
                var newToShort = metaChange.newToShort;
                var oldTypeShort = !string.IsNullOrEmpty(metaChange.oldTypeFullName) ? metaChange.oldToShort[metaChange.oldTypeFullName] : "";
                var newTypeShort = !string.IsNullOrEmpty(metaChange.newTypeFullName) ? metaChange.newToShort[metaChange.newTypeFullName] : "";
                var fieldName = metaChange.parentFields.FirstOrDefault();
                switch (metaChange.changeType)
                {
                    case ChangeType.AddComponent:
                        var newCompName = newToShort[metaChange.newTypeFullName];
                        MigrationMethod("void", @class, Migration.AddComponentMethodName(newCompName), false, (SerializedStructMetaData, $"newComponentMeta"));
                        break;
                    case ChangeType.RemoveComponent:
                        var oldCompName = oldToShort[metaChange.oldTypeFullName];
                        MigrationMethod("void", @class, Migration.RemoveComponentMethodName(oldCompName), true, (DictionaryStrObj, $"oldComponentValue"));
                        break;
                    case ChangeType.AddBuffer:
                        var newBuffName = newToShort[metaChange.newTypeFullName];
                        MigrationMethod("void", @class, Migration.AddBufferMethodName(newBuffName), false, (SerializedStructMetaData, $"newBufferMeta"));
                        break;
                    case ChangeType.RemoveBuffer:
                        var oldBuffName = oldToShort[metaChange.oldTypeFullName];
                        MigrationMethod("void", @class, Migration.RemoveBufferMethodName(oldBuffName), true, ($"List<{DictionaryStrObj}>", $"oldBufferValue"));
                        break;
                    case ChangeType.AddField:
                        FieldMigrationMethod(Migration.AddFieldMethodName(newTypeShort, fieldName, metaChange), metaChange, @class, oldMeta, newMeta);
                        break;
                    case ChangeType.RemoveField:
                        FieldMigrationMethod(Migration.RemoveFieldMethodName(oldTypeShort, fieldName, metaChange), metaChange, @class, oldMeta, newMeta);
                        break;
                    case ChangeType.ConvertField:
                        FieldMigrationMethod(Migration.ConvertFieldMethodName(oldTypeShort, newTypeShort, fieldName, metaChange), metaChange, @class, oldMeta, newMeta);
                        break;
                    case ChangeType.EnumChanged:
                        var oldEnum = oldMeta.GetEnumMeta(metaChange.oldTypeFullName);
                        var newEnum = newMeta.GetEnumMeta(metaChange.newTypeFullName);
                        oldEnum.values.ForEach(oldValue =>
                        {
                            if (newEnum.values.Contains(oldValue))
                                return;
                            var removedValue = oldValue;
                            FieldMigrationMethod("string", "",
                                Migration.EnumValueRemovedMethodName(oldTypeShort, fieldName, removedValue, metaChange),
                                metaChange, @class, oldMeta, newMeta);
                        });
                        break;
                }
            });
        }
        const string namespaceName = AOTForMobilesCodegen.namespaceName;
        private static FolderRequirement RequireSaveMigrationFolder(CodeGenerator codeGen)
            => codeGen.RequireFolder("SaveMigration");
        static void MigrationMethod(string returnType, ClassRequirement @class, string name, bool entityParams, params (string paramType, string paramName)[] parameters)
        {
            var list = parameters.ToList();
            if (entityParams)
                AddEntityParams(list);
            AddWorldParams(list);
            @class.RequireMethod(returnType, name, list.ToArray()).virtualization.RequireAbstract().visibility.RequireProtected();
        }
        const string UnversionedWorldData = AOTForMobilesCodegen.UnversionedWorldData;
        const string UnversionedEntity = AOTForMobilesCodegen.UnversionedEntity;
        const string DictionaryStrObj = "Dictionary<string, object>";
        const string SerializedStructMetaData = "SerializedStructMetaData";
        static void FieldMigrationMethod(string returnTypeFullName, string oldValueTypeFullName, string methodName, MetaDataChange metaChange, ClassRequirement @class, WorldMetaData oldMeta, WorldMetaData newMeta)
        {
            var existsInOld = !string.IsNullOrEmpty(metaChange.oldTypeFullName);
            var existsInNew = !string.IsNullOrEmpty(metaChange.newTypeFullName);
            var parameters = new List<(string paramType, string paramName)>();
            if (!string.IsNullOrEmpty(oldValueTypeFullName))
                parameters.Add((oldValueTypeFullName, $"oldValue"));
            string currParentTypeOld = metaChange.parentTypeFullName;
            string currParentTypeNew = metaChange.parentTypeFullName;
            var oldTypesShort = new List<string>();
            var newTypesShort = new List<string>();
            for (int i = metaChange.parentFields.Count - 1; i >= 0; i--)
            {
                var currField = metaChange.parentFields[i];
                if (existsInOld)
                {
                    oldTypesShort.Add(metaChange.oldToShort[currParentTypeOld]);
                    currParentTypeOld = oldMeta.GetAnyStructMeta(currParentTypeOld).GetFieldTypeFullName(currField);
                }
                if (existsInNew)
                {
                    newTypesShort.Add(metaChange.newToShort[currParentTypeNew]);
                    currParentTypeNew = newMeta.GetAnyStructMeta(currParentTypeNew).GetFieldTypeFullName(currField);
                }
            }
            if (!existsInOld && existsInNew)
                oldTypesShort = newTypesShort.Clone();
            if (existsInOld && !existsInNew)
                newTypesShort = oldTypesShort.Clone();
            oldTypesShort.Reverse();
            newTypesShort.Reverse();

            for (int i = 0; i < metaChange.parentFields.Count; i++)
            {
                var currField = metaChange.parentFields[i];
                parameters.Add((DictionaryStrObj, $"old_{oldTypesShort[i]}_{currField}_owner"));
                parameters.Add((DictionaryStrObj, $"new_{newTypesShort[i]}_{currField}_owner"));
            }
            MigrationMethod(WorldMetaData.ToWriteableFullName(returnTypeFullName), @class, methodName, true, parameters.ToArray());
        }
        static void FieldMigrationMethod(string methodName, MetaDataChange metaChange, ClassRequirement @class, WorldMetaData oldMeta, WorldMetaData newMeta)
            => FieldMigrationMethod(newMeta.IsEnum(metaChange.newTypeFullName)?"string":( !string.IsNullOrEmpty(metaChange.newTypeFullName) ? metaChange.newTypeFullName : "void"),
                oldMeta.IsEnum(metaChange.oldTypeFullName) ? "string" : metaChange.oldTypeFullName,
                methodName, metaChange, @class, oldMeta, newMeta);
        static void AddEntityParams(List<(string paramType, string paramName)> parameters)
        {
            parameters.Add((UnversionedEntity, "oldEntity"));
            parameters.Add((UnversionedEntity, "newEntity"));
        }
        static void AddWorldParams(List<(string paramType, string paramName)> parameters)
        {
            parameters.Add((UnversionedWorldData, "oldWorld"));
            parameters.Add((UnversionedWorldData, "newWorld"));
        }
        
    }
}
#endif