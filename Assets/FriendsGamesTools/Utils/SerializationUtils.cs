using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;

namespace FriendsGamesTools
{
    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class SerializationUtils
	{
        /// <summary>
        /// Encode serializable object to base64 string.
        /// </summary>
        public static string EncodeToString(this object val)
            => val.SerializeToByteArray().EncodeToString();

        public static string EncodeToString(this byte[] encodedBytes)
        {
            if (encodedBytes == null) return null;
            return Convert.ToBase64String(encodedBytes);
        }

        /// <summary>
        /// Decode serializable object from base64 string.
        /// </summary>
        public static T DecodeFromString<T>(string encodedString) where T : class
        {
            var bytesEncoded = DecodeFromString(encodedString);
            if (bytesEncoded == null)
                return null;
            var val = Deserialize<T>(bytesEncoded);
            return val;
        }

        public static byte[] DecodeFromString(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString)) return null;
            return Convert.FromBase64String(encodedString);
        }

        public static byte[] SerializeToByteArray(this object obj)
		{
			if (obj == null)
			{
				return null;
			}
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static T Deserialize<T>(this byte[] data, int dataLength = -1) where T : class
		{
			if (data == null)
			{
				return null;
			}
			using (var memStream = new MemoryStream())
			{
                var length = dataLength == -1 ? data.Length : dataLength;
                var binForm = new BinaryFormatter();
				memStream.Write(data, 0, length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = (T)binForm.Deserialize(memStream);
				return obj;
			}
		}

        public static void WriteList<T>(List<T> list, BinaryWriter bw) where T : IBinarySerializable
            => WriteList(list, bw, item => item.Write(bw));
        public static void ReadList<T>(List<T> list, BinaryReader br) where T : IBinarySerializable, new()
            => ReadList(list, br, () => { var item = new T(); item.Read(br); return item; });
        public static void WriteList<T>(List<T> list, BinaryWriter bw, Action<T> serialize)
        {
            bw.Write(list.Count);
            list.ForEach(serialize);
            //Debug.Log($"write list({list.Count}): {list.ConvertAll(i=>i.ToString()).PrintCollection(", ")}");
        }
        public static void ReadList<T>(List<T> list, BinaryReader br, Func<T> deserialize)
        {
            list.Clear();
            var count = br.ReadInt32();
            //const int InsaneCount = 100000;
            //Debug.Assert(count < InsaneCount, "dict size is insane");
            for (int i = 0; i < count; i++)
                list.Add(deserialize());
            //Debug.Log($"read list({list.Count}): {list.ConvertAll(i => i.ToString()).PrintCollection(", ")}");
        }
    }


    public interface IBinarySerializable
    {
        void Write(BinaryWriter bw);
        void Read(BinaryReader br);
    }
}