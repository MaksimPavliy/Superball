using System.IO;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class SettingsInEditor<T> where T : SettingsScriptable<T>
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetSettingsInstance();
                return _instance;
            }
        }
        public static void EnsureExists() => GetSettingsInstance();
        public static T GetSettingsInstance(bool createIfNotExists = true)
        {
            var instance = EditorUtils.GetInstanceInProject<T>();
            if (instance != null)
                Debug.Assert(AssetDatabase.GetAssetPath(instance) == instance.AssetPath, "Wrong path");
            else if (createIfNotExists)
            {
                instance = ScriptableObject.CreateInstance<T>();
                Directory.CreateDirectory(Path.GetDirectoryName(instance.AssetPath));
                AssetDatabase.CreateAsset(instance, instance.AssetPath);
                AssetDatabase.SaveAssets();
            }
            return instance;
        }
    }

    public class ScriptableObjCreatorContext : MonoBehaviour
    {
        const string ContextPath = "CONTEXT/MonoScript/Create ScriptableObject";
        [MenuItem(ContextPath, true)]
        public static bool ValidateCreateScriptableFromScript(MenuCommand command)
        {
            var script = (MonoScript)command.context;
            var type = ReflectionUtils.GetTypeByName(script.name, true);
            return typeof(ScriptableObject).IsAssignableFrom(type);
        }
        [MenuItem(ContextPath)]
        public static void CreateScriptableFromScript(MenuCommand command)
        {
            var script = (MonoScript)command.context;
            var type = ReflectionUtils.GetTypeByName(script.name, true);
            try
            {
                var TSettingsScriptable = typeof(SettingsScriptable<>).MakeGenericType(type);
                if (TSettingsScriptable.IsAssignableFrom(type))
                {
                    var TSettingsInEditor = typeof(SettingsInEditor<>).MakeGenericType(type);
                    TSettingsInEditor.CallStaticMethod("EnsureExists");
                    return;
                }
            }
            catch { }
            var instance = ScriptableObject.CreateInstance(type);
            var scriptPath = AssetDatabase.GetAssetPath(script);
            var folder = Path.GetDirectoryName(scriptPath);
            Directory.CreateDirectory(folder);
            var path = $"{folder}/{script.name}.asset";
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = instance;
        }
    }
}

//obj = EditorGUILayout.ObjectField("ignore folder", obj, typeof(Object), false);
//if (obj != null)
//GUILayout.Label(obj.GetType().Name);
