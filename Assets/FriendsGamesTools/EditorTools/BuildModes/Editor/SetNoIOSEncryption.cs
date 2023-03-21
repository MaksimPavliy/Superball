#if UNITY_IOS
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ExcemptFromEncryption : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            var plistPath = report.summary.outputPath + "/Info.plist";
            var plist = new PlistDocument(); // Read Info.plist file into memory
            plist.ReadFromString(File.ReadAllText(plistPath));
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            File.WriteAllText(plistPath, plist.WriteToString()); // Override Info.plist
        }
    }
}
#endif