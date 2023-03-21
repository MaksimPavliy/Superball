using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class EditorGUIUtils
    {
        public static void DoIfConfirmed(string confirmationQuestion, Action onConfirmed)
        {
            if (EditorUtility.DisplayDialog("Are you sure?", confirmationQuestion, "ok", "cancel"))
                onConfirmed.Invoke();
        }
        public static void ShowInfoWindow(string message)
            => EditorUtility.DisplayDialog("Info", message, "ok");
        public static Color red = new Color(1, 0, 0, 1);
        public static Color gray = Color.gray;
        public static Color green = new Color(0, 0.7f, 0, 1);
        public static Color warningColor = new Color(0.3f, 0.3f, 0, 1);
        public static void Error(string error) => WithColor(red, () => RichMultilineLabel(error));
        public static void WithColor(Color col, Action action)
        {
            EditorStyles.label.normal.textColor = col;
            action?.Invoke();
            EditorStyles.label.normal.textColor = Color.black;
        }
        static Stack<Color> backColorsStack;
        public static void SetBackgroundColor(Color col)
        {
            if (backColorsStack == null) backColorsStack = new Stack<Color>();
            backColorsStack.Push(GUI.backgroundColor);
            GUI.backgroundColor = col;
        }
        public static void RestoreBackgroundColor()
        {
            GUI.backgroundColor = backColorsStack.Pop();
        }
        public static void InHorizontal(Action action)
        {
            GUILayout.BeginHorizontal();
            action?.Invoke();
            GUILayout.EndHorizontal();
        }
        public static void ColoredLabel(string text, Color col, int width = -1)
            => WithColor(col, () => RichMultilineLabel(text, width));
        public static void RichLabel(string str, TextAnchor anchor = TextAnchor.MiddleLeft,
            bool eatAllWidth = false, bool eatAllHeight = false, Color col = new Color(),
            FontStyle fontStyle = FontStyle.Normal, bool italics = false, int width = -1, bool richText = false)
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = anchor };
            style.fontStyle = fontStyle;
            style.richText = richText;
            List<GUILayoutOption> options = new List<GUILayoutOption>();
            if (eatAllWidth)
                options.Add(GUILayout.ExpandWidth(true));
            if (eatAllHeight)
                options.Add(GUILayout.ExpandHeight(true));
            if (width != -1)
                options.Add(GUILayout.Width(width));
            var prevCol = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = col;
            EditorGUILayout.LabelField(str, style, options.ToArray());
            EditorStyles.label.normal.textColor = prevCol;
        }
        public static void LabelAtCenter(string str, FontStyle fontStyle = FontStyle.Normal)
            => RichLabel(str, TextAnchor.MiddleCenter, true, false, Color.black, fontStyle);

        public static bool ObjectField<T>(string title, ref T value, ref bool changed, 
            float width = -1, bool allowSceneObjects = true)
            where T : UnityEngine.Object
        {
            UnityEngine.Object newValue;
            if (width < 0)
                newValue = EditorGUILayout.ObjectField(title, value, typeof(T), allowSceneObjects);
            else
                newValue = EditorGUILayout.ObjectField(title, value, typeof(T), allowSceneObjects, GUILayout.Width(width));
            if (newValue == value)
                return false;
            changed = true;
            value = (T)newValue;
            return true;
        }
        public static bool GameObjectField(string title, ref GameObject value, ref bool changed, float width = -1, bool allowSceneObjects = true)
            => ObjectField(title, ref value, ref changed, width, allowSceneObjects);
        public static bool ObjectField<TItem, TObject>(string title,
            IEnumerable<TItem> items, Func<TItem, TObject> get, Action<TItem, TObject> set,
            ref bool changed, float width = -1, bool allowSceneObjects = true)
            where TObject : UnityEngine.Object
        {
            TObject obj = null;
            var exists = false;
            var same = true;
            foreach (var item in items)
            {
                var currObj = get(item);
                if (exists && currObj != obj)
                {
                    same = false;
                    obj = null;
                    break;
                }
                obj = currObj;
                exists = true;
            }
            EditorGUI.showMixedValue = !same;
            var edited = ObjectField(title, ref obj, ref changed, width, allowSceneObjects);
            EditorGUI.showMixedValue = false;
            if (edited)
                items.ForEach(item => set(item, obj));
            return edited;
        }
        public static bool FolderField(string title, ref UnityEngine.Object value, ref bool changed, float width = -1)
        {
            UnityEngine.Object newValue;
            if (width < 0)
                newValue = EditorGUILayout.ObjectField(title, value, typeof(DefaultAsset), false);
            else
                newValue = EditorGUILayout.ObjectField(title, value, typeof(DefaultAsset), false, GUILayout.Width(width));
            if (newValue != null)
            {
                var path = AssetDatabase.GetAssetPath(newValue);
                if (!Directory.Exists(path))
                    newValue = null;
            }
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
        public static bool ColorField<T>(string title, IEnumerable<T> items, Func<T, Color> get, Action<T, Color> set, 
            ref bool changed, float width = -1)
        {
            var col = Color.black;
            var exists = false;
            var same = true;
            foreach (var item in items)
            {
                var currCol = get(item);
                if (exists && currCol != col)
                {
                    same = false;
                    break;
                }
                col = currCol;
                exists = true;
            }
            EditorGUI.showMixedValue = !same;
            var edited = ColorField(title, ref col, ref changed, width);
            EditorGUI.showMixedValue = false;
            if (edited)
                items.ForEach(i => set(i, col));
            return edited;
        }
        public static bool ColorField(string title, ref Color value, ref bool changed, float width = -1)
        {
            Color newValue;
            if (width < 0)
                newValue = EditorGUILayout.ColorField(title, value);
            else
                newValue = EditorGUILayout.ColorField(title, value, GUILayout.Width(width));
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
        public static void TextFieldReadOnly(string title, string value, float width = -1, float height = -1, int labelWidth = -1)
        {
            PushDisabling();
            var changed = false;
            TextField(title, ref value, ref changed, width, height, labelWidth);
            PopEnabling();
        }
        public static bool TextField(string title, ref string value, ref bool changed, float width = -1, float height = -1, int labelWidth = -1)
        {
            string newValue = null;
            var val = value;
            WithLabelWidth(() =>
            {
                if (width < 0 && height < 0)
                    newValue = EditorGUILayout.TextField(title, val);
                else if (width >= 0 && height < 0)
                    newValue = EditorGUILayout.TextField(title, val, GUILayout.Width(width));
                else if (width < 0 && height >= 0)
                    newValue = EditorGUILayout.TextField(title, val, GUILayout.Height(height));
                else
                    newValue = EditorGUILayout.TextField(title, val, GUILayout.Width(width), GUILayout.Height(height));
            }, labelWidth);
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
        public static bool TextArea(string title, ref string value, ref bool changed, float width = -1, float height = -1)
        {
            string newValue;
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(title))
                GUILayout.Label(title, GUILayout.Width(200));
            if (width < 0 && height < 0)
                newValue = EditorGUILayout.TextArea(value);
            else if (width >= 0 && height < 0)
                newValue = EditorGUILayout.TextArea(value, GUILayout.Width(width));
            else if (width < 0 && height >= 0)
                newValue = EditorGUILayout.TextArea(value, GUILayout.Height(height));
            else
                newValue = EditorGUILayout.TextArea(value, GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.EndHorizontal();
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }

        public static bool FloatField(string title, ref float value, ref bool changed, float width = -1)
        {
            float newValue;
            if (width < 0)
                newValue = EditorGUILayout.FloatField(title, value);
            else
                newValue = EditorGUILayout.FloatField(title, value, GUILayout.Width(width));
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
		
        public static bool PrefabField<T>(string title, ref T value, ref bool changed, float width = -1)
            where T : UnityEngine.Object
        {
            T newValue;
            if (width < 0)
                newValue = EditorGUILayout.ObjectField(title, value, typeof(T), false) as T;
            else
                newValue = EditorGUILayout.ObjectField(title, value, typeof(T), false, GUILayout.Width(width)) as T;
			if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
        public static bool SpriteField(string title, ref Sprite value, ref bool changed)
        {
            var newValue = EditorGUILayout.ObjectField(title, value, typeof(Sprite), false) as Sprite;
            if (newValue != value)
            {
                changed = true;
                value = newValue;
                return true;
            }
            return false;
        }
        public static bool FontField(string title, ref TMP_FontAsset value, ref bool changed)
        {
            var newValue = EditorGUILayout.ObjectField(title, value, typeof(TMP_FontAsset), false) as TMP_FontAsset;
            if (newValue != value)
            {
                changed = true;
                value = newValue;
                return true;
            }
            return false;
        }
        public static bool LayerField(string title, ref int value, ref bool changed, float width = -1)
        {
            int newValue;
            if (width < 0)
                newValue = EditorGUILayout.LayerField(title, value);
            else
                newValue = EditorGUILayout.LayerField(title, value, GUILayout.Width(width));
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }
        public static bool IntField(string title, ref int value, ref bool changed, float width = -1, int labelWidth = -1)
        {
            var value1 = value;
            int newValue = -1;
            WithLabelWidth(() =>
            {
                if (width < 0)
                    newValue = EditorGUILayout.IntField(title, value1);
                else
                    newValue = EditorGUILayout.IntField(title, value1, GUILayout.Width(width));
            }, labelWidth);
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }

        private static void WithLabelWidth(Action action, float width = -1)
        {
            var originalValue = EditorGUIUtility.labelWidth;
            if (width >= 0)
                EditorGUIUtility.labelWidth = width;
            action?.Invoke();
            EditorGUIUtility.labelWidth = originalValue;
        }
        public static bool Toggle(string title, ref bool value, ref bool changed, int width = -1, int labelWidth = -1)
        {
            var newValue = false;
            var val = value;
            WithLabelWidth(() =>
            {
                if (width == -1)
                    newValue = EditorGUILayout.Toggle(title, val);
                else
                    newValue = EditorGUILayout.Toggle(title, val, GUILayout.Width(width));
            }, labelWidth);
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }

        public static bool ToggleMadeFromButton(string enableDisabled, string disableEnabled, ref bool value, ref bool changed)
        {
            var title = value ? disableEnabled : enableDisabled;
            if (GUILayout.Button(title))
            {
                value = !value;
                changed = true;
                return true;
            }
            return false;
        }
        public static bool ToggleMadeFromToolbar(string enabled, string disabled, ref bool value, ref bool changed, int width = -1)
        {
            int newValue, oldValue = value?1:0;
            if (width == -1)
                newValue = GUILayout.Toolbar(oldValue, new string[] { disabled, enabled });
            else
                newValue = GUILayout.Toolbar(oldValue, new string[] { disabled, enabled }, GUILayout.Width(width));
            if (newValue == oldValue)
                return false;
            changed = true;
            value = newValue == 1;
            return true;
        }
        public static bool XButton() => GUILayout.Button("X", GUILayout.Width(18));

        public static bool Toolbar<T>(string title, ref T value, ref bool changed, float width = -1) where T : Enum
        {
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(title))
                GUILayout.Label(title);
            var newVal = GUIUtils.Toolbar(value, width);
            GUILayout.EndHorizontal();
            if (newVal.CompareTo(value) == 0)
                return false;
            changed = true;
            value = newVal;
            return true;
        }

        public static bool Toolbar(string title, ref string value, string[] options, ref bool changed, float width = -1)
        {
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(title))
                GUILayout.Label(title);
            var newVal = GUIUtils.Toolbar(value, options, width);
            GUILayout.EndHorizontal();
            if (newVal.CompareTo(value) == 0)
                return false;
            changed = true;
            value = newVal;
            return true;
        }

        public static T Popup<T>(T val, float width = -1) where T : Enum
        {
            var (vals, names, oldInd) = GUIUtils.GetOptionsData(val);
            int newInd;
            if (width < 0)
                newInd = EditorGUILayout.Popup(oldInd, names);
            else
                newInd = EditorGUILayout.Popup(oldInd, names, GUILayout.Width(width));
            return (T)vals[newInd];
        }

        public static bool Popup<T>(string title, ref T value, ref bool changed, float width = -1) where T : Enum
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            var newVal = Popup(value, width);
            GUILayout.EndHorizontal();
            if (newVal.CompareTo(value) == 0)
                return false;
            changed = true;
            value = newVal;
            return true;
        }

        public static bool Popup(ref int value, string[] options, ref bool changed, int width = -1)
        {
            int newValue;
            if (width <= 0)
                newValue = EditorGUILayout.Popup(value, options);
            else
                newValue = EditorGUILayout.Popup(value, options, GUILayout.Width(width));
            if (newValue == value)
                return false;
            changed = true;
            value = newValue;
            return true;
        }

        public static void RichMultilineLabel(string str, int width = -1)
        {
            EditorStyles.label.richText = true;
            EditorStyles.label.wordWrap = true;
            if (width < 0)
                EditorGUILayout.LabelField(str);
            else
                EditorGUILayout.LabelField(str, GUILayout.Width(width));
            EditorStyles.label.wordWrap = false;
            EditorStyles.label.richText = false;
        }

        public static void LabelToCopy(string str)
        {
            GUILayout.BeginHorizontal();
            RichMultilineLabel(str);
            if (GUILayout.Button("copy", GUILayout.Width(50)))
            {
                str.CopyToClipboard();
                Debug.Log($"'{str}' copied!");
            }
            GUILayout.EndHorizontal();
        }
        
        private static Stack<bool> guiEnabled;
        static void UpdateGUIEnabled()
        {
            var enabled = true;
            foreach (var val in guiEnabled)
            {
                if (!val)
                {
                    enabled = false;
                    break;
                }
            }
            GUI.enabled = enabled;
        }
        public static void PushDisabling() => PushEnabling(false);
        public static void PushEnabling(bool enabled = true)
        {
            if (guiEnabled == null)
            {
                guiEnabled = new Stack<bool>();
                guiEnabled.Push(true);
            }
            guiEnabled.Push(enabled);
            UpdateGUIEnabled();
        }
        public static void PopEnabling()
        {
            guiEnabled.Pop();
            UpdateGUIEnabled();
        }
        static GUIStyle boxSkin = GUIStyle.none;// new GUIStyle(GUI.skin.box);
        static Texture2D good, bad, open, close;
        static void InitPicsIfNeeded()
        {
            if (good != null)
                return;
            const int size = 5;
            boxSkin.margin = new RectOffset(size, size, size, size);
            var folder = $"{FriendsGamesManager.MainPluginFolder}/EditorTools/Editor/EditorUIPics";
            good = AssetDatabase.LoadAssetAtPath<Texture2D>($"{folder}/good.png");
            bad = AssetDatabase.LoadAssetAtPath<Texture2D>($"{folder}/bad.png");
            open = AssetDatabase.LoadAssetAtPath<Texture2D>($"{folder}/open.png");
            close = AssetDatabase.LoadAssetAtPath<Texture2D>($"{folder}/close.png");
        }
        private static void ShowValidBoxTexture(Texture2D texture)
        {
            InitPicsIfNeeded();
            const float size = 12;
            GUILayout.Box(texture, boxSkin, GUILayout.Width(size), GUILayout.Height(size));
        }
        public static void ShowValid(string title, bool valid)
        {
            GUILayout.BeginHorizontal();
            ShowValid(valid);
            GUILayout.Label(title);
            GUILayout.EndHorizontal();
        }
        public static void ShowValid(bool valid)
            => ShowValidBoxTexture(valid ? good : bad);
        public static void ShowValidEmpty()
            => ShowValidBoxTexture(null);
        public static bool ShowOpenClose(ref bool opened)
        {
            InitPicsIfNeeded();
            const float size = 12;
            if (!GUILayout.Button(opened ? close : open, boxSkin, GUILayout.Width(size), GUILayout.Height(size)))
                return false;
            opened = !opened;
            return true;
        }
        public static bool ShowOpenClose(ref bool opened, string title)
        {
            if (string.IsNullOrEmpty(title))
                return ShowOpenClose(ref opened);
            else
            {
                GUILayout.BeginHorizontal();
                var changed = ShowOpenClose(ref opened);
                GUILayout.Label(title);
                GUILayout.EndHorizontal();
                return changed;
            }
        }
        public static void Indent(int indents = 1)
        {
            const float size = 12;
            GUILayout.Box(default(Texture2D), boxSkin, GUILayout.Width(size  * indents), GUILayout.Height(size));
        }
        public static void List<T>(string title, List<T> list, bool allowSceneObjects, ref bool changed) where T: UnityEngine.Object
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("size", list.Count));
            if (newCount != list.Count)
                changed = true;
            GUILayout.EndHorizontal();
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);
            for (int i = 0; i < list.Count; i++)
            {
                var newItem  = EditorGUILayout.ObjectField(list[i], typeof(T), allowSceneObjects) as T;
                if (list[i] == newItem)
                    continue;
                list[i] = newItem;
                changed = true;
            }
        }
    }
} 
