#if ECSGame

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class UnversionedWorldData : IBinarySerializable
    {
        public WorldMetaData meta = new WorldMetaData();
        public List<UnversionedEntity> entities = new List<UnversionedEntity>();

#if ECS_SAVE_MIGRATION
        public (bool success, string errors) Verify()
        {
            UnversioningConverter.UpdateCurrentTypes();
            var sb = new StringBuilder();
            entities.ForEachWithInd((entity, entityInd) =>
            {
                entity.components.ForEach(component => VerifyUnversioned(component.Value, component.Key, new List<string>(), component.Key, -1, entityInd));
                entity.buffers.ForEach(buffer => buffer.Value.ForEach(item
                    => VerifyUnversioned(item, buffer.Key, new List<string>(), buffer.Key, buffer.Value.IndexOf(item), entityInd)));
            });
            return (sb.Length == 0, sb.ToString());

            void VerifyUnversioned(object unversionedValue, string typeFullName,
                List<string> parentFields, string rootTypeFullName, int bufferInd, int entityInd)
            {
                var actualType = unversionedValue.GetType();
                var type = UnversioningConverter.GetType(typeFullName);
                if (type != null && type.IsPrimitiveOrDecimal())
                {
                    // Check primitive type.
                    if (!WorldMetaDataUtils.PrimitiveTypeIsAllowed(typeFullName))
                        sb.AppendLine($"primitive type not allowed {typeFullName} in {GetPath()}");
                    else if (actualType != type)
                        sb.AppendLine($"type mismatch: have to be {typeFullName} but actually {actualType.FullName} in {GetPath()}");
                    return;
                }
                var enumMeta = meta.GetEnumMeta(typeFullName);
                if (enumMeta != null)
                {
                    // Check enum.
                    if (actualType != typeof(string))
                        sb.AppendLine($"enum {typeFullName} value have to be unversioned as string but its {actualType.FullName} instead, value = {unversionedValue} in {GetPath()}");
                    else
                    {
                        var stringValue = (string)unversionedValue;
                        if (!enumMeta.values.Contains(stringValue))
                            sb.AppendLine($"enum value {stringValue} is available in {typeFullName}, allowed types are [{enumMeta.values.PrintCollection(",")}]");
                    }
                    return;
                }
                var structMeta = meta.GetAnyStructMeta(typeFullName);
                if (structMeta!=null)
                {
                    // Check structs: components, buffer items, serialized structs.
                    if (actualType != typeof(Dictionary<string, object>))
                        sb.AppendLine($"struct {typeFullName} value have to be unversioned as Dictionary<string, object> but its {actualType.FullName} instead, value = {unversionedValue} in {GetPath()}");
                    else
                    {
                        var dict = (Dictionary<string, object>)unversionedValue;
                        // Check missing field.
                        structMeta.fields.ForEach(f=> {
                            if (!dict.ContainsKey(f.name))
                                sb.AppendLine($"missing field {f.name} in struct {typeFullName} in {GetPath()}");
                        });
                        dict.ForEach(f =>
                        {
                            // Check excessive field.
                            var fieldName = f.Key;
                            var unversionedFieldValue = f.Value;
                            var fieldMeta = structMeta.fields.Find(f1 => f1.name == fieldName);
                            if (fieldMeta == default)
                            {
                                sb.AppendLine($"excessive field {fieldName} in struct {typeFullName} in {GetPath()}");
                                return;
                            }
                            // Check field value recursive.
                            var fieldTypeFullName = fieldMeta.typeFullName;
                            parentFields.Insert(0, fieldName);
                            VerifyUnversioned(unversionedFieldValue, fieldTypeFullName, parentFields, rootTypeFullName, bufferInd, entityInd);
                            parentFields.RemoveAt(0);
                        });
                    }
                    return;
                }
                sb.AppendLine($"unknown {typeFullName} - it is not recognized by meta(v{meta.version}), value is {unversionedValue} in {GetPath()}");

                string GetPath()
                {
                    return $"entity[{entityInd}].({rootTypeFullName}){(bufferInd == -1 ? $"" : $"[{bufferInd}]")}" +
                    $"{(parentFields.Count > 0 ? "." : "")}{parentFields.Reversed().PrintCollection(".", "")}";
                }
            }
        }

        #region Modifying in migration
        public UnversionedEntity GetSingleEntityWithComponent(string componentFullName)
            => entities.Find(e => e.components.ContainsKey(componentFullName));
        public UnversionedEntity AddEntity(params string[] componentNames)
        {
            var e = new UnversionedEntity();
            componentNames.ForEach(fullTypeName=> AddComponent(fullTypeName,e));
            entities.Add(e);
            return e;
        }
        public void OnComponentRenamed(string newTypeFullname,
            Dictionary<string, object> oldComponentValue, UnversionedEntity newEntity)
            => newEntity.components.Add(newTypeFullname, oldComponentValue);
        public Dictionary<string, object> AddComponent(string typeFullName, UnversionedEntity entity)
            => AddComponent(meta.GetComponentMeta(typeFullName), entity);
        public Dictionary<string, object> AddComponent(SerializedStructMetaData meta, UnversionedEntity entity)
        {
            var dict = CreateUnversionedDict(meta);
            entity.components.Add(meta.fullName, dict);
            return dict;
        }
        public Dictionary<string, object> CreateUnversionedDict(SerializedStructMetaData meta)
        {
            var dict = new Dictionary<string, object>();
            meta.fields.ForEach(f => dict.Add(f.name, CreateUnversionedData(f.typeFullName)));
            return dict;
        }
        public object CreateUnversionedData(string typeFullName)
        {
            var primitiveType = WorldMetaDataUtils.GetPrimitiveType(typeFullName);
            if (primitiveType != null) return ReflectionUtils.CreateInstance(primitiveType);
            var enumType = meta.GetEnumMeta(typeFullName);
            if (enumType != null) return enumType.values[0];
            var structMeta = meta.GetAnyStructMeta(typeFullName);
            if (structMeta != null) return CreateUnversionedDict(structMeta);
            Debug.LogError($"CreateUnversionedData failed to create {typeFullName}, its not in current metadata");
            return null;
        }
        #endregion
#endif

        #region Save/load binary
        public bool SaveVersionIndependant(string path)
        {
            FileStream stream = null;
            try
            {
                stream = File.Create(path);
            }
            catch 
            {
                return false;
            }
            var bw = new BinaryWriter(stream);
            meta.WriteVersion(bw);
            Write(bw);
            bw.Close();
            return true;
        }
        public static UnversionedWorldData LoadVersionIndependant(string path)
        {
            var (br, version) = StartRead(path);
            if (br == null)
                return null;
            var data = new UnversionedWorldData { meta = new WorldMetaData { version = version } };
            data.Read(br);
            br.Close();
            return data;
        }
        public static int GetVersionFromSave(string path)
        {
            var (br, version) = StartRead(path);
            if (br == null)
                return -1;
            br.Close();
            return version;
        }
        static (BinaryReader br, int version) StartRead(string path)
        {
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(path);
            }
            catch 
            {
                return (null, -1);
            }
            var br = new BinaryReader(stream);

            var version = WorldMetaData.ReadVersion(br);
            if (version == -1)
            {
                br.Close();
                return (null, -1);
            }
            return (br, version);
        }
        public void Write(BinaryWriter bw)
        {
            meta.Write(bw);
            SerializationUtils.WriteList(entities, bw, e => e.Write(bw, meta));
        }
        public void Read(BinaryReader br)
        {
            meta.Read(br);
            SerializationUtils.ReadList(entities, br, ()=> {
                var e = new UnversionedEntity();
                e.Read(br, meta);
                return e;
            });
        }
        #endregion
    }
}
#endif