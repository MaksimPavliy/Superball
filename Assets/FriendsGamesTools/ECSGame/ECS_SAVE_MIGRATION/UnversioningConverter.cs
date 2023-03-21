#if ECSGame

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public static class UnversioningConverter
    {
        public static UnversionedWorldData VersionedToUnversioned()
        {
            var unversionedWorld = new UnversionedWorldData() { meta = WorldMetaData.CreateCurrent() };
            var versionedEntities = World.Active.EntityManager.GetAllEntities();

            var genericGetComponentDataMethodInfo = typeof(EntityManager).GetMethod("GetComponentData");
            var genericGetBufferMethodInfo = typeof(EntityManager).GetMethod("GetBuffer");
            var parameters = new object[1];

            foreach (var versionedEntity in versionedEntities)
            {
                var unversionedEntity = new UnversionedEntity();
                unversionedWorld.entities.Add(unversionedEntity);
                parameters[0] = versionedEntity;

                WorldMetaDataUtils.componentTypes.ForEach(componentType =>
                {
                    if (!World.Active.EntityManager.HasComponent(versionedEntity, componentType))
                        return;
                    var getComponentDataMethodInfo = genericGetComponentDataMethodInfo.MakeGenericMethod(componentType);
                    object componentDataBoxed = getComponentDataMethodInfo.Invoke(World.Active.EntityManager, parameters);
                    var unversionedComponent = new Dictionary<string, object>();
                    WorldMetaDataUtils.GetFieldsToSerialize(componentType).ForEach(fieldInfo => {
                        var value = fieldInfo.GetValue(componentDataBoxed);
                        unversionedComponent.Add(fieldInfo.Name, FromVersionedToUnversioned(value));
                    });
                    unversionedEntity.components.Add(componentType.FullName, unversionedComponent);
                });

                WorldMetaDataUtils.bufferItemTypes.ForEach(bufferItemType =>
                {
                    if (!World.Active.EntityManager.HasComponent(versionedEntity, bufferItemType))
                        return;

                    var getBufferDataMethodInfo = genericGetBufferMethodInfo.MakeGenericMethod(bufferItemType);
                    object bufferDataBoxed = getBufferDataMethodInfo.Invoke(World.Active.EntityManager, parameters);
                    var bufferData = new List<object>();

                    var bufferType = bufferDataBoxed.GetType();
                    var pi = bufferType.GetIndexerInfo(bufferItemType);
                    if (pi != null)
                    {
                        var count = ReflectionUtils.GetProperty<int>(bufferDataBoxed, "Length");
                        for (int i = 0; i < count; i++)
                        {
                            object bufferItemValueBoxed = pi.GetValue(bufferDataBoxed, new object[] { i });
                            bufferData.Add(FromVersionedToUnversioned(bufferItemValueBoxed));
                        }
                    }
                    else
                        UnityEngine.Debug.LogError("this class didn't have an indexer");
                    unversionedEntity.buffers.Add(bufferItemType.FullName, bufferData);
                });
            }
            versionedEntities.Dispose();
            //UnityEngine.Debug.Log(data.ToString() + "\n\n\n");
            return unversionedWorld;
        }
        static object FromVersionedToUnversioned(object versionedValue)
        {
            // Primitive types just dont change.
            var type = versionedValue.GetType();
            if (WorldMetaDataUtils.allowedPrimitiveTypes.Contains(type))
                return versionedValue;

            // Enum values get converted to string.
            if (type.IsEnum)
                return versionedValue.ToString();

            // Components, Serialized structs convert to Dictionary<string, object>
            if (WorldMetaDataUtils.componentTypes.Contains(type) || WorldMetaDataUtils.bufferItemTypes.Contains(type) || WorldMetaDataUtils.serializedStructs.Contains(type))
            {
                var unversionedValue = new Dictionary<string, object>();
                WorldMetaDataUtils.GetFieldsToSerialize(type).ForEach(fieldInfo => {
                    var versionedFieldValue = fieldInfo.GetValue(versionedValue);
                    unversionedValue.Add(fieldInfo.Name, FromVersionedToUnversioned(versionedFieldValue));
                });
                return unversionedValue;
            }

            Debug.LogError($"Cant unversion {versionedValue}, type = {type.FullName}");
            return null;
        }

        static Dictionary<string, Type> types = new Dictionary<string, Type>();
        public static void UpdateCurrentTypes()
        {
            types.Clear();
            void AddType(Type type) => types.Add(type.FullName, type);
            WorldMetaDataUtils.allStructTypes.ForEach(AddType);
            WorldMetaDataUtils.enumTypes.ForEach(AddType);
            WorldMetaDataUtils.allowedPrimitiveTypes.ForEach(AddType);
        }
        public static Type GetType(string fullName)
            => types.TryGetValue(fullName, out var type) ? type : null;

        static MethodInfo genericAddComponent;
        public static MethodInfo GenericAddComponentDataMethodInfo()
            => genericAddComponent ?? (genericAddComponent = typeof(EntityManager).GetMethods().Find(m
                => m.Name == "AddComponentData" && m.ContainsGenericParameters && m.GetParameters().FirstOrDefault()?.ParameterType == typeof(Entity)));
        static MethodInfo genericSetComponent;
        public static MethodInfo GenericSetComponentDataMethodInfo()
            => genericSetComponent ?? (genericSetComponent = typeof(EntityManager).GetMethods().Find(m
                => m.Name == "SetComponentData" && m.ContainsGenericParameters && m.GetParameters().FirstOrDefault()?.ParameterType == typeof(Entity)));
        static MethodInfo genericGetComponent;
        public static MethodInfo GenericGetComponentDataMethodInfo()
            => genericGetComponent ?? (genericGetComponent = typeof(EntityManager).GetMethods().Find(m
                => m.Name == "GetComponentData" && m.ContainsGenericParameters && m.GetParameters().FirstOrDefault()?.ParameterType == typeof(Entity)));

        public static bool UnversionedToVersioned(UnversionedWorldData unversionedWorld)
        {
            UpdateCurrentTypes();

            var meta = WorldMetaData.CreateCurrent();
            // TODO: Check unversionedWorld.meta==meta;
            var valid = true;
            var genericAddComponentDataMethodInfo = GenericAddComponentDataMethodInfo();
            var genericAddBufferMethodInfo = typeof(EntityManager).GetMethod("AddBuffer");
            var genericGetBufferMethodInfo = typeof(EntityManager).GetMethod("GetBuffer");
            var parameter = new object[1];
            var parameters = new object[2];

            Serialization.ClearWorld();
            List<Entity> versionedEntities = new List<Entity>();
            //var sw = System.Diagnostics.Stopwatch.StartNew();
            unversionedWorld.entities.ForEach(entityUnversioned =>
            {
                var versionedEntity = ECSUtils.CreateEntity();
                versionedEntities.Add(versionedEntity);
                entityUnversioned.components.ForEach(unversionedItem => {
                    var componentFullName = unversionedItem.Key;
                    var unversionedComponent = (Dictionary<string, object>)unversionedItem.Value;
                    var componentType = GetType(componentFullName);
                    if (componentType == null)
                    {
                        Debug.LogError($"componentType {componentFullName} does not exist in current version");
                        valid = false;
                        return;
                    }
                    //Debug.Log($"type detected {componentFullName}");
                    var fieldsToDeserialize = WorldMetaDataUtils.GetFieldsToSerialize(componentType);
                    if (fieldsToDeserialize.Length != unversionedComponent.Count)
                    {
                        // type fileds count changed.
                        Debug.LogError($"componentType {componentFullName} fileds count changed " +
                            $"({fieldsToDeserialize.Length} in curr version, {unversionedComponent.Count} in save)");
                        valid = false;
                        return;
                    }
                    var componentVersioned = ReflectionUtils.CreateInstance(componentType);
                    fieldsToDeserialize.ForEach(fieldInfo => {
                        if (unversionedComponent.TryGetValue(fieldInfo.Name, out var unversionedValue))
                        {
                            var valueVersioned = UnversionedToVersioned(unversionedValue, fieldInfo.FieldType.FullName, meta);
                            if (valueVersioned == null)
                            {
                                Debug.LogError($"value {unversionedValue} of field {fieldInfo.Name} of component {componentFullName} cant be converted to versioned");
                                valid = false;
                                return;
                            }
                            fieldInfo.SetValue(componentVersioned, valueVersioned);
                        }
                        else
                        {
                            // field fieldInfo.Name does not exist.
                            Debug.LogError($"field {fieldInfo.Name} does not exist in component {componentFullName}");
                            valid = false;
                            return;
                        }
                    });
                    var addComponentDataMethodInfo = genericAddComponentDataMethodInfo.MakeGenericMethod(componentType);
                    parameters[0] = versionedEntity;
                    parameters[1] = componentVersioned;
                    addComponentDataMethodInfo.Invoke(World.Active.EntityManager, parameters);
                });
                entityUnversioned.buffers.ForEach(unversionedItem =>
                {
                    var bufferItemFullName = unversionedItem.Key;
                    var unversionedBuffer = unversionedItem.Value;

                    var bufferItemType = GetType(bufferItemFullName);
                    if (bufferItemType == null)
                    {
                        Debug.LogError($"buffer {bufferItemFullName} does not exist in current version");
                        valid = false;
                        return;
                    }
                    //Debug.Log($"type detected {bufferItemFullName}");
                    var addBufferDataMethodInfo = genericAddBufferMethodInfo.MakeGenericMethod(bufferItemType);
                    parameter[0] = versionedEntity;
                    addBufferDataMethodInfo.Invoke(World.Active.EntityManager, parameter);
                    var getBufferDataMethodInfo = genericGetBufferMethodInfo.MakeGenericMethod(bufferItemType);
                    object bufferVersioned = getBufferDataMethodInfo.Invoke(World.Active.EntityManager, parameter);
                    unversionedBuffer.ForEach(unversionedBufferItem =>
                    {
                        var versionedBufferItem = UnversionedToVersioned(unversionedBufferItem, bufferItemFullName, meta);
                        ReflectionUtils.CallMethodExplicitParamTypes(bufferVersioned, "Add", (bufferItemType, versionedBufferItem));
                    });
                });
            });
            //Debug.Log($"entities, {sw.ElapsedMilliseconds}ms"); sw.Restart();
            return valid;
        }
        static object UnversionedToVersioned(object unversionedValue, string typeFullName, WorldMetaData meta)
        {
            // Primitive types just dont change.
            var type = GetType(typeFullName);
            if (WorldMetaDataUtils.allowedPrimitiveTypes.Contains(type))
                return unversionedValue;

            // Enum values get converted from string.
            if (type.IsEnum)
                return Enum.Parse(type, (string)unversionedValue);

            // Components, Serialized structs convert from Dictionary<string, object>
            if (WorldMetaDataUtils.componentTypes.Contains(type) || WorldMetaDataUtils.bufferItemTypes.Contains(type) || WorldMetaDataUtils.serializedStructs.Contains(type))
            {
                var unversionedValueDict = (Dictionary<string, object>)unversionedValue;
                var versionedValue = ReflectionUtils.CreateInstance(type);
                WorldMetaDataUtils.GetFieldsToSerialize(type).ForEach(fieldInfo =>
                {
                    var unversionedFieldValue = unversionedValueDict[fieldInfo.Name];
                    fieldInfo.SetValue(versionedValue, UnversionedToVersioned(unversionedFieldValue, fieldInfo.FieldType.FullName, meta));
                });
                return versionedValue;
            }

            Debug.LogError($"Cant version {unversionedValue}, type = {typeFullName}");
            return null;
        }

        static Dictionary<int, string> indents = new Dictionary<int, string>();
        static string GetIndent(int indent)
        {
            if (indents.TryGetValue(indent, out var str))
                return str;
            str = "";
            for (int i = 0; i < indent; i++)
                str += "  ";
            indents.Add(indent, str);
            return str;
        }
        public static string ToString(Dictionary<string, object> unversioned, int indent = 0)
        {
            var ind = GetIndent(indent);
            if (unversioned.Count == 0)
                return $"{ind}{{ }}";
            return $"{ind}{{\n{unversioned.ConvertAll(u=>$"{ind}{u.Key}:{ToString(u.Value, indent+1)}").PrintCollection(",\n")}\n{ind}}}";
        }
        public static string ToString(object unversioned, int indent = 0)
        {
            if (unversioned is Dictionary<string, object> dict)
                return ToString(dict, indent);
            return unversioned.ToString();
        }
    }
}
#endif