using FriendsGamesTools.Ads;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class AppLovinSetupManager : LibrarySetupManager<AppLovinSetupManager>
    {
        public const string define = "APP_LOVIN";
        public override string SomeClassNameWithNamespace => "AppLovin";
        public override string Define => define;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(AdsModule.define);
        public override HowToModule HowTo() => new AppLovinSetupManager_HowTo();
        //public override List<string> dependFromModules => new List<string> { GooglePlayGamesSetupManager.define };

#if !APP_LOVIN
        public override bool configured => false;
#elif GOOGLE_PlAY_GAMES
        public override bool configured => applovinSDKKeyOk && isManifestOK 
            && GooglePlayGamesSetupManager.ExistsAndConfigured;
#else
        public override bool configured => applovinSDKKeyOk;
#endif
        protected override string SDKSharingURL => "https://drive.google.com/open?id=1x_OGNiPko7CxLMqhdFG71a0wnZ5bhZqS";
        protected override bool canCheckUpdate => false;

#if APP_LOVIN
        AppLovinSettings settings => SettingsInEditor<AppLovinSettings>.instance;
        bool applovinSDKKeyOk => !string.IsNullOrEmpty(settings.applovinSDKKey);
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            UpdateManifestStatus();
        }

        protected override void OnCompiledFocus()
        {
            base.OnCompiledFocus();
            UpdateManifestStatus();
            EditorUtility.SetDirty(settings);
        }

        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            settings.applovinSDKKey = EditorGUILayout.TextField("AppLovin SDK Key", settings.applovinSDKKey);
            if (!applovinSDKKeyOk)
                EditorGUIUtils.ColoredLabel("AppLovin SDK Key is not set up", Color.red);
            settings.interstitialsEnabled = EditorGUILayout.Toggle("interstitials enabled", settings.interstitialsEnabled);
            settings.rewardedVideosEnabled = EditorGUILayout.Toggle("rewarded videos enabled", settings.rewardedVideosEnabled);
            settings.testModeEnabled = EditorGUILayout.Toggle("test mode enabled", settings.testModeEnabled);
            if (settings.testModeEnabled)
                EditorGUIUtils.ColoredLabel("Test mode should be disabled for release builds", EditorGUIUtils.warningColor);

            EditorGUIUtils.PushEnabling(applovinSDKKeyOk);
            if (GUILayout.Button("Setup android manifest"))
                ModifyManifest();
            EditorGUIUtils.PopEnabling();
            if (!isManifestOK)
                EditorGUIUtils.ColoredLabel("android manifest is not set up", EditorGUIUtils.warningColor);

            if (!GooglePlayGamesSetupManager.ExistsAndConfigured)
                EditorGUIUtils.ColoredLabel("Google play games not setup - Applovin requires it for android", EditorGUIUtils.warningColor);
        }

        #region Manifest
        bool isManifestOK;
        const string PluginManifestPath = "Assets/AppLovinSdk/Plugins/Android/AndroidManifest.xml";
        void UpdateManifestStatus()
        {
            var manifest = new AndroidManifestManager();
            if (manifest.exists) {
                var haveAppLovinContents = manifest.contents.Contains(manifestApplicationContents);
                var appIdOk = manifest.GetPackageName() == ApplicationIdValidator.androidId;
                var SDKKeyExists = !string.IsNullOrEmpty(settings.applovinSDKKey);
                var ownManifestContents = File.ReadAllText(PluginManifestPath).ToCrLf();
                var ownAllLovinManifestOk = SDKKeyExists 
                    && ownManifestContents.Contains(ApplicationIdValidator.androidId)
                    && ownManifestContents.Contains(settings.applovinSDKKey);
                isManifestOK = haveAppLovinContents && appIdOk && SDKKeyExists && ownAllLovinManifestOk;
            } else 
                isManifestOK = false;
        }
        const string manifestComment = "<!-- Applovin -->";
        const string applovinSDKKeyPrefix = "<meta-data android:name=\"applovin.sdk.key\" android:value=\"";
        string manifestApplicationContents =>
$"    {manifestComment}\r\n" +
$"    {applovinSDKKeyPrefix}{settings.applovinSDKKey}\" />\r\n";
        void ModifyManifest()
        {
            var manifest = new AndroidManifestManager();
            if (!manifest.exists)
            {
                Debug.LogError("android manifest does not exist."); // TODO: Create default manifest.
                return;
            }
            manifest.RemoveTagWith("applovin.sdk.key");
            manifest.RemoveLineWith(manifestComment);
            manifest.AddInApplicationTag(manifestApplicationContents);
            manifest.SetPackageName(ApplicationIdValidator.androidId);
            manifest.Save();

            var ownManifest = new AndroidManifestManager(PluginManifestPath);
            ownManifest.RemoveTagWith("applovin.sdk.key");
            ownManifest.AddInApplicationTag(manifestApplicationContents);
            ownManifest.SetPackageName(ApplicationIdValidator.androidId);
            ownManifest.Save();

            UpdateManifestStatus();
        }
        #endregion

#endif
    }
}


