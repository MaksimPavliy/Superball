using System;
using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class InPrefs<T>
    {
        protected readonly string key;
        readonly T defaultValue;
        protected abstract T Read();
        protected abstract void Write(T value);
        public InPrefs(string key, T defaultValue = default)
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }
        public T value
        {
            get => PlayerPrefs.HasKey(key) ? Read() : defaultValue;
            set => Write(value);
        }
        public static implicit operator T(InPrefs<T> item) => item.value;
    }
    public class BoolInPrefs : InPrefs<bool>
    {
        public BoolInPrefs(string key, bool defaultValue = default) : base(key, defaultValue) { }
        protected override bool Read() => PlayerPrefs.GetInt(key) == 1;
        protected override void Write(bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
    public class IntInPrefs : InPrefs<int>
    {
        public IntInPrefs(string key, int defaultValue = default) : base(key, defaultValue) { }
        protected override int Read() => PlayerPrefs.GetInt(key);
        protected override void Write(int value) => PlayerPrefs.SetInt(key, value);
    }
    public class FloatInPrefs : InPrefs<float>
    {
        public FloatInPrefs(string key, float defaultValue = default) : base(key, defaultValue) { }
        protected override float Read() => PlayerPrefs.GetFloat(key);
        protected override void Write(float value) => PlayerPrefs.SetFloat(key, value);
    }
    public class StringInPrefs : InPrefs<string>
    {
        public StringInPrefs(string key, string defaultValue = default) : base(key, defaultValue) { }
        protected override string Read() => PlayerPrefs.GetString(key);
        protected override void Write(string value) => PlayerPrefs.SetString(key, value);
    }
    public class EnumInPrefs<T> : InPrefs<T> where T: Enum
    {
        public EnumInPrefs(string key, T defaultValue = default) : base(key, defaultValue) { }
        protected override T Read() => (T)Enum.Parse(typeof(T), PlayerPrefs.GetString(key));
        protected override void Write(T value) => PlayerPrefs.SetString(key, value.ToString());
    }
}