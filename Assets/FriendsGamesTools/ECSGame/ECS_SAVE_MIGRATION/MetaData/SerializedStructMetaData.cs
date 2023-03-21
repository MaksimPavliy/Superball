#if ECSGame

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class SerializedStructMetaData : IBinarySerializable
    {
        public string fullName;
        public List<(string typeFullName, string name)> fields = new List<(string typeFullName, string name)>();
        public string GetFieldTypeFullName(string fieldName, bool logErrors = true)
        {
            var field = fields.Find(f=>f.name == fieldName);
            if (field != default)
                return field.typeFullName;
            else
            {
                if (logErrors)
                    Debug.LogError($"cant find field {fieldName} type, it does not exist in {fullName}");
                return "";
            }
        }
        public static SerializedStructMetaData CreateFromType(Type type)
        {
            Debug.Assert(type.IsStruct());
            var res = new SerializedStructMetaData() { fullName = type.FullName };
            WorldMetaDataUtils.GetFieldsToSerialize(type).ForEach(f => res.fields.Add((f.FieldType.FullName, f.Name)));
            return res;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(fullName);
            SerializationUtils.WriteList(fields, bw, f =>
            {
                bw.Write(f.typeFullName);
                bw.Write(f.name);
            });
        }
        public void Read(BinaryReader br)
        {
            fullName = br.ReadString();
            SerializationUtils.ReadList(fields, br, () => (br.ReadString(), br.ReadString()));
        }
        public override string ToString() => $"{fullName} (fields:{fields.ConvertAll(f=>$"{f.typeFullName} {f.name}").PrintCollection(", ")})";

        public override bool Equals(object obj) => (obj is SerializedStructMetaData otherMeta) ? (otherMeta.GetHashCode() == GetHashCode()) : false;
        public override int GetHashCode()
        {
            var hash = fullName.ToHash();
            hash = fields.SortedBy(c => c.name).ToHash(c => c.name.ToHash().ToHash(c.typeFullName.ToHash())).ToHash(hash);
            return (int)hash;
        }
    }
}
#endif