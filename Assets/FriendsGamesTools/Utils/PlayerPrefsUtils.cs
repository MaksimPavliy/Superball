using System;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class PlayerPrefsUtils
    {
        public static void SetDouble(string key, double val)
        {
            var bytes = BitConverter.GetBytes(val);
            var str = Convert.ToBase64String(bytes);
            PlayerPrefs.SetString(key, str);
        }
        public static double GetDouble(string key, double defaultVal = -1)
        {
            if (!PlayerPrefs.HasKey(key))
                return defaultVal;
            var str = PlayerPrefs.GetString(key);
            var bytes = Convert.FromBase64String(str);
            var res = BitConverter.ToDouble(bytes, 0);
            return res;
        }
        public static bool GetBool(string key, bool defaultValue = false)
            => PlayerPrefs.GetInt(key, defaultValue?1:0) == 1;
        public static void SetBool(string key, bool value)
            => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public static T GetEnum<T>(string key, T defaultValue = default) where T : struct
            => Enum.TryParse<T>(PlayerPrefs.GetString(key), out var result) ? result : defaultValue;
        public static void SetEnum<T>(string key, T value) where T : Enum
            => PlayerPrefs.SetString(key, value.ToString());
    }
}