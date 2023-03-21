#if UPDATE_ART
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
 
namespace FriendsGamesTools.EditorTools
{
    public class SetArtCompression
    {
        //[MenuItem(UpdateArtResources.SyncArtMenuFolder + "/Set release quality (slow)")]
        //static void SetReleaseSlowQuality() => SetQuality(Quality.ReleaseSlow);
        //[MenuItem(UpdateArtResources.SyncArtMenuFolder + "/Set release quality (fast)")]
        //static void SetReleaseFastQuality() => SetQuality(Quality.ReleaseFast);
        //[MenuItem(UpdateArtResources.SyncArtMenuFolder + "/Set develop quality")]
        //static void SetDevelopQuality() => SetQuality(Quality.Develop);
        enum Quality { Develop, ReleaseFast, ReleaseSlow }
        private static void SetQuality(Quality quality)
        {
            var startTime = Time.realtimeSinceStartup;
            try
            {
                SetSpritesCompression(quality);
                SetAtlasesCompression(quality);
            } catch 
            {
                EditorUtility.ClearProgressBar();
            } finally
            {
                var duraton = (int)(Time.realtimeSinceStartup - startTime);
                Debug.Log($"switch to {quality} completed in {duraton/60}m {duraton%60}s");
            }
        }
        static UpdateArtResourcesSettings settings => UpdateArtResources.settings;
        public static string ArtSourceFolder => AssetsIterator.InvariantSeparator(settings.folder_in_assets.Remove(0, settings.folder_in_assets.IndexOf("Assets")));
        private static void SetSpritesCompression(Quality quality)
            => IterateSprites($"Switching compression to {quality}", assetPath => SetSpriteCompressionIfNeeded(quality, assetPath));
        public static void IterateSprites(string progressbarTitle, Action<string> spritePathAction)
        {
            int count = 0, maxCount = 0;
            AssetsIterator.IterateAssetsInFolderRecursively(ArtSourceFolder, assetPath =>
            { if (IsPng(assetPath)) maxCount++; });
            AssetsIterator.IterateAssetsInFolderRecursively(ArtSourceFolder, assetPath =>
            {
                if (!IsPng(assetPath))
                    return;
                spritePathAction(assetPath);
                count++;
                EditorUtility.DisplayProgressBar(progressbarTitle,
                    $"{count}/{maxCount}", count / (float)maxCount);
            });
            EditorUtility.ClearProgressBar();
        }
        //[MenuItem(UpdateArtResources.SyncArtMenuFolder + "/Test")]
        //public static void Test()
        //{
        //    SetSpriteCompressionIfNeeded(Quality.Develop, "Assets/Airport737/Art Source Sprites/cafe/bar_back_lvl1/Image0001.png");
        //}
        private static bool IsPng(string str) => str.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase);
        private static void SetSpriteCompressionIfNeeded(Quality quality, string str)
        {
            //if (!str.Contains(Sprites.ArtSourceFolder))
            //    return;
            // Get settings.
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(str);
            const string platform = "iPhone"; // "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PS4", "XboxOne", "Nintendo 3DS" and "tvOS". https://docs.unity3d.com/ScriptReference/TextureImporter.GetPlatformTextureSettings.html
            var settings = ti.GetPlatformTextureSettings(platform);
            // Define needed settings.
            var neededCompression = quality == Quality.ReleaseSlow ? 
                TextureImporterCompression.Uncompressed : TextureImporterCompression.CompressedLQ;
            var neededSettings = new TextureImporterPlatformSettings();
            settings.CopyTo(neededSettings);
            if (quality == Quality.ReleaseFast)
            {
                neededSettings.compressionQuality = 100;
                neededSettings.format = TextureImporterFormat.ASTC_6x6;
                neededSettings.overridden = true;
            } else if (quality == Quality.ReleaseSlow)
            {
                neededSettings.overridden = false;
            }
            else if (quality == Quality.Develop)
            {
                neededSettings.compressionQuality = 0;
                neededSettings.format = TextureImporterFormat.PVRTC_RGBA4;
                neededSettings.overridden = true;
            }
            // Dont reimport if settings are the same.
            if (neededSettings.overridden == settings.overridden
            && neededSettings.compressionQuality == settings.compressionQuality
            && neededSettings.format == settings.format
            && neededSettings.textureCompression == settings.textureCompression
            && neededCompression == settings.textureCompression)
                return;
            // Change settings and reimport.
            ti.textureCompression = neededCompression;
            ti.SetPlatformTextureSettings(neededSettings);
            ti.SaveAndReimport();
        }


        #region Atlases
        //// Cant do this, unity does not have SpriteAtlasImporter as of 2019.1.13f
        //[MenuItem(UpdateArtResources.SyncArtMenuFolder + "/Test set atlases quality")]
        //private static void Test() => SetAtlasesCompression(Quality.Develop);
        private static void SetAtlasesCompression(Quality quality)
        {
            var guids = AssetDatabase.FindAssets("t:SpriteAtlas", null);
            var pathes = guids.ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            //Debug.Log($"there are {pathes.Count} atlases found");
            // pathes = new List<string> { "Assets/Airport737/Rooms/InfoRoom/InfoRoom.spriteatlas" }; // DEBUG
            pathes.ForEach(path =>
            {
                //var importer = AssetImporter.GetAtPath(path);
                //TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
                var lines = File.ReadAllText(path).WithLineEndings("\n").Split('\n').ToList();
                int platformSettingsInd = lines.FindIndex(l=>l.Contains("platformSettings:"));
                int iphoneBuildTargetInd = lines.FindIndex(l=>l.Contains("m_BuildTarget: iPhone"));
                // Add 'override platform' block.
                if (iphoneBuildTargetInd==-1)
                {
                    lines.InsertRange(platformSettingsInd+1, new List<string> {
                        "    - serializedVersion: 2",
                        "      m_BuildTarget: iPhone",
                        "      m_MaxTextureSize: 2048",
                        "      m_ResizeAlgorithm: 0",
                        "      m_TextureFormat: 50",
                        "      m_TextureCompression: 1",
                        "      m_CompressionQuality: 100",
                        "      m_CrunchedCompression: 0",
                        "      m_AllowsAlphaSplitting: 0",
                        "      m_Overridden: 1",
                        "      m_AndroidETC2FallbackOverride: 0"
                    });
                    iphoneBuildTargetInd = platformSettingsInd + 2;
                }
                // Change 'override platform' block.
                int line = iphoneBuildTargetInd;
                string indent = "      ";
                while (lines[line].StartsWith(indent))
                {
                    SetParameter("m_CompressionQuality", quality == Quality.Develop? "0" : "100");
                    SetParameter("m_Overridden", "1");
                    SetParameter("m_TextureFormat", ((int)(quality == Quality.Develop ? 
                        TextureImporterFormat.PVRTC_RGBA4 : TextureImporterFormat.ASTC_6x6)).ToString());
                    void SetParameter(string paramName, string paramValue)
                    {
                        var str = lines[line];
                        if (!str.Contains(paramName))
                            return;
                        str = indent + paramName + ": " + paramValue;
                        lines[line] = str;
                    }
                    line++;
                }
                File.WriteAllText(path, lines.PrintCollection("\n"));
                // Nothing refreshes it.
                //var metaPath = path + ".meta";
                //File.WriteAllText(metaPath, File.ReadAllText(metaPath));
                //AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
                //var importer = AssetImporter.GetAtPath(path);
                //importer.userData = Utils.Random(1, 10000).ToString();
                //importer.SaveAndReimport();
            });
            //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        #endregion
    }
}
#endif