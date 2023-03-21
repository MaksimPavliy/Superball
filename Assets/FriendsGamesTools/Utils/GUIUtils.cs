using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class GUIUtils
    {
        public static void URL(string name, string url)
        {
            if (GUILayout.Button(name))
                Application.OpenURL(url);
        }
        static Dictionary<Type, (object[] vals, string[] names)> optionsData;
        public static (object[] vals, string[] names, int oldInd) GetOptionsData<T>(T currValue) where T : Enum
        {
            if (optionsData == null)
                optionsData = new Dictionary<Type, (object[] vals, string[] names)>();
            if (!optionsData.TryGetValue(typeof(T), out var item))
            {
                item.vals = Enum.GetValues(typeof(T)).ConvertAll(o => o).ToArray();
                item.names = item.vals.ConvertAll(i => i.ToString()).ToArray();
            }
            var oldInd = item.vals.FindIndex(v => ((int)v) == ((int)(object)currValue));
            return (item.vals, item.names, oldInd);
        }
        public static T Toolbar<T>(T val, float width = -1) where T : Enum
        {
            var (vals, names, oldInd) = GetOptionsData(val);
            int newInd;
            if (width < 0)
                newInd = GUILayout.Toolbar(oldInd, names);
            else
                newInd = GUILayout.Toolbar(oldInd, names, GUILayout.Width(width));
            return (T)vals[newInd];
        }
        public static string Toolbar(string val, string[] options, float width = -1)
        {
            var oldInd = options.IndexOf(val);
            int newInd;
            if (width < 0)
                newInd = GUILayout.Toolbar(oldInd, options);
            else
                newInd = GUILayout.Toolbar(oldInd, options, GUILayout.Width(width));
            if (newInd == -1)
                return string.Empty;
            return options[newInd];
        }


        public static bool IntField(string title, ref int value, ref bool changed, float width = -1)
        {
            var oldStr = value.ToString();
            string newStr;
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            if (width < 0)
                newStr = GUILayout.TextField(oldStr);
            else
                newStr = GUILayout.TextField(oldStr, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            if (newStr == oldStr)
                return false;
            if (!int.TryParse(newStr, out var newInt))
                return false;
            changed = true;
            value = newInt;
            return true;
        }
        public static bool TextField(string title, ref string value, ref bool changed, float width = -1)
        {
            string newValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            if (width < 0)
                newValue = GUILayout.TextField(value);
            else
                newValue = GUILayout.TextField(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
    }
} 
