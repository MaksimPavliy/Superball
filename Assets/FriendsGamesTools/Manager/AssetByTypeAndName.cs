using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace FriendsGamesTools
{
    public enum TypeFilterName
    {
        Prefab, MonoScript, Folder, Material, TMP_FontAsset, Font
    }
    public static class AssetByTypeAndName 
    {
        public static T Find<T>(string fileName, TypeFilterName type, bool okIfNotFound) where T : Object
        {
#if UNITY_EDITOR
            var pathes = AssetDatabase.FindAssets($"t:{type} {fileName}")
                .ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Filter(path => Path.GetFileNameWithoutExtension(path) == fileName && path.StartsWith(FriendsGamesManager.AssetsFolder));
            if (pathes.Count == 0 && okIfNotFound)
                return null;
            else
            {
                Debug.Assert(pathes.Count == 1, $"Should be single {fileName} " +
                    $"with type {type} found, but found {pathes.Count}\n" +
                    $"{pathes.PrintCollection("\n")}");
                var scriptPath = pathes[0];
                return AssetDatabase.LoadAssetAtPath<T>(scriptPath);
            }
#else
            return null;
#endif
        }        
    }
}