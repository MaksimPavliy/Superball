#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading;
using System.IO;
using FriendsGamesTools.ZipUtil;
using System.Collections.Generic;
using System;

namespace FriendsGamesTools.Integrations
{
    public static class LibrarySetupUtils
    {
        public static void DownloadUnzipAndImport(string sdkUrl, bool unzip, 
            Func<string, bool> FilterPackageNameInZip = null, Func<List<string>, int> GetPackageIndToImport = null)
        {
            Download(sdkUrl, unzip);
            if (unzip)
                Unzip();
            Import(FilterPackageNameInZip, GetPackageIndToImport);
        }
        const string FileName = "CurrLibraryInstalledByFGT";
        static string tempFolder
#if WINDOWS_EDITOR
            => $"{Path.GetPathRoot(Application.persistentDataPath).Replace("\\", "/")}Temp/FGT";
#else
            => $"{Application.persistentDataPath.Replace("\\", "/")}/Temp/FGT";
#endif
        static string downloadedSDKZipFileName => $"{tempFolder}/{FileName}-Plugin.zip";
        static string downloadedSDKPackageFileName => $"{uncompressedSDKFolder}/{FileName}-Plugin.unitypackage";
        static string uncompressedSDKFolder => $"{tempFolder}/{FileName}-Plugin-Uncompressed";
        private static void Download(string sdkUrl, bool unzip)
        {
            var www = UnityWebRequest.Get(sdkUrl);
            www.SetRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml");
            www.SetRequestHeader("Accept-Encoding", "gzip, deflate");
            www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:68.0) Gecko/20100101 Firefox/68.0");
            //www.SetRequestHeader("Accept-Charset", "ISO-8859-1");

            EditorUtility.DisplayProgressBar("downloading library", "downloading...", 0);
            var downloading = www.SendWebRequest();
            while (!downloading.isDone)
            {
                Thread.Sleep(100);
                EditorUtility.DisplayProgressBar("downloading library", "downloading...", downloading.progress);
            }

            // Save file.
            EditorUtility.DisplayProgressBar("downloading library", "saving...", 1);
            var fileName = unzip ? downloadedSDKZipFileName : downloadedSDKPackageFileName;
            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(fileName, www.downloadHandler.data);
            EditorUtility.ClearProgressBar();
        }
        private static void Unzip()
        {
            // Unzip file.
            EditorUtility.DisplayProgressBar("downloading library", "unzipping...", 0);
            Directory.CreateDirectory(uncompressedSDKFolder);
            Zip.UncompressZip(downloadedSDKZipFileName, uncompressedSDKFolder);
            EditorUtility.ClearProgressBar();
        }
        private static void Import(Func<string, bool> FilterPackageNameInZip, Func<List<string>, int> GetPackageIndToImport)
        {
            // Find *.unitypackage in unzipped.
            EditorUtility.DisplayProgressBar("downloading library", "finding package...", 1);
            var unitypackageFiles = Directory.GetFiles(uncompressedSDKFolder, "*.unitypackage", SearchOption.AllDirectories).ToList();
            if (FilterPackageNameInZip != null)
                unitypackageFiles.RemoveAll(p => !FilterPackageNameInZip(p));
            int packageInd;
            if (unitypackageFiles.Count > 1)
            {
                if (GetPackageIndToImport != null)
                    packageInd = GetPackageIndToImport(unitypackageFiles);
                else
                    packageInd = -1;
                if (!unitypackageFiles.IndIsValid(packageInd))
                {
                    Debug.LogError($"I expect to get 1 unitypackage to import, but received next packages:\n{unitypackageFiles.PrintCollection("\n")}");
                    return;
                }
            }
            else
                packageInd = 0;
            var packagePath = unitypackageFiles[packageInd];
            packagePath = packagePath.Replace("\\", "/");
            // Import.
            EditorUtility.ClearProgressBar();
            AssetDatabase.ImportPackage(packagePath, false);
            Directory.Delete(tempFolder, true);
            // It recompiles here.
        }
    }
}
#endif