#if EDITOR_TOOLS
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FriendsGamesTools.EditorTools
{
    public static class AssetsIterator
    {
#if UNITY_EDITOR
        public static string InvariantSeparator(string path)
        {
            path = path.Replace('\\', '/');
            return path;
        }
        public static void IterateAssetsFolders(Action<string> iterateFolder)
        {
            IterateWithSubfolders(Application.dataPath + "/", iterateFolder);
        }
        public static void IterateWithSubfolders(string path, Action<string> iterateFolder, int maxDepth = -1)
        {
            iterateFolder(path);
            IterateSubfolders(path, iterateFolder, maxDepth);
        }
        private static void IterateSubfolders(string path, Action<string> iterateFolder, int maxDepth = -1)
        {
            if (maxDepth > 0)
                maxDepth--;
            if (!Directory.Exists(path))
                return; // Dir was moved, deleted, renamed etc.
            if (maxDepth == 0)
                return; // Max depth exceeded.
            var subDirs = Directory.GetDirectories(path).ConvertAll(subDir => subDir.Replace("\\", "/"));
            foreach (string currDirectory in subDirs)
                IterateWithSubfolders(currDirectory, iterateFolder, maxDepth);
        }
        public static void IterateAssetsInFolderRecursively(string path, Action<string> iterateAsset, int maxDepth = -1)
        {
            IterateWithSubfolders(path, (subfolderPath)=> {
                IterateAssetsInFolder(subfolderPath, iterateAsset);
            }, maxDepth);
        }
        public static void IterateAllAssetsRecursively(Action<string> iterateAsset, int maxDepth = -1)
            => IterateAssetsInFolderRecursively(FriendsGamesManager.AssetsFolder, iterateAsset, maxDepth);
        public static void IterateAllGameObjectsInProject(Action<string, string, GameObject> action) {
            IterateAllAssetsRecursively((path) =>
            {
                if (path.EndsWith(".unity") || path.EndsWith(".gitattributes") || path.EndsWith(".gitignore")
                    || path.EndsWith(".npmignore") || path.EndsWith(".editorconfig"))
                    return;
                var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
                assetsAtPath.ForEach(asset=> {
                    GameObject go = asset as GameObject;
                    if (go == null)
                        return;
                    action(path, go.transform.FullName(), go);
                });
            });
            IterateAllSceneGameObjectsInProject(action);
        }
        public static void IterateAllSceneGameObjectsInProject(Action<string, string, GameObject> action)
            => IterateAllScenesInProject(scene
                => IterateSceneObjects(scene, (hierarchyPath, go)
                    => action(scene.path, hierarchyPath, go)));
        public static void IterateAllScenesInProject(Action<Scene> action)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
                action(SceneManager.GetSceneAt(i));
        }
        public static void IterateSceneObjects(Scene scene, Action<string, GameObject> action)
        {
            var roots = scene.GetRootGameObjects();
            roots.ForEach(root=>root.transform.IterateChildren(child=>action(child.FullName(), child.gameObject)));
        }
        public static void IterateAllPrefabsInProject(Action<string, GameObject> action)
        {
            IterateAllAssetsRecursively((path) =>
            {
                if (!path.EndsWith(".prefab"))
                    return;
                var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
                assetsAtPath.ForEach(asset => {
                    GameObject go = asset as GameObject;
                    if (go != null)
                        action(path, go);
                });
            });
        }
        public class MemberReference
        {
            public Component referencer;
            public Type referencerType;
            public object value;
        }
        public class FieldReference : MemberReference
        {
            public FieldInfo field;
            public int indInList = -1;
        }
        public class PropertyReference : MemberReference
        {
            public PropertyInfo prop;
        }
        public static void IterateCurrSceneReferencesOf(GameObject searchedGO, Action<FieldReference> iterator)
        {
            IterateOpenedScene(go => {
                MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
                for (int i = 0; i < components.Length; i++)
                {
                    MonoBehaviour currComp = components[i];
                    if (currComp != null)
                    {
                        var type = currComp.GetType();
                        FindRefsOfSearchedObjRecursively(currComp, type, searchedGO, iterator);
                    }
                }
            });
        }
        private static void FindRefsOfSearchedObjRecursively(MonoBehaviour iteratedMonobeh, Type type, GameObject searchedObj, Action<FieldReference> iterator)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo currField = fields[i];
                object fieldValue = currField.GetValue(iteratedMonobeh);
                var fieldValueMonobeh = fieldValue as MonoBehaviour;
                if (fieldValueMonobeh != null)
                {
                    if (searchedObj.Equals(fieldValueMonobeh.gameObject))
                        iterator?.Invoke(new FieldReference { field = currField, referencer = iteratedMonobeh, referencerType = type, value = currField.GetValue(iteratedMonobeh) });
                }
                IList fieldValList = fieldValue as IList;
                if (fieldValList != null)
                {
                    int ind = 0;
                    foreach (var item in fieldValList)
                    {
                        if (searchedObj.Equals(item) || ((item is Component) && ((item as Component).gameObject == searchedObj)))
                            iterator?.Invoke(new FieldReference { field = currField, referencer = iteratedMonobeh, referencerType = type, value = item, indInList = ind });
                        ind++;
                    }
                }
            }
            // Iterate base classes.
            if (type == typeof(MonoBehaviour) || type == typeof(object))
                return; // Higher in hierarcy can not contain serializable resources.
            FindRefsOfSearchedObjRecursively(iteratedMonobeh, type.BaseType, searchedObj, iterator);
        }
        public static void IterateFieldsWithBaseClassRecursively(Component iteratedMonobeh, Type type, Action<FieldReference> iterator)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo currField = fields[i];
                iterator?.Invoke(new FieldReference { field = currField, referencer = iteratedMonobeh, referencerType = type, value = currField.GetValue(iteratedMonobeh) });
            }
            // Iterate base classes.
            if (type == typeof(Component) || type == typeof(object))
                return; // Higher in hierarcy can not contain serializable resources.
            IterateFieldsWithBaseClassRecursively(iteratedMonobeh, type.BaseType, iterator);
        }
        public static void IterateGameObjectRecursively(GameObject go, Action<string, GameObject> action, string currSubPath = "root")
        {
            if (go == null)
                return;
            action(currSubPath, go);
            for (int i=0;i<go.transform.childCount;i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                var childPath = $"{currSubPath}.{child.name}";
                IterateGameObjectRecursively(child, action, childPath);
            }
        }
        public static void IterateAssetsInFolder(string folderPath, Action<string> iterateAsset)
        {
            string[] aFilePaths = Directory.GetFiles(folderPath);
            foreach (string filePath in aFilePaths)
            {
                if (filePath.EndsWith(".meta"))
                    continue;
                iterateAsset?.Invoke(filePath.Replace("\\", "/"));
            }
        }
#endif
        public static bool IterateOpenedScene(Action<GameObject> action)
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded) return false;
            var roots = scene.GetRootGameObjects();
            roots.ForEach(r => r.IterateChildren(action));
            return true;
        }
    }
}
#endif