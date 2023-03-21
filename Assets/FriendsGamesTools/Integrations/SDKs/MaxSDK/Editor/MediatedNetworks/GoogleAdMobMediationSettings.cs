#if MAX_SDK
using System;
using FriendsGamesTools;
using System.IO;
using UnityEditor;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    [Serializable]
    public class GoogleAdMobMediationSettings : MediationSetupManager
    {
        public override Mediations type => Mediations.ADMOB_NETWORK;
        AppLovinSettings maxOwnSettings => MaxSDKSetupManager.maxOwnSettings;
        bool GoogleAdMobAppIdMissingOnAndroid => settings.android.enabled && string.IsNullOrEmpty(maxOwnSettings.AdMobAndroidAppId);
        bool GoogleAdMobAppIdMissingOnIOS => settings.ios.enabled && string.IsNullOrEmpty(maxOwnSettings.AdMobIosAppId);
        public override string folder => "Google";

        public override void OnGUI(ref bool changed)
        {
            base.OnGUI(ref changed);
            if (!settings.enabledMediations.Contains(type)) return;
            OnAdMobAppIdGUI(TargetPlatform.Android, ref changed);
            OnAdMobAppIdGUI(TargetPlatform.IOS, ref changed);
        }

        private void OnAdMobAppIdGUI(TargetPlatform platform, ref bool changed)
        {
            var platformSettings = platform == TargetPlatform.IOS ? settings.ios : settings.android;
            if (!platformSettings.enabled)
                return;
            var id = platform == TargetPlatform.IOS ? maxOwnSettings.AdMobIosAppId : maxOwnSettings.AdMobAndroidAppId;
            EditorGUIUtils.TextField($"GoogleAdMobAppId for {platform}", ref id, ref changed);
            if (changed)
                maxOwnSettings.SaveAsync();
        }
        string AppIdNotSetError(string platformName) => $"{platformName} application MIGHT CRASH ON STARTUP - setup admob app id or remove admob mediation from project";

        [NonSerialized] string goolgePostProcessorPath;
        public override bool GetIOSSet()
        {
            return true;
            //const string scriptName = "PostProcessor";
            //var scriptPathes = AssetDatabase.FindAssets($"t:MonoScript {scriptName}")
            //    .ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            //goolgePostProcessorPath = scriptPathes.Find(path => path.Contains("MaxSdk/Mediation/Google"));
            //var googlePostProcessor = AssetDatabase.LoadAssetAtPath<MonoScript>(goolgePostProcessorPath);
            //var iosSet = false;
            //if (googlePostProcessor != null && googlePostProcessor.text != null && !string.IsNullOrEmpty(MaxSDKSetupManager.maxOwnSettings.AdMobIosAppId))
            //    iosSet = googlePostProcessor.text.Contains(MaxSDKSetupManager.maxOwnSettings.AdMobIosAppId);
            //return iosSet;
        }
        public override void SetupIOS()
        {
            //var text = File.ReadAllText(goolgePostProcessorPath);
            //text = text.ReplaceLineWith("appId = ", $"      var appId = \"{MaxSDKSetupManager.maxOwnSettings.AdMobIosAppId}\";");
            //File.WriteAllText(goolgePostProcessorPath, text);
        }
        public override (bool can, string whyCant) canSetIOS => (!GoogleAdMobAppIdMissingOnIOS, AppIdNotSetError("ios"));


        const string manifestPath = "Assets/Plugins/Android/MaxMediationGoogle.androidlib/AndroidManifest.xml";
        AndroidManifestManager LoadManifest() => new AndroidManifestManager(manifestPath);
        public override bool GetAndroidSet()
        {
            var manifest = LoadManifest();
            if (!manifest.exists)
                return false;
            var appIdInManifest = manifest.GetParam("android:value", "manifest", "application", "meta-data");
            return MaxSDKSetupManager.maxOwnSettings.AdMobAndroidAppId == appIdInManifest;
        }
        public override void SetupAndroid()
        {
            var manifest = LoadManifest();
            manifest.ReplaceParam(MaxSDKSetupManager.maxOwnSettings.AdMobAndroidAppId, "android:value", "manifest", "application", "meta-data");
            manifest.Save();
        }
        public override (bool can, string whyCant) canSetAndroid => (!GoogleAdMobAppIdMissingOnAndroid, AppIdNotSetError("android"));
    }

}
#endif