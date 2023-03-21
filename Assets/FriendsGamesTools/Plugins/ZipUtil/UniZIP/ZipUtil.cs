#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

namespace UniZip
{
    public class ZipUtil
    {

        public static void Unzip(string zipFilePath, string location)
        {
            Directory.CreateDirectory(location);

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {

                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Zip(string zipFileName, params string[] files)
        {
            string path = Path.GetDirectoryName(zipFileName);
            Directory.CreateDirectory(path);

            using (ZipFile zip = new ZipFile())
            {
                foreach (string file in files)
                {
                    zip.AddFile(file, "");
                }
                zip.Save(zipFileName);
            }
        }
    }
}
#endif