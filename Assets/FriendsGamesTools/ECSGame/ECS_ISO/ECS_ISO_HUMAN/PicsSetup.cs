#if ECS_ISO_HUMAN
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FriendsGamesTools.ECSGame.Iso
{
    public abstract class PicsSetup : MonoBehaviour
    {
        protected virtual void Awake() {
            replaceFolder = GetComponent<PicsSetupReplaceFolder>();
            if (replaceFolder != null)
                Debug.LogError("replaceFolder is obsolete, " + PicsReplaceObsoleteWarning);
            if (Application.isPlaying)
            {
                if (replaceFolder != null)
                    Destroy(replaceFolder);
                Destroy(this); // Doesnt exist in runtime.
                return;
            }
        }
        protected virtual string OnSpriteFolderFound(string spriteFolder) => spriteFolder;
        public const string PicsReplaceObsoleteWarning = "Use PicsSetup.OnSpriteFolderFound() instead";
        PicsSetupReplaceFolder replaceFolder;

#if UNITY_EDITOR
        public static string GetPathToFolderStatic(Sprite sprite)
        {
            var path = AssetDatabase.GetAssetPath(sprite);
            path = path.Remove(path.LastIndexOf("/"));
            return path;
        }
        public string GetPathToFolder(Sprite sprite)
        {
            var path = GetPathToFolderStatic(sprite);
            if (replaceFolder != null)
                path = ReplaceFolder(path, replaceFolder.originalFolder, replaceFolder.replacedFolder);
            path = OnSpriteFolderFound(path);
            return path;
        }
        public static List<Sprite> GetSpritesFromFolder(string folder)
        {
            var picGuids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
            var pics = new List<Sprite>();
            foreach (var id in picGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(id);
                pics.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
            }
            return pics;
        }
        protected virtual string GetShadowsFolder(string picFolder) => picFolder;
        public List<SpriteAndShadow> GetSpritesShadowsFromFolder(string folder)
        {
            bool IsShadow(Sprite pic) => pic.name.Contains("shadow");
            var pics = GetSpritesFromFolder(folder);
            if (pics.Count == 0)
                return new List<SpriteAndShadow>();
            List<Sprite> shadows;
            string shadowsFolder = GetShadowsFolder(folder);
            if (shadowsFolder == folder)
                shadows = pics.Filter(pic => IsShadow(pic));
            else
                shadows = GetSpritesFromFolder(shadowsFolder);
            pics.RemoveAll(pic => IsShadow(pic));
            // Check.
            Debug.Assert(pics.Count == shadows.Count, $"pics.Count={pics.Count}, " +
                $"but shadows.Count={shadows.Count}, folder={folder}");
            var res = new List<SpriteAndShadow>();
            for (int i = 0; i < pics.Count; i++)
                res.Add(new SpriteAndShadow { pic = pics[i], shadow = shadows[i] });
            return res;
        }
        public string ReplaceFolder(string path, string originalFolder, string replaceToFolder)
        {
            if (!string.IsNullOrEmpty(originalFolder) && !string.IsNullOrEmpty(replaceToFolder))
                path = ReplaceFolder(path,
                    f => f == originalFolder, f => replaceToFolder);
            return path;
        }
        public string ReplaceFolder(string path, 
            Predicate<string> findFolder, Func<string, string> replace, bool onlyOneReplace = false)
        {
            var folders = path.Split('/');
            for (int i=0;i<folders.Length;i++)
            {
                if (findFolder(folders[i]))
                {
                    folders[i] = replace(folders[i]);
                    if (onlyOneReplace)
                        break;
                }
            }
            return string.Join("/", folders);
        }
#endif
    }
    [ExecuteAlways]
    public abstract class PicsSetup<TView> : PicsSetup
        where TView : MonoBehaviour
    {
        public bool setup;
#if UNITY_EDITOR
        private void Update()
        {
            if (!setup) return;
            setup = false;
            var view = GetComponent<TView>();
            Setup(view);
            EditorUtility.SetDirty(gameObject);
        }
        protected abstract void Setup(TView view);

        protected void DoForEachOrientationFolder<TOrientedView>(Sprite example, TView view,
            Func<TView, Orientation, TOrientedView> getOrientedView, Action<TOrientedView, string> action)
        {
            var example_folder = GetPathToFolder(example);
            Dictionary<string, TOrientedView> orientsByFolder = new Dictionary<string, TOrientedView>();
            Utils.ForEach<Orientation>(o => orientsByFolder.Add(o.ToString(), getOrientedView(view, o)));
            var example_folder_orient_ending = example_folder.Substring(example_folder.Length - 3);
            Debug.Assert(example_folder_orient_ending[0] == '_',
                $"{example.name}'s folder {example_folder} should end with orientation");
            example_folder_orient_ending = example_folder_orient_ending.Remove(0, 1);
            Debug.Assert(orientsByFolder.ContainsKey(example_folder_orient_ending),
                $"{example_folder_orient_ending} does not exist, {example_folder} invalid");
            var folderWithoutOrient = example_folder.Remove(example_folder.Length - 2);
            foreach (var (orientString, orient) in orientsByFolder)
                action(orient, folderWithoutOrient + orientString);
        }
#endif
    }



    public abstract class HumanPicsSetup<THuman, TView, TOrientedView> : PicsSetup<TView>
        where TView : HumanView<THuman, TOrientedView>
        where TOrientedView : OrientedHuman
        where THuman : struct, IComponentData
    {
#if UNITY_EDITOR
        protected void DoForEachOrientationFolder(Sprite example, TView view, Action<TOrientedView, string> action)
            => DoForEachOrientationFolder(example, view, (v, orient) => v.GetOrientation(orient), action);
#endif
    }
}
#endif