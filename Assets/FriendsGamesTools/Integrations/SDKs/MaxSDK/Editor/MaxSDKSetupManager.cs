using FriendsGamesTools.Ads;
using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKSetupManager : LibrarySetupManager<MaxSDKSetupManager>
    {
        public const string define = "MAX_SDK";
        public override string Define => define;
        public override string SomeClassNameWithNamespace => "MaxSdk";
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(AdsModule.define);
        public override List<string> collidesWithModules 
            => base.collidesWithModules.Adding(AppLovinSetupManager.define).Adding(GoogleMobileAdsSetupManager.define);
        public override HowToModule HowTo() => new MaxSDKSetupManager_HowTo();
#if !MAX_SDK
        public override bool configured => false;
#else
        public override bool configured => SDKKeyOk && CheckAdUnitsOk(false) && mediationsOk;
#endif
        protected override string SDKDownloadURL
            => "https://artifacts.applovin.com/unity/com/applovin/applovin-sdk/AppLovin-MAX-Unity-Plugin-4.3.0-Android-10.3.0-iOS-10.3.0.unitypackage";
        protected override bool canCheckUpdate => false;
        protected override bool unzipAfterDownload => false;

#if MAX_SDK
        MaxSDKSettings settings => SettingsInEditor<MaxSDKSettings>.instance;
        public static AppLovinSettings maxOwnSettings => AppLovinSettings.Instance;
        bool atLeastOneMediationInstalled => MediationSetupManager.instances.Values.Any(m => m.installed);
        bool SDKKeyOk => !string.IsNullOrEmpty(settings.SDKKey) && (settings.SDKKey == FriendsGamesConstants.MAXSDKKey || settings.customSDKKeyAllowed);
        bool mediationsIOSOk => !settings.ios.enabled || (atLeastOneMediationInstalled && MediationSetupManager.instances.Values.All(m => m.iosOk));
        bool mediationsAndroidOk => !settings.android.enabled || (atLeastOneMediationInstalled && MediationSetupManager.instances.Values.All(m => m.androidOk));
        bool mediationsOk => mediationsIOSOk && mediationsAndroidOk;
        public override string DoReleaseChecks()
        {
            var baseErr = base.DoReleaseChecks();
            if (!string.IsNullOrEmpty(baseErr))
                return baseErr;
            string err = "";
            if (BuildModesModule.settings.AndroidEnabled != settings.android.enabled)
                err += $"Global settings in {BuildModesModule.define} " +
                    $"say AndroidEnabled={BuildModesModule.settings.AndroidEnabled}," +
                    $"but max sdk sets android.enabled={settings.android.enabled}";
            if (BuildModesModule.settings.IOSEnabled != settings.ios.enabled)
                err += $"Global settings in {BuildModesModule.define} " +
                    $"say IOSEnabled={BuildModesModule.settings.IOSEnabled}," +
                    $"but max sdk sets ios.enabled={settings.ios.enabled}";
            return null;
        }
        protected override void ShowUpdateGUI()
        {
            if (GUILayout.Button("check update"))
                EditorApplication.ExecuteMenuItem("Assets/AppLovin Integration Manager");
        }
        protected override void OnCompiledFocus()
        {
            base.OnCompiledFocus();
            MediationSetupManager.instances.Values.ForEach(m => m.Init());
            EditorUtility.SetDirty(settings);
        }

        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            var changed = false;

            EditorGUIUtils.TextField("Max SDK Key", ref settings.SDKKey, ref changed);
            if (!SDKKeyOk)
                EditorGUIUtils.ColoredLabel("SDK Key is not set up", Color.red);
            if (!settings.SDKKey.IsNullOrEmpty() && settings.SDKKey != FriendsGamesConstants.MAXSDKKey || settings.customSDKKeyAllowed)
                EditorGUIUtils.Toggle("3rd party sdk key?", ref settings.customSDKKeyAllowed, ref changed);

            DrawAdTypes(ref changed);

            DrawConscent(ref changed);

            DrawPlatforms("IOS", settings.ios, ref changed);
            DrawPlatforms("Android", settings.android, ref changed);

            // Add mediation networks for platforms.
            EditorGUIUtils.LabelAtCenter("MEDIATIONS");
            foreach (var m in MediationSetupManager.instances.Values)
                m.OnGUI(ref changed);

            if (changed)
                settings.SetChanged();
        }

        void DrawConscent(ref bool changed)
        {
            EditorGUIUtils.Toolbar("conscent in editor", ref settings.conscentInEditor, ref changed);
            EditorGUIUtils.Toolbar("conscent in dev build", ref settings.conscentInDevBuild, ref changed);
        }

        void DrawAdTypes(ref bool changed)
        {
            EditorGUIUtils.Toggle("interstitials enabled", ref settings.interstitialsEnabled, ref changed);
            EditorGUIUtils.Toggle("rewarded videos enabled", ref settings.rewardedVideosEnabled, ref changed);
            EditorGUIUtils.Toggle("banner ads", ref settings.bannerAdsEnabled, ref changed);
            CheckAdUnitsOk(true);
        }
        bool CheckAdUnitsOk(bool drawError)
        {
            const string rewarded = "rewarded", interstitial = "interstitial", banner = "banner";
            const string ios = "IOS", android = "Android";
            bool ok = true;
            ok &= CheckAdUnitOk(rewarded, settings.rewardedVideosEnabled, ios, settings.ios.enabled, settings.ios.rewardedAdUnitId, drawError);
            ok &= CheckAdUnitOk(rewarded, settings.rewardedVideosEnabled, android, settings.android.enabled, settings.android.rewardedAdUnitId, drawError);
            ok &= CheckAdUnitOk(interstitial, settings.interstitialsEnabled, ios, settings.ios.enabled, settings.ios.interstitialAdUnitId, drawError);
            ok &= CheckAdUnitOk(interstitial, settings.interstitialsEnabled, android, settings.android.enabled, settings.android.interstitialAdUnitId, drawError);
            ok &= CheckAdUnitOk(banner, settings.bannerAdsEnabled, ios, settings.ios.enabled, settings.ios.bannerAdUnitId, drawError);
            ok &= CheckAdUnitOk(banner, settings.bannerAdsEnabled, android, settings.android.enabled, settings.android.bannerAdUnitId, drawError);
            return ok;
        }
        bool CheckAdUnitOk(string adType, bool adTypeEnabled, string platform, bool platformEnabled, string adUnitId, bool drawError)
        {
            var ok = !adTypeEnabled || !platformEnabled || !string.IsNullOrEmpty(adUnitId);
            if (!ok && drawError)
                EditorGUIUtils.ColoredLabel($"ad unit id not set for {adType} on {platform}", Color.red);
            return ok;
        }

        private void DrawPlatforms(string platform, MaxSDKSettings.PlatformSettings platformSettings, ref bool changed)
        {
            platformSettings.enabled = EditorGUILayout.Toggle($"{platform} setup required", platformSettings.enabled);
            if (!platformSettings.enabled)
                return;
            if (settings.interstitialsEnabled)
                EditorGUIUtils.TextField("interstitialAdUnitId", ref platformSettings.interstitialAdUnitId, ref changed);
            if (settings.rewardedVideosEnabled)
                EditorGUIUtils.TextField("rewardedAdUnitId", ref platformSettings.rewardedAdUnitId, ref changed);
            if (settings.bannerAdsEnabled)
                EditorGUIUtils.TextField("bannerAdUnitId", ref platformSettings.bannerAdUnitId, ref changed);
        }
#endif
    }
}


