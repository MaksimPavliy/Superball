#if ECSGame
using Dict = System.Collections.Generic.Dictionary<string, object>;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class UnversionedEntity
    {
        public Dictionary<string, object> components = new Dictionary<string, object>();
        public Dictionary<string, List<object>> buffers = new Dictionary<string, List<object>>();

        #region Binary serialization
        public void Write(BinaryWriter bw, WorldMetaData meta)
        {
            bw.Write(components.Count);
            meta.components.ForEachWithInd((compMeta, compMetaInd)=> {
                if (!components.TryGetValue(compMeta.fullName, out var comp))
                    return;
                bw.Write(compMetaInd);
                WriteUnversioned((Dictionary<string, object>)comp, compMeta, bw, meta);
            });
            bw.Write(buffers.Count);
            meta.buffers.ForEachWithInd((buffMeta, buffMetaInd) => {
                if (!buffers.TryGetValue(buffMeta.fullName, out var buff))
                    return;
                bw.Write(buffMetaInd);
                SerializationUtils.WriteList(buff, bw, buffItem 
                    => WriteUnversioned((Dictionary<string, object>)buffItem, buffMeta, bw, meta));
            });
        }
        void WriteUnversioned(Dictionary<string, object> value, SerializedStructMetaData valueMeta, BinaryWriter bw, WorldMetaData meta)
        {
            foreach (var (fieldTypeName, fieldName) in valueMeta.fields)
            {
                var fieldValue = value[fieldName];
                // Write struct.
                var fieldValueStructMeta = meta.GetAnyStructMeta(fieldTypeName);
                if (fieldValueStructMeta != null) {
                    WriteUnversioned((Dictionary<string, object>)fieldValue, fieldValueStructMeta, bw, meta);
                    continue;
                }
                // Write enum.
                var fieldValueEnumMeta = meta.GetEnumMeta(fieldTypeName);
                if (fieldValueEnumMeta!=null)
                {
                    bw.Write((string)fieldValue);
                    continue;
                }
                // Write primitive.
                WorldMetaDataUtils.WritePrimitive(bw, fieldValue);
            }
        }
        public void Read(BinaryReader br, WorldMetaData meta)
        {
            components.Clear();
            int compsCount = br.ReadInt32();
            for (int i = 0; i < compsCount; i++)
            {
                var compTypeInd = br.ReadInt32();
                var compMeta = meta.components[compTypeInd];
                var unversioned = ReadUnversioned(compMeta, br, meta);
                components.Add(compMeta.fullName, unversioned);
            }
            buffers.Clear();
            int buffsCount = br.ReadInt32();
            for (int i = 0; i < buffsCount; i++)
            {
                var buffTypeInd = br.ReadInt32();
                var buffMeta = meta.buffers[buffTypeInd];
                var unversioned = new List<object>();
                SerializationUtils.ReadList(unversioned, br, () => ReadUnversioned(buffMeta, br, meta));
                buffers.Add(buffMeta.fullName, unversioned);
            }
        }
        object ReadUnversioned(SerializedStructMetaData valueMeta, BinaryReader br, WorldMetaData meta)
        {
            var value = new Dictionary<string, object>();
            foreach (var (fieldTypeName, fieldName) in valueMeta.fields)
            {
                object fieldValue;
                // Read struct.
                var fieldValueStructMeta = meta.GetAnyStructMeta(fieldTypeName);
                if (fieldValueStructMeta != null)
                    fieldValue = ReadUnversioned(fieldValueStructMeta, br, meta);
                else
                {
                    // Read enum.
                    var fieldValueEnumMeta = meta.GetEnumMeta(fieldTypeName);
                    if (fieldValueEnumMeta != null)
                        fieldValue = br.ReadString();
                    else  // Read primitive.
                        fieldValue = WorldMetaDataUtils.ReadPrimitive(br, fieldTypeName);
                }
                value.Add(fieldName, fieldValue);
            }
            return value;
        }
        #endregion

        public bool HasComponent(string componentName) => components.Keys.Any(fullName => fullName.EndsWith(componentName));
        public bool HasBuffer(string bufferName) => buffers.Keys.Any(fullName => fullName.EndsWith(bufferName));
        public bool Change<T>(string componentFullName, string fieldName, RefAction<T> action)
        {
            if (!components.TryGetValue(componentFullName, out var compObj))
                return false;
            var comp = (Dict)components[componentFullName];
            if (!comp.ContainsKey(fieldName))
                return false;
            var value = (T)comp[fieldName];
            action(ref value);
            comp[fieldName] = value;
            return true;
        }
    }
}
#endif