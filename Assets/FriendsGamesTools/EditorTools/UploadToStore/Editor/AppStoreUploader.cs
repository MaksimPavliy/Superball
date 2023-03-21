//using UnityEditor;
//using UnityEditor.Callbacks;

//namespace FriendsGamesTools.EditorTools.Upload {
//#if STORE_UPLOAD
//    public class AppStoreUploader
//    {
//        [PostProcessBuild(1000)]
//        public static void OnPostProcessBuild(BuildTarget target, string path)
//        {
//            UnityEngine.Debug.Log($"AppStoreUploader.OnPostProcessBuild called");
//            if (target != BuildTarget.iOS) return;
//            if (!BuildInfo.instance.isCloud) return;
//            UnityEngine.Debug.Log($"settings.cloudBuildTargetContains={settings.cloudBuildTargetContains}");
//            UnityEngine.Debug.Log($"BuildInfo.instance.cloudBuildTargetName={BuildInfo.instance.cloudBuildTargetName}");
//            if (!BuildInfo.instance.cloudBuildTargetName.ToLower().Contains(settings.cloudBuildTargetContains.ToLower())) return;
//            Upload();
//        }
//        public static UploadToStoreSettings settings => SettingsInEditor<UploadToStoreSettings>.instance;
//        public static async void Upload()
//        {
//            UnityEngine.Debug.Log("Upload called");

//            var ipaPath = "/Users/antonpendiur/Programming/test/airport_v1.84(4).ipa";
//            var itunesUserName = settings.itunesUserName;
//            var itunesAppPass = settings.itunesAppPass; 

//            // on cloud build.
//            if (BuildInfo.instance.isCloud)
//            {
//                ipaPath = "$WORKSPACE/.build/last/$TARGET_NAME/build.ipa";
//                //itunesAppPass = "$ITUNES_PASSWORD";
//            }

//            var launcher = new ProcessLauncher();
//            var executableName = "xcrun";
//            var executableParams = $"altool --upload-app -f {ipaPath} -u {itunesUserName} -p {itunesAppPass}";
//            var res = await launcher.Execute(executableName, executableParams,
//                outputline => UnityEngine.Debug.Log(outputline),
//                errorline => UnityEngine.Debug.LogError(errorline)
//                );

//            UnityEngine.Debug.Log($"executing: {executableName} {executableParams.Replace(itunesAppPass, "hidden_password")}");
//            UnityEngine.Debug.Log($"Upload finished {res.success}");
//        }
//    }
//#endif
//}