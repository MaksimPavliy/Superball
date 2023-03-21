using System.Collections.Generic;
using FriendsGamesTools.ECSGame;
using System.IO;
using System;
using UnityEngine;
using FriendsGamesTools.EditorTools;
using UnityEditor;

namespace FriendsGamesTools
{
    public class HCTemplateModule : ECSModule
    {
        public const string define = "HC_TEMPLATE";
        public override string Define => define;
        public override HowToModule HowTo() => new HCTemplateModule_HowTo();
        public const string RunCodegeneration = "Run codegeneration";

#if HC_TEMPLATE
        const string HC = "HC";
        const string DirectoryToRename = FriendsGamesManager.AssetsFolder + "/" + HC;
        public override void ShortcutOnGUI()
        {
            if (Directory.Exists(DirectoryToRename))
                OnRenameGUI();
        }
        string newName = HC;
        private void OnRenameGUI()
        {
            var changed = false;
            GUILayout.BeginHorizontal();
            var valid = !newName.IsNullOrEmpty() && newName.Length >= 2 && newName.Length < 100 && newName.IndexOf(' ') == -1 && newName[0].IsLetter();
            EditorGUIUtils.ShowValid(valid);
            EditorGUIUtils.TextField("", ref newName, ref changed, 300);
            EditorGUIUtils.PushEnabling(valid);
            if (GUILayout.Button("rename template game"))
                RenameGame();
            EditorGUIUtils.PopEnabling();
            GUILayout.EndHorizontal();
        }

        private void RenameGame()
        {
            string ReplaceName(string hcName) => newName + hcName.Remove(0, HC.Length);
            string ReplacePath(string hcPath)
            {
                var hcName = Path.GetFileName(hcPath);
                var name = ReplaceName(hcName);
                return name;
                //var path = hcPath.Remove(hcPath.Length - hcName.Length) + name;
                //return path;
            }
            var prefabPathes = new List<string>();
            var codePathes = new List<string>();
            var classesToRename = new Dictionary<string, string>();
            AssetsIterator.IterateAssetsInFolderRecursively(DirectoryToRename, assetPath =>
            {
                if (assetPath.EndsWith(".prefab") && Path.GetFileName(assetPath).StartsWith(HC))
                    prefabPathes.Add(assetPath);
                if (assetPath.EndsWith(".cs"))
                {
                    codePathes.Add(assetPath);
                    if (Path.GetFileName(assetPath).StartsWith(HC))
                    {
                        var oldClassName = Path.GetFileNameWithoutExtension(assetPath);
                        var newClassName = ReplaceName(oldClassName);
                        classesToRename.Add(oldClassName, newClassName);
                    }
                }
            });
            codePathes.ForEach(codePath =>
            {
                var code = File.ReadAllText(codePath);
                foreach (var (oldClassName, newClassName) in classesToRename)
                    code = code.Replace(oldClassName, newClassName);
                code = code.Replace($"namespace {HC}", $"namespace {newName}");
                File.WriteAllText(codePath, code);
            });
            prefabPathes.ForEach(oldPath => AssetDatabase.RenameAsset(oldPath, ReplacePath(oldPath)));
            codePathes.ForEach(oldPath =>
            {
                if (Path.GetFileName(oldPath).StartsWith(HC))
                {
                    var err = AssetDatabase.RenameAsset(oldPath, ReplacePath(oldPath));
                    if (!err.IsNullOrEmpty())
                        Debug.LogError(err);
                }
            });
            AssetDatabase.MoveAsset(DirectoryToRename, FriendsGamesManager.AssetsFolder + "/" + newName);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}