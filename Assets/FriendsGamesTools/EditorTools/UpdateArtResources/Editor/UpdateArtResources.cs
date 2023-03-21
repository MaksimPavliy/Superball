#if UPDATE_ART
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    public class UpdateArtResources
    {
        public const string SyncArtMenuFolder = "FriendsGamesTools/Sync art";
        public static void Sync(string subFolder) => Run(true, subFolder);

        public static UpdateArtResourcesSettings settings => SettingsInEditor<UpdateArtResourcesSettings>.instance;
        public static bool setupDone => settings.setupDone;
        static string ToProjFolder(string path) => path.Replace('\\', '/').Replace(Application.dataPath, "Assets");
        static bool SettingsExist()
        {
            var settings = SettingsInEditor<UpdateArtResourcesSettings>.GetSettingsInstance(false);
            return settings != null && !string.IsNullOrEmpty(settings.folder_external)
                && !string.IsNullOrEmpty(settings.folder_in_assets);
        }
        public static List<string> ignoredExtensions = new List<string> {
            ".ini", ".DS_Store", ".lnk"
        };
        public static bool FileIgnored(string path)
            => ignoredExtensions.Any(ext => path.EndsWith(ext) || path.EndsWith(ext + ".meta"));
        public static bool isRunning { get; private set; }
        static void ShowSyncingProgress(int completedCount, int totalCount)
        {
            var progress = totalCount == -1 ? 0 : completedCount / (float)totalCount;
            EditorUtility.DisplayProgressBar("syncing art", $"{completedCount}/{totalCount}", progress);
        }
        static string Concat(string folder, string subFolder)
        {
            if (string.IsNullOrEmpty(subFolder))
                return folder;
            return folder + subFolder;
        }
        public static void Run(bool perform, string subFolder)
        {
            ShowSyncingProgress(0, -1);
            isRunning = true;
            try
            {
                var folder_external = Concat(settings.folder_external, subFolder);
                var folder_in_assets = Concat(settings.folder_in_assets, subFolder);
                if (settings.postProcessing)
                {
                    ArtSourcePostprocessing.UpdatePostprocessedArt();
                    folder_external = settings.folder_external_postprocessed;
                }

                var files_in_assets = Directory.GetFiles(folder_in_assets, "*", SearchOption.AllDirectories).ToList();
                var files_in_external = Directory.GetFiles(folder_external, "*", SearchOption.AllDirectories).ToList();
                
                var deletedFiles = new List<string>();
                int total_assets = 0, old = 0;
                //int renamed = 0;
                var totalCount = files_in_assets.Count + files_in_external.Count;
                var currCount = 0;
                for (int i = files_in_assets.Count - 1; i >= 0; i--)
                {
                    currCount++;
                    ShowSyncingProgress(currCount, totalCount);
                    var file_in_assets = files_in_assets[i];
                    if (file_in_assets.EndsWith(".meta"))
                    {
                        files_in_assets.RemoveAt(i);
                        continue;
                    }
                    // Remove crap files 
                    if (FileIgnored(file_in_assets))
                    //||and old person shadows.
                    //(Path.GetFileNameWithoutExtension(file_in_assets).Contains("shadow") && file_in_assets.Contains("\\peoples\\") 
                    //&& (!file_in_assets.Contains("man_shadows") && !file_in_assets.Contains("woman_shadows"))))
                    {
                        files_in_assets.RemoveAt(i);
                        if (perform)
                            File.Delete(file_in_assets);
                        continue;
                    }
                    
                    total_assets++;
                    var file_in_external = file_in_assets.Replace(folder_in_assets, folder_external);
                    if (!files_in_external.Contains(file_in_external))
                    {
                        old++;
                        deletedFiles.Add(file_in_assets);
                        if (perform)
                            //if (!file_in_assets.Contains("laggage_control"))
                            AssetDatabase.DeleteAsset(ToProjFolder(file_in_assets));
                    }
                }
                //Debug.Log($"renamed {renamed} files");
                int newFiles = 0, total_external = 0;
                var addedFiles = new List<string>();
                for (int i = files_in_external.Count - 1; i >= 0; i--)
                {
                    currCount++;
                    ShowSyncingProgress(currCount, totalCount);
                    var file_in_external = files_in_external[i];
                    if (file_in_external.EndsWith(".ini"))
                    {
                        files_in_external.RemoveAt(i);
                        continue;
                    }

                    total_external++;
                    var file_in_assets = file_in_external.Replace(folder_external, folder_in_assets);
                    if (!files_in_assets.Contains(file_in_assets))
                    {
                        newFiles++;
                        addedFiles.Add(file_in_assets);
                    }
                    if (perform)
                    {
                        var newFileDir = Path.GetDirectoryName(file_in_assets);
                        if (!Directory.Exists(newFileDir))
                            Directory.CreateDirectory(newFileDir);
                        try
                        {
                            File.Copy(file_in_external, file_in_assets, true);
                        } catch (Exception e)
                        {
                            Debug.LogError($"{e.Message}\n{e.StackTrace}");
                        }
                    }
                }
                var sb = new StringBuilder();
                sb.AppendLine($"old = {old}/{total_assets}\nnew = {newFiles}/{total_external}");
                if (addedFiles.Count > 0)
                {
                    sb.AppendLine("added files:");
                    sb.AppendLine(addedFiles.PrintCollection("\n"));
                }
                if (deletedFiles.Count > 0)
                {
                    sb.AppendLine("removed files:");
                    sb.AppendLine(deletedFiles.PrintCollection("\n"));
                }
                Debug.Log(sb);
                DeleteEmptyFolders();
                File.WriteAllText("Temp/sync art log.txt", sb.ToString());

                //if (settings.clipAlpha)
                //    ClipPicAlpha.ClipFolderShowingProgressbar(settings.folder_in_assets, settings.clipAlphaThreshold);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                isRunning = false;
            }
        }
        public static void DeleteEmptyFolders()
        {
            var subfolders = Directory.GetDirectories(settings.folder_in_assets, "*", SearchOption.AllDirectories).ToList();
            subfolders.ForEach(f =>
            {
                if (Directory.GetFiles(f).Length == 0)
                    Directory.Delete(f);
            });
        }
    }
}
#endif