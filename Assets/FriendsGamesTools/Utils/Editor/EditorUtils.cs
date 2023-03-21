using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class EditorUtils
    {
        public static void DeleteFolder(string folderPath)
        {
            var subDirs = Directory.GetDirectories(folderPath);
            foreach (var path in subDirs)
                DeleteFolder(path);
            var filePathes = Directory.GetFiles(folderPath);
            foreach (var path in filePathes)
                File.Delete(path);
            Directory.Delete(folderPath);
        } 
        public static T CreateScriptableInstanceAtPath<T>(string path) where T : ScriptableObject
        {
            var instance = ScriptableObject.CreateInstance<T>();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            return instance;
        }
        public static T[] GetAllInstancesInProject<T>() where T : UnityEngine.Object
        {
            var name = typeof(T).Name;
            string[] guids = AssetDatabase.FindAssets("t:" + name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return a;
        }
        public static T GetInstanceInProject<T>() where T : UnityEngine.Object
        {
            var instances = GetAllInstancesInProject<T>();
            if (instances.Length >= 1)
                return instances[0];
            return null;
        }
        public static T GetPrivateField<T>(UnityEngine.Object obj, string name)
        {
            var value = ReflectionUtils.GetFieldInfo(obj, name).GetValue(obj);
            return (T)value;
        }
        public static void SetPrivateField<T>(UnityEngine.Object obj, string name, T value)
        {
            var field = ReflectionUtils.GetFieldInfo(obj, name);
            field.SetValue(obj, value);
        }
        public static void CallPrivateMethod(UnityEngine.Object obj, string name, params object[] methodParams)
            => ReflectionUtils.CallMethod(obj, name, methodParams);
        public static void SetDirty(UnityEngine.Object obj) => EditorUtility.SetDirty(obj);

        public static async Task ImportPackage(string packagePath) {
            var completed = false;
            AssetDatabase.ImportPackageCallback onComplete = _ => completed = true;
            AssetDatabase.importPackageCompleted += onComplete;
            AssetDatabase.ImportPackage(packagePath, false);
            await Awaiters.While(() => !completed);
            AssetDatabase.importPackageCompleted -= onComplete;
        }

        public static void CorrectCode(string badCode, string goodCode, string filePath, bool reimport = true) {
            var maxScript = File.ReadAllText(filePath).ToLf();
            if (maxScript.Contains(goodCode)) return;
            if (!maxScript.Contains(badCode))
                throw new Exception($"{badCode} line not found in {filePath}");
            maxScript = maxScript.ReplaceLineWith(badCode, goodCode).ToCrLf();
            File.WriteAllText(filePath, maxScript);
            if (reimport)
                AssetDatabase.Refresh();
        }
    }
}
