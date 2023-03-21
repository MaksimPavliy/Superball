using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static List<T> Shuffle<T>(this IEnumerable<T> items)
        {
            var list = new List<T>();
            items.ForEach(item => list.Add(item));
            for (int i = 0; i < list.Count - 1; i++)
                list.Swap(i, UnityEngine.Random.Range(i, list.Count));
            return list;
        }
        public static long RandomLong() => (((long)RandomInt()) << 32) + RandomInt();
        public static int RandomInt() => UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        public static float RandomBetween(float v1, float v2) => Random(Mathf.Min(v1, v2), Mathf.Max(v1, v2));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">including</param>
        /// <param name="to">including</param>
        /// <returns></returns>
        public static int Random(int from, int to) => UnityEngine.Random.Range(from, to + 1);
        public static float Random(float from, float to) => UnityEngine.Random.Range(from, to);
        public static int RandomFromProbabilities(params float[] weights)
        {
            var sum = weights.Sum();
            var val = UnityEngine.Random.value * sum;
            for (int i = 0; i < weights.Length; i++)
            {
                val -= weights[i];
                if (val < 0)
                    return i;
            }
            return weights.Length - 1;
        }
        public static bool Chance(float chance) => UnityEngine.Random.value < chance;
        public static T RandomValue<T>() where T : Enum
        {
            var vals = Enum.GetValues(typeof(T));
            var ind = RandomInd(vals.Length);
            int currInd = 0;
            foreach (var val in vals)
            {
                if (ind == currInd)
                    return (T)val;
                currInd++;
            }
            return default;
        }
        public static T RandomEnumElement<T>() where T: Enum
        {
            var values = Enum.GetValues(typeof(T));
            var ind = RandomInd(values.Length);
            return (T)(values as IList)[ind];
        }
        public static List<T> EnumValues<T>() where T : Enum
            => Enum.GetValues(typeof(T)).ConvertAll(t => (T)t);
        public static T RandomElement<T>(this NativeArray<T> list) where T : struct
            => list[UnityEngine.Random.Range(0, list.Length)];
        public static T RandomElement<T>(this List<T> list, T ifEmpty = default)
            => list.Count == 0 ? ifEmpty : list[list.RandomInd()];
        public static T RandomElement<T>(this IReadOnlyList<T> list, T ifEmpty = default)
            => list.Count == 0 ? ifEmpty : list[list.RandomInd()];
        public static int RandomInd<T>(this List<T> list) => RandomInd(list.Count);
        public static int RandomInd<T>(this ICollection<T> list) => RandomInd(list.Count);
        public static int RandomInd<T>(this IReadOnlyCollection<T> list) => RandomInd(list.Count);
        public static int RandomIndWhere<T>(this IList<T> list, Func<T, bool> condition)
        {
            var filtered = list.Filter(condition);
            var item = filtered.RandomElement();
            return list.IndexOf(item);
        }
        public static int RandomInd(int count) => UnityEngine.Random.Range(0, count);
    }
}