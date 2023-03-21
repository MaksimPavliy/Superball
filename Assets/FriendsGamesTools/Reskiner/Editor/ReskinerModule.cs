using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Reskiner
{
    public class ReskinerModule : RootModule
    {
        public const string define = "RESKINER";
        public override string Define => define;
        public override HowToModule HowTo() => new ReskinerModule_HowTo();

#if RESKINER
        Reskiner reskiner => SettingsInEditor<Reskiner>.GetSettingsInstance(false);
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            objToExplainUsage = EditorGUILayout.ObjectField(objToExplainUsage, typeof(Object), false);
            if (GUILayout.Button("find where used"))
                Debug.Log(CalcAssetUsageExplanation(AssetDatabase.GetAssetPath(objToExplainUsage)));


            if (reskiner == null)
            {
                if (GUILayout.Button("Start reskinning"))
                    SettingsInEditor<Reskiner>.EnsureExists();
                return;
            }

            // Reskiner exists.
            // 1. update assets to reskin.
            // 2. show assets to reskin.
            // 3. ignore list.
            // 4. gather assets in one folder.
            // 5. start/restart reskin. each model has state - old, new. Show reskin progress.
            // 6. rescale models.

            if (GUILayout.Button("Update assets to reskin"))
                UpdateAssetsToReskin();

            if (reskiner.objs.Count > 0)
            {
                var reskinnedCount = reskiner.objs.Count(obj => obj.state != ObjectToReskin.State.Original);
                GUILayout.Label($"Reskin progress is {100 * reskinnedCount / reskiner.objs.Count}% ({reskinnedCount}/{reskiner.objs.Count})");
                reskiner.showObjsList = GUILayout.Toggle(reskiner.showObjsList, "show objects list");

                if (GUILayout.Button("Update progress"))
                    UpdateChangedAssets();

                if (reskiner.showObjsList)
                {
                    foreach (var obj in reskiner.objs)
                    {
                        GUILayout.BeginHorizontal();
                        using (new ChangedBackCol(obj.state == ObjectToReskin.State.Original ? new Color(0.8f, 0.5f, 0.5f, 1) :
                            (obj.state == ObjectToReskin.State.WontChange ? new Color(0.5f, 0.8f, 0.5f, 1) : new Color(0, 1, 0, 1))))
                        {
                            EditorGUIUtils.PushDisabling();
                            EditorGUILayout.ObjectField(obj.obj, typeof(Object), false);
                            EditorGUILayout.EnumPopup(obj.state, GUILayout.Width(100));
                            EditorGUIUtils.PopEnabling();
                        }
                        if (obj.state == ObjectToReskin.State.Original)
                        {
                            if (GUILayout.Button(new GUIContent("mark ok",
                                "Press if this asset is ok to be left the same in reskinned game"), GUILayout.Width(100)))
                                obj.state = ObjectToReskin.State.WontChange;
                        }
                        else
                        {
                            if (GUILayout.Button(new GUIContent("reset",
                                "Press if this asset needs to be changed in reskin"), GUILayout.Width(100)))
                                ResetObj(obj);
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                if (GUILayout.Button("move to one folder"))
                    GatherAssetsInOneFolder();

                var modelsCount = reskiner.objs.Count(obj => IsModel(obj.obj));
                if (GUILayout.Button($"rescale {modelsCount} models"))
                    RescaleModels();
            }
        }

        public Object objToExplainUsage;
        static List<string> CalcRootPathesInBuild(string[] allAssetsGUIDs = null)
        {
            List<string> rootPathes = new List<string>();
            foreach (var sc in EditorBuildSettings.scenes)
            {
                if (sc.enabled && !rootPathes.Contains(sc.path))
                    rootPathes.Add(sc.path);
            }
            if (allAssetsGUIDs == null)
                allAssetsGUIDs = AssetDatabase.FindAssets("");
            foreach (var guid in allAssetsGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!InResources(path))
                    continue;
                if (!rootPathes.Contains(path))
                    rootPathes.Add(path);
            }
            return rootPathes;
        }
        public static bool InResources(string path) => path.Contains("/Resources/");
        static List<string> otherMediaFileExtentions = new List<string> {
                    ".png", ".jpg", ".jpeg", ".ogg", ".mp3", ".wav"
                };
        static List<string> modelsFileExtentions = new List<string> {
                    ".fbx", ".obj"
                };
        static bool HasExtention(string path, List<string> extensions) => extensions.Any(ext => path.EndsWith(ext, true, null));
        static bool IsModel(Object obj) => HasExtention(AssetDatabase.GetAssetPath(obj), modelsFileExtentions);
        static bool IsMediaFilePath(string path) => HasExtention(path, otherMediaFileExtentions) || HasExtention(path, modelsFileExtentions);
        static void FilterOnlyDistinctMediaPathes(List<string> assetPathes)
        {
            assetPathes.RemoveAll(path => !IsMediaFilePath(path));
            MakeDistinct(assetPathes);
        }
        static void MakeDistinct(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list.LastIndexOf(list[i]) != i)
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }
        public static string CalcAssetUsageExplanation(string assetPath)
        {
            var rootPathes = CalcRootPathesInBuild();
            MakeDistinct(rootPathes);
            var dependancies = AssetDatabase.GetDependencies(rootPathes.ToArray(), true);
            if (!dependancies.Contains(assetPath))
                return $"asset {assetPath} is not used";
            return CalcAssetUsageExplanationRecursive(assetPath, "", rootPathes);
        }
        static string CalcAssetUsageExplanationRecursive(string assetPath, string parentExplanation, List<string> pathes)
        {
            if (pathes.Count == 0)
                return null;
            var dependancies = AssetDatabase.GetDependencies(pathes.ToArray(), true);
            if (!dependancies.Contains(assetPath))
                return null;
            if (pathes.Count > 1)
            {
                var explanationByFirstHalf = CalcAssetUsageExplanationRecursive(assetPath, parentExplanation, pathes.GetRange(0, pathes.Count / 2));
                if (explanationByFirstHalf != null)
                    return explanationByFirstHalf;
                var explanationBySecondHalf = CalcAssetUsageExplanationRecursive(assetPath, parentExplanation, pathes.GetRange(pathes.Count / 2, pathes.Count - pathes.Count / 2));
                return explanationBySecondHalf;
            }
            else
            {
                var directDependancies = AssetDatabase.GetDependencies(pathes[0], false).ToList();
                if (directDependancies.Contains(assetPath))
                    return $"{parentExplanation}->{assetPath}";
                else
                    return CalcAssetUsageExplanationRecursive(assetPath, $"{parentExplanation}->{pathes[0]}", directDependancies);
            }
        }

        void UpdateAssetsToReskin()
        {
            EditorUtility.DisplayProgressBar("Updating assets to reskin", "", 0);
            var allAssetGuids = AssetDatabase.FindAssets("");
            var reskinnableAssets = allAssetGuids.ToList().ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            FilterOnlyDistinctMediaPathes(reskinnableAssets);
            var assetsInBuild = AssetDatabase.GetDependencies(CalcRootPathesInBuild(allAssetGuids).ToArray(), true).ToList();
            //Debug.Log("assetsInBuild count ===== " + assetsInBuild.Count.ToString());
            FilterOnlyDistinctMediaPathes(assetsInBuild);
            //  Debug.Log($"distinct media asstes in build count = {assetsInBuild.Count}");
            int allMediaAssetsCount = reskinnableAssets.Count;
            reskinnableAssets.RemoveAll(path => !assetsInBuild.Contains(path));
            int mediaAssetsInBuild = reskinnableAssets.Count;
            reskiner.objs.Clear();
            var notUsedMediaAssetsCount = allMediaAssetsCount - mediaAssetsInBuild;
            Debug.Log($"used media assets = {mediaAssetsInBuild}, also there are {notUsedMediaAssetsCount} not used media assets in project");
            var sb = new StringBuilder();
            for (int i = 0; i < reskinnableAssets.Count; i++)
            {
                var assetPath = reskinnableAssets[i];
                sb.AppendLine(assetPath);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                reskiner.objs.Add(new ObjectToReskin { obj = obj });
            }
            Debug.Log(sb);
            ResetObjects();
            EditorUtility.SetDirty(reskiner);
            EditorUtility.ClearProgressBar();
        }

        void GatherAssetsInOneFolder()
        {
            using (new EditorProgressbar("Moving files to one folder"))
            {
                foreach (var item in reskiner.objs)
                {
                    var oldPath = AssetDatabase.GetAssetPath(item.obj);
                    var newFolder = InResources(oldPath) ? reskiner.AssetsToReskinFolderResources : reskiner.AssetsToReskinFolder;
                    var newPath = $"{newFolder}/{Path.GetFileName(oldPath)}";
                    Directory.CreateDirectory(newFolder);
                    AssetDatabase.MoveAsset(oldPath, newPath);
                }
                AssetDatabase.SaveAssets();
            }
        }

        long CalcFileHash(Object obj)
        {
            var fileBytes = File.ReadAllBytes(AssetDatabase.GetAssetPath(obj));
            long hash = 825976838;
            foreach (var b in fileBytes)
            {
                hash += hash << 11; hash ^= hash >> 7;
                hash += b;
            }
            return hash;
        }
        void UpdateChangedAssets()
        {
            foreach (var item in reskiner.objs)
            {
                if (item.state == ObjectToReskin.State.WontChange)
                    continue;
                var currHash = CalcFileHash(item.obj);
                item.state = currHash == item.originalHash ? ObjectToReskin.State.Original : ObjectToReskin.State.Reskinned;
            }
        }
        void ResetObjects()
        {
            foreach (var item in reskiner.objs)
                ResetObj(item);
        }
        void ResetObj(ObjectToReskin item)
        {
            item.state = ObjectToReskin.State.Original;
            item.originalHash = CalcFileHash(item.obj);
            item.modelScale = CalcModelScale(item);
        }
        float CalcModelScale(ObjectToReskin item)
        {
            if (IsModel(item.obj))
            {
                GameObject go = item.obj as GameObject;
                var bounds = new Bounds(go.transform.position, Vector3.zero);
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                    bounds.Encapsulate(renderer.bounds);
                return Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                //Debug.Log($"size = {item.modelScale} for {AssetDatabase.GetAssetPath(item.obj)}");
            }
            else
                return 0;
        }
        void RescaleModels()
        {
            using (new EditorProgressbar("Rescaling models"))
            {
                foreach (var item in reskiner.objs)
                {
                    if (!IsModel(item.obj))
                        continue;
                    var neededScale = item.modelScale;
                    var currScale = CalcModelScale(item);
                    var scaleMul = neededScale / currScale;
                    if (Mathf.Abs(scaleMul - 1) < 0.01f)
                        continue;
                    var mi = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(item.obj)) as ModelImporter;
                    mi.globalScale *= scaleMul;
                    mi.SaveAndReimport();
                }
            }
        }
#endif
    }
}


