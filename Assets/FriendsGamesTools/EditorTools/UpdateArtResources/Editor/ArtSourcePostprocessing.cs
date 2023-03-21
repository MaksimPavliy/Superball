#if UPDATE_ART
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace FriendsGamesTools.EditorTools
{
    public static class ArtSourcePostprocessing
    {
        public static UpdateArtResourcesSettings settings => UpdateArtResources.settings;

        //[MenuItem("FriendsGamesTools/Sync art/Postprocess only")]
        public static void UpdatePostprocessedArt()
        {
            if (string.IsNullOrEmpty(settings.folder_external_postprocessed))
                throw new Exception("set folder_external_postprocessed");

            CopyReplaceDirectory(settings.folder_external, settings.folder_external_postprocessed);

            if (settings.clipAlpha)
            {
                ClipPicAlpha.ClipFolderShowingProgressbar(settings.folder_external_postprocessed,
                    settings.clipAlphaThreshold, path => !path.Contains("/UI/"));
            }
        }

        public static void CopyReplaceDirectory(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            var oldDirs = Directory.GetDirectories(to, "*", SearchOption.AllDirectories).ToList();
            var oldFiles = Directory.GetDirectories(to, "*", SearchOption.AllDirectories).ToList();

            var newDirs = Directory.GetDirectories(from, "*", SearchOption.AllDirectories).ConvertAll(dirPath => dirPath.Replace(from, to));
            var newFiles = Directory.GetFiles(from, "*.*", SearchOption.AllDirectories).ConvertAll(filePath => filePath.Replace(from, to));

            // Create new dirs.
            foreach (var path in newDirs)
            {
                if (!oldDirs.Contains(path))
                    Directory.CreateDirectory(path);
            }
            // Remove old dirs.
            foreach (var path in oldDirs)
            {
                if (!newDirs.Contains(path))
                    Directory.Delete(path, true);
            }
            // Remove old files.
            foreach (var path in oldFiles)
            {
                if (!newFiles.Contains(path) && File.Exists(path))
                    File.Delete(path);
            }
            // Add new files.
            foreach (var path in newFiles)
            {
                var fromPath = path.Replace(to, from);
                //UnityEngine.Debug.Log($"from {fromPath} to {path}");
                File.Copy(fromPath, path, true);
            }
        }

        private static void ClearDirectory(string dir)
        {
            foreach (string path in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                File.Delete(path);
            foreach (string path in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
                Directory.Delete(path);
        } 
    }
}
#endif