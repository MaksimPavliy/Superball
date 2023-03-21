using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public class Delayed<T>
    {
        float delay;
        public Delayed(float delay) => this.delay = delay;
        Queue<(float time, T value)> values = new Queue<(float time, T value)>();
        public void SetCurrValue(T value) => values.Enqueue((Time.time, value));
        public T value
        {
            get
            {
                if (values.Count == 0) return default;
                var delayedTime = Time.time - delay;
                while (values.Count > 1 && values.Peek().time < delayedTime)
                    values.Dequeue();
                return values.Peek().value;
            }
        }
    }
}