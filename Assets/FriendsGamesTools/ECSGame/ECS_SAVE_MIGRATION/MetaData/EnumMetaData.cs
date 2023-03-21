#if ECSGame

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class EnumMetaData : IBinarySerializable
    {
        public string fullName;
        public List<string> values = new List<string>();
        public static EnumMetaData CreateFromType(Type type)
        {
            Debug.Assert(type.IsEnum);
            var res = new EnumMetaData() { fullName = type.FullName };
            Enum.GetValues(type).ForEach(v=>res.values.Add(v.ToString()));
            return res;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(fullName);
            SerializationUtils.WriteList(values, bw, value => bw.Write(value));
        }
        public void Read(BinaryReader br)
        {
            fullName = br.ReadString();
            SerializationUtils.ReadList(values, br, () => br.ReadString());
        }
        public override string ToString() => $"{fullName} (values:{PrintValues()})";
        public string PrintValues() => values.PrintCollection(", ");

        public bool SameAs(EnumMetaData other)
        {
            if (other.values.Count != values.Count)
                return false;
            for (int i = 0; i < values.Count; i++)
            {
                if (!values.Contains(other.values[i]))
                    return false;
            }
            return true;
        }

        public static void GetDiffs(EnumMetaData oldEnum, EnumMetaData newEnum, out List<string> addedValues, out List<string> removedValues)
        {
            addedValues = new List<string>();
            removedValues = new List<string>();
            foreach (var value in newEnum.values)
                addedValues.Add(value);
            foreach (var value in oldEnum.values)
            {
                if (addedValues.Contains(value))
                    addedValues.Remove(value);
                else
                    removedValues.Add(value);
            }
        }

        public override bool Equals(object obj) => (obj is EnumMetaData otherMeta) ? (otherMeta.GetHashCode() == GetHashCode()) : false;
        public override int GetHashCode()
        {
            var hash = fullName.ToHash();
            hash = values.SortedBy(v => v).ToHash(v => v.GetHashCode());
            return (int)hash;
        }
    }
}
#endif