using System;
using System.Collections;
using System.Collections.Generic;

namespace FriendsGamesTools
{
    // This is basically two-way dictionary.
    public class Lens<T1,T2> : IEnumerator<(T1, T2)>, IEnumerable<(T1,T2)>
    {
        Dictionary<T1, T2> T2ByT1 = new Dictionary<T1, T2>();
        Dictionary<T2, T1> T1ByT2 = new Dictionary<T2, T1>();
        public Lens(params (T1 t1,T2 t2)[] items)
        {
            foreach (var (t1, t2) in items)
            {
                T2ByT1.Add(t1, t2);
                T1ByT2.Add(t2, t1);
            }
        }
        public T2 this[T1 t1] => T2ByT1[t1];
        public T1 this[T2 t2] => T1ByT2[t2];

        public void Add(T1 t1, T2 t2)
        {
            T2ByT1.Add(t1, t2);
            T1ByT2.Add(t2, t1);
        }
        public void Add(T2 t2, T1 t1) => Add(t1, t2);

        public void Remove(T1 t1, T2 t2)
        {
            T2ByT1.Remove(t1);
            T1ByT2.Remove(t2);
        }
        public void Remove(T1 t1)
        {
            if (T2ByT1.TryGetValue(t1, out var t2))
                Remove(t1, t2);
        }
        public void Remove(T2 t2)
        {
            if (T1ByT2.TryGetValue(t2, out var t1))
                Remove(t1, t2);
        }

        public bool TryGetValue(T1 t1, out T2 t2) => T2ByT1.TryGetValue(t1, out t2);
        public bool TryGetValue(T2 t2, out T1 t1) => T1ByT2.TryGetValue(t2, out t1);

        #region IEnumerator
        public (T1, T2) Current => (enumerator.Current.Value, enumerator.Current.Key);
        object IEnumerator.Current => Current;
        Dictionary<T2,T1>.Enumerator enumerator;
        public void Reset() => enumerator = default;
        public void Dispose() => enumerator.Dispose();
        public bool MoveNext()
        {
            if (EqualityComparer<Dictionary<T2, T1>.Enumerator>.Default.Equals(enumerator, default))
                enumerator = T1ByT2.GetEnumerator();
            return enumerator.MoveNext();
        }
        public IEnumerator<(T1, T2)> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        #endregion
    }
    public class Lens<T> : IEnumerable<T> 
    {
        Dictionary<T, T> dict = new Dictionary<T, T>();
        public Lens(params (T t1, T t2)[] items)
        {
            foreach (var (t1, t2) in items)
            {
                Add(t1,t2);
                dict.Add(t1, t2);
                dict.Add(t2, t1);
            }
        }
        public T this[T t1] => dict[t1];
        public void Add(T t1, T t2)
        {
            dict.Add(t1, t2);
            if (!t1.Equals(t2))
                dict.Add(t2, t1);
        }

        public void Remove(T t)
        {
            if (!dict.TryGetValue(t, out var t2))
                return;
            dict.Remove(t);
            dict.Remove(t2);
        }

        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => dict.Keys.GetEnumerator();
    }
}