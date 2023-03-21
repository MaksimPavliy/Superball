#if ECSGame

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class WorldMetaDataUtils
    {
        public static List<Type> allowedPrimitiveTypes = new List<Type>();            
        private static Dictionary<Type, Action<object, BinaryWriter>> writePrimitive = new Dictionary<Type, Action<object, BinaryWriter>>();
        private static Dictionary<string, Func<BinaryReader, object>> readPrimitive = new Dictionary<string, Func<BinaryReader, object>>();
        private static void AddPrimitiveType(Type t, Action<object, BinaryWriter> write, Func<BinaryReader, object> read)
        {
            allowedPrimitiveTypes.Add(t);
            writePrimitive.Add(t, write);
            readPrimitive.Add(t.FullName, read);
        }
        public static void WritePrimitive(BinaryWriter bw, object value) => writePrimitive[value.GetType()](value, bw);
        public static object ReadPrimitive(BinaryReader br, string fullTypeName) => readPrimitive[fullTypeName](br);

        public static List<Type> allStructTypes;
        public static List<Type> componentTypes;
        public static List<Type> bufferItemTypes;
        public static List<Type> serializedStructs;
        public static List<Type> enumTypes;
        static HashSet<string> primitiveTypesHash;
        public static bool PrimitiveTypeIsAllowed(string typeFullName) => primitiveTypesHash.Contains(typeFullName);
        public static Type GetPrimitiveType(string typeFullName) => allowedPrimitiveTypes.Find(t => t.FullName == typeFullName);
        static WorldMetaDataUtils()
        {
            AddPrimitiveType(typeof(bool), (val, bw) => bw.Write((bool)val), br => br.ReadBoolean());
            AddPrimitiveType(typeof(byte), (val, bw) => bw.Write((byte)val), br => br.ReadByte());
            AddPrimitiveType(typeof(char), (val, bw) => bw.Write((char)val), br => br.ReadChar());
            AddPrimitiveType(typeof(decimal), (val, bw) => bw.Write((decimal)val), br => br.ReadDecimal());
            AddPrimitiveType(typeof(double), (val, bw) => bw.Write((double)val), br => br.ReadDouble());
            AddPrimitiveType(typeof(float), (val, bw) => bw.Write((float)val), br => br.ReadSingle());
            AddPrimitiveType(typeof(int), (val, bw) => bw.Write((int)val), br => br.ReadInt32());
            AddPrimitiveType(typeof(long), (val, bw) => bw.Write((long)val), br => br.ReadInt64());

            primitiveTypesHash = new HashSet<string>();
            allowedPrimitiveTypes.ForEach(t => primitiveTypesHash.Add(t.FullName));
            allStructTypes = new List<Type>();
            bool TypeIsReferenced(Type type) => allStructTypes.Any(c => GetFieldsToSerialize(c).Any(f => f.FieldType == type));
            componentTypes = ReflectionUtils.GetAllDerivedTypes<IComponentData>().Filter(t => t.IsStruct());
            allStructTypes.AddRange(componentTypes);
            bufferItemTypes = ReflectionUtils.GetAllDerivedTypes<IBufferElementData>().Filter(
                t => t.IsStruct());
            allStructTypes.AddRange(bufferItemTypes);
            var serializedStructsNotReferenced = ReflectionUtils.GetAllUnityAssemblyTypes().Filter(
                t => t.IsStruct() && t.HasAttribute<SerializableAttribute>());
            serializedStructs = new List<Type>();
            bool changed;
            do
            {
                changed = false;
                for (int i = serializedStructsNotReferenced.Count - 1; i >= 0; i--)
                {
                    var t = serializedStructsNotReferenced[i];
                    if (TypeIsReferenced(t))
                    {
                        changed = true;
                        serializedStructsNotReferenced.RemoveAt(i);
                        serializedStructs.Add(t);
                        allStructTypes.Add(t);
                    }
                }
            } while (changed);
            enumTypes = ReflectionUtils.GetAllUnityAssemblyTypes().Filter(t => t.IsEnum && TypeIsReferenced(t));
        }
        public static FieldInfo[] GetFieldsToSerialize(Type type)
            => type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        #region Current metadata verification
        public static bool IsCurrentMetaValid(StringBuilder sb = null)
        {
            bool valid = true;
            componentTypes.ForEach(c =>
            {
                if (!TypeAllowed(c, sb))
                    valid = false;
            });
            bufferItemTypes.ForEach(c =>
            {
                if (!SerializedStructDataTypeAllowed(c, sb))
                    valid = false;
            });
            return valid;
        }
        public static bool TypeAllowed(Type type, StringBuilder sb = null)
        {
            var valid = true;
            if (allowedPrimitiveTypes.Contains(type))
                return valid;
            if (type.IsNested && !type.IsNestedPublic)
            {
                valid = false;
                sb?.AppendLine($"type {type.FullName} is not public and nested, migration cant access it");
            }
            if (type.IsEnum)
                return valid;
            if (type.IsStruct())
            {
                valid &= SerializedStructDataTypeAllowed(type, sb);
                return valid;
            }
            sb?.AppendLine($"type {type.Name} is not serialized struct, enum or one of {allowedPrimitiveTypes.ConvertAll(t => t.Name).PrintCollection(", ")}");
            valid = false;
            return valid;
        }
        public static bool SerializedStructDataTypeAllowed(Type type, StringBuilder sb = null)
        {
            var valid = true;
            if (!type.IsStruct())
            {
                sb?.AppendLine($"{type.Name} should be struct");
                valid = false;
            }
            if (!allStructTypes.Contains(type))
            {
                sb?.AppendLine($"struct {type.Name} not found in executing assembly");
                valid = false;
            }
            if (!typeof(IComponentData).IsAssignableFrom(type)
                && 
                !typeof(IBufferElementData).IsAssignableFrom(type) 
                && 
                !type.HasAttribute<SerializableAttribute>())
            {
                sb?.AppendLine($"struct {type.Name} should be IComponentData or IBufferElementData or [Serializable]");
                valid = false;
            }
            if (!GetFieldsToSerialize(type).All(f => TypeAllowed(f.FieldType, sb)))
                valid = false;
            return valid;
        }
        #endregion
    }
}
#endif