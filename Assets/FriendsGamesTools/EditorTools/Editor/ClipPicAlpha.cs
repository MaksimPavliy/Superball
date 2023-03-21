#if UNITY_EDITOR && EDITOR_TOOLS
using FriendsGamesTools.EditorTools;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    public class ClipPicAlpha : MonoBehaviour
    {
        //[MenuItem("FriendsGamesTools/Sync art/test")]
        //public static void Test()
        //{
        //    //var settings = UpdateArtResources.settings;
        //    //ArtSourcePostprocessing.CopyReplaceDirectory(settings.folder_external, settings.folder_external_postprocessed);

        //    //const string folderPath = @"D:\Programming\FriendsGames\IdleRestaurant\ArtSourceSpritesPostProcessed\interior\table";
        //    //ClipFolderShowingProgressbar(folderPath, 100);

        //    string path = @"D:\Programming\FriendsGames\IdleRestaurant\ArtSourceSpritesPostProcessed\";
        //    var filePathes = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
        //    Debug.Log($"Directory.GetFiles({path}).count = {filePathes.Length}");
        //}
        public static void ClipFolderShowingProgressbar(string folderPath, byte threshold, Predicate<string> filterPics = null)
        {
            data = new byte[5000000]; // 5Mb
            tex = new Texture2D(2, 2);

            int count = 0, maxCount = 0;
            const string title = "Postprocessing sprites";
            EditorUtility.DisplayProgressBar(title, "start", 0);
            var filePathes = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);
            maxCount = filePathes.Length;
            foreach (var assetPath in filePathes)
            {
                if (!assetPath.EndsWith(".png"))
                    continue;
                if (filterPics != null && !filterPics(assetPath))
                    continue;
                count++;
                ClipAlpha(assetPath, threshold);
                EditorUtility.DisplayProgressBar(title, $"processed {count}/{maxCount} sprites", count / (float)maxCount);
                if (count % 10 == 0)
                    System.GC.Collect(100, System.GCCollectionMode.Forced, true, false);
            };
            EditorUtility.ClearProgressBar();
        }
        static byte[] data;
        static Texture2D tex;
        public static void ClipAlpha(string pngPath, byte threshold)
        {
            //var data = File.ReadAllBytes(pngPath);
            int bytesRead = 0;
            do
            {
                using (Stream source = File.OpenRead(pngPath))
                {
                    bytesRead = source.Read(data, 0, data.Length);
                    if (bytesRead < data.Length)
                        break;
                    data = new byte[data.Length * 2];
                }
            } while (true);

            tex.LoadImage(data);
            var pixels = tex.GetRawTextureData<Color32>();
            //Debug.Log(tex.format);
            for (int i = 0; i < pixels.Length; i++)
            {
                // Actually here I receive ARGB32 but not RGBA32.
                //byte a = pixels[i].r;
                //byte r = pixels[i].g;
                //byte g = pixels[i].b;
                //byte b = pixels[i].a;
                byte a = pixels[i].r;
                if (a < threshold)
                    pixels[i] = new Color32(0, 0, 0, 0);
            }
            tex.LoadRawTextureData(pixels);
            var data2 = tex.EncodeToPNG();
            File.WriteAllBytes(pngPath, data2);
        }
    }
}
#endif