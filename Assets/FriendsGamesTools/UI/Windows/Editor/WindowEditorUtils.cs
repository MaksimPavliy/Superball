#if WINDOWS
using FriendsGamesTools.UI;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.UI
{
    public static class WindowEditorUtils
    {
        public static void ShowWindow<T, TSettings>(this TSettings window, ref bool changed, bool defaultIsValid = false)
            where T : Window where TSettings : WindowPrefabSettings<T>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtils.ShowValid(defaultIsValid ? true : WindowValid(window));
            EditorGUIUtils.PrefabField($"{window.title}", ref window.prefab, ref changed);
            if (window.prefab == null)
            {
                window.prefab = AssetDatabase.LoadAssetAtPath<T>(window.defaultPath);
                changed = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        public static bool WindowValid<T>(WindowPrefabSettings<T> window, StringBuilder sb = null) where T : Window
        {
            if (IsDefaultWindow(window))
            {
                sb?.AppendLine($"'{window.title}' window is using default gray window");
                return false;
            }
            return true;
        }
        public static bool IsDefaultWindow<T>(this WindowPrefabSettings<T> window) where T : Window
            => AssetDatabase.GetAssetPath(window.prefab) == window.defaultPath;
    }
}
#endif