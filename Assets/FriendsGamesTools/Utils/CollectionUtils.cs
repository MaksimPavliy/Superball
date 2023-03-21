using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static List<T> ToListFromItem<T>(this T item) => new List<T> { item };
        public static void ReplaceValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue toBeReplaced, TValue toReplaceWith) {
            List<TKey> keys = null;
            foreach (var (key, value) in dict) {
                if (!EqualityComparer<TValue>.Default.Equals(value, toBeReplaced))
                    continue;
                keys = new List<TKey>();
                keys.Add(key);
            }
            keys?.ForEach(key => dict[key] = toReplaceWith);
        }
        public static IEnumerable<T> Unify<T, TCollection>(this IEnumerable<TCollection> collections, Func<TCollection, IEnumerable<T>> getItems) {
            foreach (var collection in collections) {
                var items = getItems(collection);
                var i = items.GetEnumerator();
                while (i.MoveNext())
                    yield return i.Current;
            }
        }
        public static TValue CachedGet<TKey, TValue>(TKey key, Func<TValue> calcValue, Dictionary<TKey, TValue> cache) {
            if (!cache.TryGetValue(key, out var value)) {
                value = calcValue();
                cache.Add(key, value);
            }
            return value;
        }
        public static void RemoveLast<T>(this List<T> list) => list.RemoveAt(list.LastInd());
        public static List<int> GetIndsList(int count) {
            var list = new List<int>();
            for (int i = 0; i < count; i++)
                list.Add(i);
            return list;
        }
        public static List<int> GetIndsList<T>(this IList<T> list) => GetIndsList(list.Count);
        public static bool All<T>(this (IEnumerable<T> items1, IEnumerable<T> items2) tuple, Func<T, T, bool> condition) {
            if (tuple.items1 == null || tuple.items2 == null)
                return false;
            foreach (var item1 in tuple.items1) {
                if (!tuple.items2.Any(item2 => condition(item1, item2)))
                    return false;
            }
            foreach (var item2 in tuple.items2) {
                if (!tuple.items1.Any(item1 => condition(item1, item2)))
                    return false;
            }
            return true;
        }
        public static bool Any<T>(this (IEnumerable<T> items1, IEnumerable<T> items2) tuple, Func<T, T, bool> condition) {
            var e1 = tuple.items1.GetEnumerator();
            var e2 = tuple.items2.GetEnumerator();
            var res = false;
            do {
                var e1Finished = !e1.MoveNext();
                var e2Finished = !e2.MoveNext();
                if (e1Finished || e2Finished)
                    break;
                if (condition(e1.Current, e2.Current)) {
                    res = true;
                    break;
                }
            } while (true);
            e1.Dispose();
            e2.Dispose();
            return res;
        }
        public static List<T> ToList<T>(this Tuple<T, T> tuple) => new List<T> { tuple.Item1, tuple.Item2 };
        public static List<T> ToList<T>(this Tuple<T, T, T> tuple) => new List<T> { tuple.Item1, tuple.Item2, tuple.Item3 };
        public static List<T> ToList<T>(this Tuple<T, T, T, T> tuple) => new List<T> { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };
        public static List<T> ToList<T>(this Tuple<T, T, T, T, T> tuple) => new List<T> { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        public static int IndexOf<T>(this IReadOnlyList<T> list, T item) {
            for (int i = 0; i < list.Count; i++) {
                if (EqualityComparer<T>.Default.Equals(list[i], item))
                    return i;
            }
            return -1;
        }
        public static int CountSafe<T>(this IList<T> list) => list == null ? 0 : list.Count;
        public static void AddSafe<T>(ref List<T> list, T item) {
            if (item == null) return;
            if (list == null) list = new List<T>();
            list.Add(item);
        }
        public static bool All<T>(this IEnumerable<T> items, Func<T, bool> predicate, int startInd, int endInd) {
            var ind = 0;
            foreach (var item in items) {
                if (ind < startInd) continue;
                if (ind > endInd) return true;
                if (!predicate(item)) return false;
                ind++;
            }
            return true;
        }
        public static int LastInd(this IList list) => list.Count - 1;
        public static void GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value, Func<TValue> getDefault = null) {
            if (!dict.TryGetValue(key, out value)) {
                if (getDefault == null)
                    value = default;
                else
                    value = getDefault();
                dict.Add(key, value);
            }
        }
        public static T MinItem<T>(this IEnumerable<T> items, Func<T, float> getValue) {
            T minItem = default;
            float min = float.MaxValue;
            foreach (var item in items) {
                var curr = getValue(item);
                if (curr < min) {
                    min = curr;
                    minItem = item;
                }
            }
            return minItem;
        }
        public static float Mean<T>(this IEnumerable<T> items, Func<T, float> getValue) {
            var count = 0;
            var sum = 0f;
            foreach (var item in items) {
                count++;
                sum += getValue(item);
            }
            return sum / count;
        }
        public static float Max<T>(this IEnumerable<T> items, Func<T, float> getValue, float defaultValue) {
            var anythingExists = false;
            var max = float.MinValue;
            foreach (var item in items) {
                anythingExists = true;
                var curr = getValue(item);
                max = Mathf.Max(max, curr);
            }
            if (!anythingExists)
                return defaultValue;
            return max;
        }
        public static float Min<T>(this IEnumerable<T> items, Func<T, float> getValue, float defaultValue) {
            var anythingExists = false;
            var min = float.MaxValue;
            foreach (var item in items) {
                anythingExists = true;
                var curr = getValue(item);
                min = Mathf.Min(min, curr);
            }
            if (!anythingExists)
                return defaultValue;
            return min;
        }
        public static T MaxItem<T>(this IEnumerable<T> items, Func<T, float> getValue) {
            T maxItem = default;
            float max = float.MinValue;
            foreach (var item in items) {
                var curr = getValue(item);
                if (curr > max) {
                    max = curr;
                    maxItem = item;
                }
            }
            return maxItem;
        }
        public static T GetElementSafe<T>(this List<T> items, int ind) {
            if (items.IndIsValid(ind))
                return items[ind];
            else
                return default;
        }
        public static T GetElementSafe<T>(this IList<T> items, int ind) {
            if (items.IndIsValid(ind))
                return items[ind];
            else
                return default;
        }
        public static T GetElementSafe<T>(this IReadOnlyList<T> items, int ind) where T : class {
            if (items.IndIsValid(ind))
                return items[ind];
            else
                return default;
        }
        public static T GetElementBounded<T>(this IReadOnlyList<T> items, int ind) where T : class
            => items[Mathf.Clamp(ind, 0, items.Count - 1)];
        public static void ForeachDuplicate<T, C>(IList<T> list, Func<T, C> whatToCompare, Action<int, int, C> onDuplicateFound)
            where C : IComparable {
            for (int i = 0; i < list.Count - 1; i++) {
                for (int j = i + 1; j < list.Count; j++) {
                    var c1 = whatToCompare(list[i]);
                    var c2 = whatToCompare(list[j]);
                    if (c1?.CompareTo(c2) == 0)
                        onDuplicateFound(i, j, c1);
                }
            }
        }
        public static T ClampedInd<T>(this IList<T> items, int ind) => items[items.ClampInd(ind)];
        public static int ClampInd<T>(this IList<T> list, int ind) => Mathf.Clamp(ind, 0, list.Count - 1);
        public static bool IndIsValid<T>(this List<T> list, int ind) => ind >= 0 && ind < list.Count;
        public static bool IndIsValid<T>(this IList<T> list, int ind) => ind >= 0 && ind < list.Count;
        public static bool IndIsValid<T>(this IReadOnlyList<T> list, int ind) where T : class => ind >= 0 && ind < list.Count;
        public static bool DebugIndIsValid<T>(this IList<T> list, int ind) {
            var valid = IndIsValid(list, ind);
            if (!valid)
                Debug.LogError($"ind is invalid, count = {list.Count}, ind = {ind}");
            return valid;
        }
        public static void SortPartialOrder<T>(this List<T> items, Func<T, T, int> compare) {
            bool changes;
            do {
                changes = false;
                for (int i = 0; i < items.Count - 1; i++) {
                    for (int j = i + 1; j < items.Count; j++) {
                        if (compare(items[i], items[j]) > 0) {
                            var itemAfterJ = items[i];
                            for (int k = i; k < j; k++)
                                items[k] = items[k + 1];
                            items[j] = itemAfterJ;
                            changes = true;
                            i--;
                            break;
                        }
                    }
                }
            } while (changes);
        }
        public static void FromTo(this (int start, int end) bounds, Action<int> action) {
            if (bounds.start < bounds.end) {
                for (int i = bounds.start; i <= bounds.end; i++)
                    action(i);
            } else {
                for (int i = bounds.start; i >= bounds.end; i--)
                    action(i);
            }
        }

        static Stack<StringBuilder> pcSB;
        static StringBuilder GetPCSB() {
            if (pcSB == null)
                pcSB = new Stack<StringBuilder>();
            return pcSB.Count > 0 ? pcSB.Pop() : new StringBuilder();
        }
        static void ReturnPCSB(StringBuilder sb) {
            sb.Clear();
            pcSB.Push(sb);
        }
        public static string PrintCollection<T>(this IEnumerable<T> items,
            string separator = ", ", string whenNoItems = "NoItems", int maxCount = -1,
            Func<T, string> toString = null, Func<T, bool> filter = null) {
            var sb = GetPCSB();

            var count = 0;
            foreach (var item in items) {
                if (filter != null && !filter(item))
                    continue;
                if (count > 0)
                    sb.Append(separator);
                if (toString == null)
                    sb.Append(item);
                else
                    sb.Append(toString(item));
                count++;
                if (maxCount != -1 && count >= maxCount)
                    break;
            }
            if (count == 0)
                sb.Append(whenNoItems);
            var str = sb.ToString();
            ReturnPCSB(sb);
            return str;
        }
        public static List<T> Reversed<T>(this IEnumerable<T> list) {
            List<T> reversed = new List<T>();
            foreach (var item in list)
                reversed.Insert(0, item);
            return reversed;
        }
        public static bool TryGetValue<T>(this List<T> list, Predicate<T> predicate, out T t) {
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i])) {
                    t = list[i];
                    return true;
                }
            }
            t = default;
            return false;
        }
        public static TValue GetValueWithDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
            => dict.TryGetValue(key, out var value) ? value : defaultValue;
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            int ind = 0;
            foreach (var item in items) {
                if (predicate(item)) return ind;
                ind++;
            }
            return -1;
        }
        public static T Find<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            foreach (var item in items)
                if (predicate(item)) return item;
            return default(T);
        }
        public static T FindOnlyOne<T>(this IEnumerable<T> items, Predicate<T> predicate) {
            var theOnlyResult = default(T);
            var anythingFound = false;
            foreach (var item in items) {
                if (!predicate(item)) continue;
                if (anythingFound) return default;
                theOnlyResult = item;
                anythingFound = true;
            }
            return theOnlyResult;
        }
        public static int FindLastIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            int lastInd = -1;
            int ind = 0;
            foreach (var item in items) {
                if (predicate(item)) lastInd = ind;
                ind++;
            }
            return lastInd;
        }
        public static void Deconstruct<T, U>(this KeyValuePair<T, U> k, out T t, out U u) {
            t = k.Key; u = k.Value;
        }
        public static void ForEach<T>(this T[] array, Action<T> action) {
            foreach (var t in array)
                action(t);
        }
        public static void ForEach(this Array array, Action<object> action) {
            foreach (var t in array)
                action(t);
        }
        public static void ForEachWithInd<T>(this IEnumerable<T> items, Action<T, int> action) {
            int ind = 0;
            foreach (var t in items) {
                action(t, ind);
                ind++;
            }
        }
        public static void ForEachDownTo<T>(this IList<T> items, Action<T> action) {
            for (int i = items.Count - 1; i >= 0; i--)
                action(items[i]);
        }
        public static void ForEach<TEnum>(Action<TEnum> action) where TEnum : Enum {
            foreach (TEnum type in Enum.GetValues(typeof(TEnum)))
                action(type);
        }
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
            foreach (var t in items)
                action(t);
        }
        public static List<TOut> ConvertAll<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> convert) {
            var res = new List<TOut>();
            foreach (var item in items)
                res.Add(convert(item));
            return res;
        }
        public static List<TOut> ConvertAll<TOut>(this IEnumerable items, Func<object, TOut> convert) {
            var res = new List<TOut>();
            foreach (var item in items)
                res.Add(convert(item));
            return res;
        }
        public static List<TOut> ConvertAllWithInd<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, int, TOut> convert) {
            var res = new List<TOut>();
            var ind = 0;
            foreach (var item in items) {
                res.Add(convert(item, ind));
                ind++;
            }
            return res;
        }
        public static List<T> ToList<T>(this IList<T> list) {
            List<T> res = new List<T>();
            for (int i = 0; i < list.Count; i++)
                res.Add(list[i]);
            return res;
        }
        public static List<T> ToList<T>(this Array items) {
            var list = new List<T>();
            foreach (T item in items)
                list.Add(item);
            return list;
        }
        public static void Repeat(int count, Action action) {
            for (int i = 0; i < count; i++)
                action();
        }
        public static List<T> Adding<T>(this List<T> list, T item) {
            list.Add(item);
            return list;
        }
        public static List<T> Without<T>(this List<T> list, T item) {
            list.Remove(item);
            return list;
        }
        public static List<T> AddingRange<T>(this List<T> list, IEnumerable<T> items) {
            list.AddRange(items);
            return list;
        }
        public static void AddRepetative<T>(this List<T> list, T item, int count) {
            for (int i = 0; i < count; i++)
                list.Add(item);
        }
        public static void AddIfNotExists<T>(this List<T> list, T item) {
            if (!list.Contains(item))
                list.Add(item);
        }
        public static void AddRangeIfNotExists<T>(this List<T> list, IEnumerable<T> range) {
            foreach (var item in range)
                list.AddIfNotExists(item);
        }
        public static void AddIfNotNull<T>(this List<T> list, T item) where T : class {
            if (item != null)
                list.Add(item);
        }
        public static List<T> Filter<T>(this IEnumerable<T> items, Func<T, bool> filter) {
            if (filter == null)
                return items.Clone();
            var res = new List<T>();
            foreach (var item in items) {
                if (filter(item))
                    res.Add(item);
            }
            return res;
        }
        public static void RemoveNulls<T>(this List<T> items) where T : class {
            for (int i = items.Count - 1; i >= 0; i--) {
                if (items[i] == null)
                    items.RemoveAt(i);
            }
        }
        public static List<T> FilterNulls<T>(this IEnumerable<T> items) where T : class
            => Filter(items, item => item != null);
        public static List<TOut> FilterConvert<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> convert) where TOut : class {
            var res = new List<TOut>();
            foreach (var item in items) {
                var itemConverted = convert(item);
                if (itemConverted != null)
                    res.Add(itemConverted);
            }
            return res;
        }
        public static List<TOut> FilterConvert<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, (TOut res, bool filter)> convert) where TOut : struct {
            var res = new List<TOut>();
            foreach (var item in items) {
                var (itemConverted, filter) = convert(item);
                if (filter)
                    res.Add(itemConverted);
            }
            return res;
        }
        // Let linq do this.
        //public static bool Contains<T>(this IEnumerable<T> items, T item) where T : class
        //{
        //    if (item == null)
        //        return false;
        //    foreach (var i in items) {
        //        if (item.Equals(i))
        //            return true;
        //    }
        //    return false;
        //}
        // Let linq do this.
        //public static bool All<T>(this IEnumerable<T> items, Func<T, bool> condition)
        //{
        //    if (condition == null)
        //        return false;
        //    foreach (var i in items) {
        //        if (!condition(i))
        //            return false;
        //    }
        //    return true;
        //}
#if ECSGame
        public static bool Any<T>(this Unity.Entities.DynamicBuffer<T> items, Func<T, bool> condition) where T : struct, Unity.Entities.IBufferElementData
        {
            if (condition == null)
                return false;
            foreach (var i in items)
            {
                if (condition(i))
                    return true;
            }
            return false;
        }
        public static bool All<T>(this Unity.Entities.DynamicBuffer<T> items, Func<T, bool> condition) where T : struct, Unity.Entities.IBufferElementData
        {
            if (condition == null)
                return false;
            foreach (var i in items)
            {
                if (!condition(i))
                    return false;
            }
            return true;
        }
        public static List<T> Clone<T>(this Unity.Entities.DynamicBuffer<T> items) where T : struct, Unity.Entities.IBufferElementData
        {
            var list = new List<T>();
            foreach (var i in items)
                list.Add(i);
            return list;
        }
#endif
        public static List<T> Clone<T>(this IEnumerable<T> list) {
            var clone = new List<T>();
            foreach (var item in list)
                clone.Add(item);
            return clone;
        }
        public static Dictionary<T1, T2> Clone<T1, T2>(this Dictionary<T1, T2> dict) {
            var clone = new Dictionary<T1, T2>();
            foreach (var (key, value) in dict)
                clone.Add(key, value);
            return clone;
        }
        public static void SortBy<TItem, TNumber>(this List<TItem> items, Func<TItem, TNumber> byWhat, bool descending = false)
            where TNumber : IComparable<TNumber> => items.Sort((i1, i2) => (descending ? -1 : 1) * byWhat(i1).CompareTo(byWhat(i2)));
        public static List<TItem> SortedBy<TItem, TNumber>(this List<TItem> items, Func<TItem, TNumber> byWhat, bool descending = false)
            where TNumber : IComparable<TNumber> {
            items.SortBy(byWhat, descending);
            return items;
        }
        public static void AddSorted<TItem, TNumber>(this List<TItem> items, TItem item, Func<TItem, TNumber> byWhat, bool descending = false)
            where TNumber : IComparable<TNumber> {
            var itemValue = byWhat(item);
            for (int i = 0; i < items.Count; i++) {
                var currValue = byWhat(items[i]);
                var insertHere = currValue.CompareTo(itemValue) * (descending ? -1 : 1) >= 0;
                if (insertHere) {
                    items.Insert(i, item);
                    return;
                }
            }
            items.Add(item);
        }
        public static int IndexOfExpensive<T>(this IEnumerable<T> items, T item) {
            var c = EqualityComparer<T>.Default;
            var ind = 0;
            foreach (var curr in items) {
                if (c.Equals(curr, item))
                    return ind;
                ind++;
            }
            return -1;
        }
        public static T GetElementSafeExpensive<T>(this IEnumerable<T> items, int ind) {
            var i = 0;
            foreach (var curr in items) {
                if (i == ind)
                    return curr;
                i++;
            }
            return default;
        }
    }
}