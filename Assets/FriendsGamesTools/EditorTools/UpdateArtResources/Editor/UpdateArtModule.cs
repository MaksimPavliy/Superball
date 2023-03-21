using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    public class UpdateArtModule : ModuleManager
    {
        public const string define = "UPDATE_ART";
        public override string Define => define;
        public override string parentModule => EditorToolsModule.define;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(EditorToolsModule.define);
        public override HowToModule HowTo() => new UpdateArtModule_HowTo();

#if UPDATE_ART
        public static UpdateArtResourcesSettings settings => UpdateArtResources.settings;
        UnityEngine.Object subFolder = null;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;

            EditorGUIUtils.FolderField("subfolder", ref subFolder, ref changed);
            OnSyncArtButtonGUI();

            GUILayout.BeginHorizontal();
            EditorGUIUtils.TextField("folder_in_assets", ref settings.folder_in_assets, ref changed);
            if (GUILayout.Button("open", GUILayout.Width(100))) EditorUtility.RevealInFinder(settings.folder_in_assets);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUIUtils.TextField("folder_external", ref settings.folder_external, ref changed);
            if (GUILayout.Button("open", GUILayout.Width(100))) EditorUtility.RevealInFinder(settings.folder_external);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("change sprites resolution"))
                ChangeSpritesResolution();

            ShowSpritesCountInDirectory();

                if (changed)
                EditorUtility.SetDirty(settings);
        }

        bool showSpritesCountInDireactory;
        UnityEngine.Object dirObj;
        void ShowSpritesCountInDirectory()
        {
            EditorGUIUtils.ShowOpenClose(ref showSpritesCountInDireactory, "count sprites in directory");
            if (!showSpritesCountInDireactory)
                return;
            EditorGUIUtils.InHorizontal(() =>
            {
                var changed = false;
                EditorGUIUtils.FolderField("directory", ref dirObj, ref changed);                
                if (GUILayout.Button("count PNG"))
                {
                    var path = AssetDatabase.GetAssetPath(dirObj);
                    var files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                    Debug.Log($"{files.Length} png found in {path}");
                }
            });
        }

        private void ChangeSpritesResolution()
        {
            //int count = 0, maxcount = 10;
            SetArtCompression.IterateSprites("changing resolution", assetPath => {
                //count++;
                //if (count > maxcount)
                //    return;
                var texture=  AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (texture == null)
                {
                    Debug.Log($"texture == null at {assetPath}");
                    return;
                }
                TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                if (ti == null)
                {
                    Debug.Log($"ti == null at {assetPath}");
                    return;
                }
                var maxTextureSize = Mathf.RoundToInt(Mathf.Pow(2, Mathf.Max(1, Mathf.FloorToInt((float)Math.Log(Math.Max(texture.width, texture.height), 2)) - 2)));
                if (maxTextureSize == ti.maxTextureSize)
                    return;
                ti.maxTextureSize = maxTextureSize;
                ti.SaveAndReimport();
                //Debug.Log($"texture {texture.width}x{texture.height}  set max size to {ti.maxTextureSize}");
            });
        }

        public override void ShortcutOnGUI()
        {
            base.ShortcutOnGUI();
            if (settings.setupDone)
                OnSyncArtButtonGUI();
        }
        void OnSyncArtButtonGUI()
        {
            EditorGUIUtils.PushEnabling(settings.setupDone && !UpdateArtResources.isRunning);
            if (GUILayout.Button("Do sync art"))
            {
                EditorGUIUtils.DoIfConfirmed("confirm updating art", () =>
                {
                    string subFolderPath = null;
                    if (subFolder != null)
                    {
                        subFolderPath = AssetDatabase.GetAssetPath(subFolder).Replace("/", "\\");
                        var str = settings.folder_in_assets;
                        str = str.Substring(str.IndexOf("Assets"));
                        if (subFolderPath.IndexOf(str) == -1)
                        {
                            Debug.LogError($"folder {AssetDatabase.GetAssetPath(subFolder)} is not in {settings.folder_in_assets}");
                            return;
                        }
                        subFolderPath = subFolderPath.Replace(str, "");
                    }
                    UpdateArtResources.Sync(subFolderPath);
                });
            }
            EditorGUIUtils.PopEnabling();
        }
#endif
    }
}