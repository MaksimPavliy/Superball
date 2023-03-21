#if ECSGame

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class WorldMetaData : IBinarySerializable
    {
        public static WorldMetaData instance; // This data is currently reading or writing.

        #region Version with check, serialized independant
        public int version;
        public void WriteVersion(BinaryWriter wr)
        {
            wr.Write(version);
            wr.Write(version.GetHashCode());
        }
        public static int ReadVersion(BinaryReader br)
        {
            var version = br.ReadInt32();
            var hashFromFile = br.ReadInt32();
            var calcedHash = version.GetHashCode();
            if (calcedHash != hashFromFile)
                return -1;
            else
                return version;
        }
        #endregion

        public List<SerializedStructMetaData> components { get; private set; } = new List<SerializedStructMetaData>();
        public List<SerializedStructMetaData> serializedStructs { get; private set; } = new List<SerializedStructMetaData>();
        public List<SerializedStructMetaData> buffers { get; private set; } = new List<SerializedStructMetaData>();
        public List<EnumMetaData> enums { get; private set; } = new List<EnumMetaData>();
        public bool IsEnum(string fieldTypeFullName) => enums.Any(metaData => metaData.fullName == fieldTypeFullName);
        public bool IsSerializedStruct(string fieldTypeFullName) => serializedStructs.Any(metaData => metaData.fullName == fieldTypeFullName);
        public bool IsBuffer(string fieldTypeFullName) => buffers.Any(metaData => metaData.fullName == fieldTypeFullName);
        public bool IsComponent(string fieldTypeFullName) => components.Any(metaData => metaData.fullName == fieldTypeFullName);
        public static WorldMetaData CreateCurrent()
        {
            WorldMetaData data = new WorldMetaData();
            data.version = DataVersion.versionInd;
            WorldMetaDataUtils.componentTypes.ForEach(type=> data.components.Add(SerializedStructMetaData.CreateFromType(type)));
            WorldMetaDataUtils.serializedStructs.ForEach(type => data.serializedStructs.Add(SerializedStructMetaData.CreateFromType(type)));
            WorldMetaDataUtils.bufferItemTypes.ForEach(type => data.buffers.Add(SerializedStructMetaData.CreateFromType(type)));
            WorldMetaDataUtils.enumTypes.ForEach(type => data.enums.Add(EnumMetaData.CreateFromType(type)));
            return data;
        }
        public void Write(BinaryWriter bw)
        {
            SerializationUtils.WriteList(components, bw);
            SerializationUtils.WriteList(serializedStructs, bw);
            SerializationUtils.WriteList(buffers, bw);
            SerializationUtils.WriteList(enums, bw);
            //Debug.Log($"saved {ToString()}");
        }
        public void Read(BinaryReader br)
        {
            SerializationUtils.ReadList(components, br);
            SerializationUtils.ReadList(serializedStructs, br);
            SerializationUtils.ReadList(buffers, br);
            SerializationUtils.ReadList(enums, br);
            //Debug.Log($"loaded {ToString()}");
        }
        public string EncodeToString()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            WriteVersion(bw);
            Write(bw);
            bw.Close();
            var bytes = ms.ToArray();
            var str = bytes.EncodeToString();
            return str;
        }
        public static WorldMetaData DecodeFromString(string str)
        {
            var bytes = SerializationUtils.DecodeFromString(str);
            if (bytes == null)
                return null;
            var ms = new MemoryStream(bytes);
            var br = new BinaryReader(ms);
            var res = new WorldMetaData();
            res.version = ReadVersion(br);
            res.Read(br);
            br.Close();
            return res;
        }
        public override string ToString()
            => $"metadata:\n" +
                $"version {version}\n" +
                $"\t{components.Count} components\n" +
                $"{components.ConvertAll(c => $"\t\t{c}").PrintCollection("\n", "")}\n" +
                $"\t{serializedStructs.Count} serializedStructs\n" +
                $"{serializedStructs.ConvertAll(c => $"\t\t{c}").PrintCollection("\n", "")}\n" +
                $"\t{buffers.Count} buffers\n" +
                $"{buffers.ConvertAll(c => $"\t\t{c}").PrintCollection("\n", "")}\n" +
                $"\t{enums.Count} enums\n" +
                $"{enums.ConvertAll(c => $"\t\t{c}").PrintCollection("\n", "")}\n";

        public Dictionary<string, string> CreateShortNames()
        {
            int namespaces = 0;
            var dict1 = new Dictionary<string, string>();
            components.ForEach(i => dict1.Add(i.fullName, ToShort(i.fullName, namespaces)));
            serializedStructs.ForEach(i => dict1.Add(i.fullName, ToShort(i.fullName, namespaces)));
            buffers.ForEach(i => dict1.Add(i.fullName, ToShort(i.fullName, namespaces)));
            enums.ForEach(i => dict1.Add(i.fullName, ToShort(i.fullName, namespaces)));
            WorldMetaDataUtils.allowedPrimitiveTypes.ForEach(t => dict1.Add(t.FullName, ToShort(t.FullName, namespaces)));
            var dict2 = new Dictionary<string, string>();
            do
            {
                dict2.Clear();
                var duplicatesExist = false;
                foreach (var (full, shortened) in dict1)
                {
                    var duplicateShort = dict1.Values.Count(v => v == shortened) > 1;
                    var newShorteded = duplicateShort ? ToShort(full, namespaces) : shortened;
                    dict2.Add(full, newShorteded);
                    if (duplicateShort)
                        duplicatesExist = true;
                }
                if (!duplicatesExist)
                    break;
                Utils.Swap(ref dict1, ref dict2);
                namespaces++;
            } while (true);
            return dict2;

            string ToShort(string fullName, int namespacesAllowed)
            {
                int namespacesFound = 0;
                int ind = fullName.Length - 1;
                while (ind >= 0)
                {
                    if (fullName[ind] == '.')
                    {
                        namespacesFound++;
                        if (namespacesFound >= namespacesAllowed)
                            break;
                    }
                    ind--;
                }
                var name = fullName.Substring(ind + 1);
                name = name.Replace(".", "").Replace("+", "_");
                return name;
            }
        }
        public static string ToWriteableFullName(string typeFullName) => typeFullName.Replace("+", ".");

        public SerializedStructMetaData GetComponentMeta(string fullName) => components.Find(c => c.fullName == fullName);
        public SerializedStructMetaData GetBufferMeta(string fullName)  => buffers.Find(c => c.fullName == fullName);
        public SerializedStructMetaData GetSerializedStructMeta(string fullName) => serializedStructs.Find(c => c.fullName == fullName);
        public EnumMetaData GetEnumMeta(string fullName) => enums.Find(c => c.fullName == fullName);
        public SerializedStructMetaData GetAnyStructMeta(string fullName) 
            => GetComponentMeta(fullName) ?? GetBufferMeta(fullName) ?? GetSerializedStructMeta(fullName);

        public static bool operator ==(WorldMetaData obj1, WorldMetaData obj2)
            => obj1?.GetHashCode() == obj2?.GetHashCode();
        public static bool operator !=(WorldMetaData obj1, WorldMetaData obj2)
            => !(obj1 == obj2);
        public override bool Equals(object obj) => this == (obj as WorldMetaData);
        public override int GetHashCode()
        {
            var hash = Utils.ToHash(version);
            hash = components.SortedBy(c => c.fullName).ToHash(c => c.GetHashCode()).ToHash(hash);
            hash = serializedStructs.SortedBy(c => c.fullName).ToHash(c => c.GetHashCode()).ToHash(hash);
            hash = buffers.SortedBy(c => c.fullName).ToHash(c => c.GetHashCode()).ToHash(hash);
            hash = enums.SortedBy(c => c.fullName).ToHash(c => c.GetHashCode()).ToHash(hash);
            return (int)hash;
        }
    }
}
#endif