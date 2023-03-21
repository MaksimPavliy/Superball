#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;

namespace FriendsGamesTools.Integrations
{
    public class BuildPostProcessor
    {
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                // Read.
                string projectPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromFile(projectPath);
                string targetGUID = project.GetUnityFrameworkTargetGuid();

                AddFrameworks(project, targetGUID);

                // Fix FB for IOS from https://github.com/facebook/facebook-sdk-for-unity/issues/287
                project.SetBuildProperty(targetGUID, "GCC_C_LANGUAGE_STANDARD", "gnu99");
                project.AddBuildProperty(targetGUID, "GCC_ENABLE_ASM_KEYWORD", "YES");
                project.AddBuildProperty(targetGUID, "GCC_NO_COMMON_BLOCKS", "NO");

                // Disable bitcode, because cant build in xcode.
                project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");

                // Write.
                File.WriteAllText(projectPath, project.WriteToString());

#if PUSH_MANAGER
                var bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                var entitlementsFileName = $"{bundleId.Substring(bundleId.LastIndexOf('.'))}.entitlements";
                var capManager = new ProjectCapabilityManager(projectPath, entitlementsFileName, "Unity-iPhone");
                capManager.AddPushNotifications(false);
                capManager.WriteToFile();
#endif

                AutorotationCrashFix(path);
                IsOrientationEnabledFix(path);
            }
        }

        private static void AutorotationCrashFix(string path) {
            var filePath = Path.Combine(path, "Classes");
            filePath = Path.Combine(filePath, "UI");
            filePath = Path.Combine(filePath, "UnityViewControllerBase+iOS.mm");
            //Debug.Log("File Path for View Controller Class: " + filePath);
            var classFile = File.ReadAllText(filePath);
            var newClassFile = classFile.Replace("NSAssert(UnityShouldAutorotate()", "//NSAssert(UnityShouldAutorotate()");
            File.WriteAllText(filePath, newClassFile);
        }

        private static void IsOrientationEnabledFix(string path) {
            var filePath = Path.Combine(path, "Classes");
            filePath = Path.Combine(filePath, "UnityAppController.mm");
            var classFile = File.ReadAllText(filePath);
            var newClassFile = classFile.Replace("[_UnityAppController window].rootViewController = nil;", "//[_UnityAppController window].rootViewController = nil;");
            File.WriteAllText(filePath, newClassFile);
        }

        static void AddFrameworks(PBXProject project, string targetGUID)
        {
            bool objc = false;
            List<string> frameworks = new List<string>();

#if APP_LOVIN
            frameworks.Add("libz.tbd");
            // AppLovin has own AppLovinPostProcessBuildiOS. So curr is not nessesary.
            // objc = true;
#endif

#if APPS_FLYER
            frameworks.Add("AdSupport.framework");
            frameworks.Add("iAd.framework");
#endif

#if GOOGLE_MOBILE_ADS
            //frameworks.Add("GoogleAppMeasurement.framework");
            //frameworks.Add("GoogleMobileAds.framework");
            //frameworks.Add("GoogleUtilities.framework");
            //frameworks.Add("nanopb.framework");

            project.AddBuildProperty(targetGUID, "CLANG_ENABLE_MODULES", "YES");
            project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "$(inherited)");
#endif

            if (objc)
                project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");

            var frameworksDistinct = frameworks.Distinct();
            foreach (var framework in frameworksDistinct)
                project.AddFrameworkToProject(targetGUID, framework, false);
        }
    }
}
#endif