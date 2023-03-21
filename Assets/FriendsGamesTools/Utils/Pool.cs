using System;
using System.Collections.Generic;

namespace FriendsGamesTools
{
    public class Pool<T> {
        Stack<T> inPool = new Stack<T>();
        Func<T> create;
        T Create() {
            totalItemsCount++;
            return create();
        }
        public Pool(int startCount, Func<T> create)
        {
            this.create = create;
            for (int i = 0; i < startCount; i++)
                inPool.Push(Create());
        }
        public T Get() => inPool.Count > 0 ? inPool.Pop() : Create();
        public void Return(T item) => inPool.Push(item);
        public int totalItemsCount { get; private set; }
    }
}