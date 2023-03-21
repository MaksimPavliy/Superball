#if UNITY_EDITOR
using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using Ionic.Zip;

namespace FriendsGamesTools.ZipUtil
{
    public class Zip
    {

        public static void CompressDirectory(string dirName, string outputPath, 
            SearchOption searchOption = SearchOption.AllDirectories, Func<string, bool> filesFilter = null)
        {
            string[] files = Directory.GetFiles(dirName, "*.*", searchOption);
            using (var zip = new Ionic.Zip.ZipFile())
            {
                foreach (string filePath in files)
                {
                    if (filesFilter != null && !filesFilter(filePath))
                        continue;
                    var dirInArchive = Path.GetDirectoryName(filePath).Replace(dirName, "");
                    zip.AddFile(filePath, dirInArchive);
                }
                zip.Save(outputPath);
            }
        }
        public static void UncompressZip(string zipPath, string outputDirectory)
        {
            UniZip.ZipUtil.Unzip(zipPath, outputDirectory);
        }
    }
}
#endif