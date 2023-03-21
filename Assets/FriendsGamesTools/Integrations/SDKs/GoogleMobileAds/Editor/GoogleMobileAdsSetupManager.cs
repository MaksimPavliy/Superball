using FriendsGamesTools.Ads;
using FriendsGamesTools.EditorTools.BuildModes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.Integrations
{
    public class GoogleMobileAdsSetupManager : LibrarySetupManager<GoogleMobileAdsSetupManager>
    {
        public const string define = "GOOGLE_MOBILE_ADS";
        public override string Define => define;
        public override string SomeClassNameWithNamespace => "GoogleMobileAds.Common.MobileAdsEventExecutor";
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(AdsModule.define);
        public override List<string> collidesWithModules 
            => base.collidesWithModules.Adding(MaxSDK.MaxSDKSetupManager.define);
#if !GOOGLE_MOBILE_ADS
        public override bool configured => false;
#else
        public override bool configured => EnabledValid() && KeysValid() && MediationsValid();
#endif
        protected override string SDKDownloadURL => "https://github.com/googleads/googleads-mobile-unity/releases/download/v5.1.0/GoogleMobileAds-v5.1.0.unitypackage";
        protected override bool canCheckUpdate => false;
        protected override bool unzipAfterDownload => false;

#if GOOGLE_MOBILE_ADS
        GoogleMobileAdsModuleSettings settings => SettingsInEditor<GoogleMobileAdsModuleSettings>.instance;
        global::GoogleMobileAds.Editor.GoogleMobileAdsSettings pluginSettings => global::GoogleMobileAds.Editor.GoogleMobileAdsSettings.Instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            var changed = false;
            EditorGUIUtils.Toggle("enabled", ref settings.enabled, ref changed);
            EditorGUIUtils.Toggle("use test ad unit ids from Google", ref settings.useTestAdUnitIdsFromGoogle, ref changed);
            EditorGUIUtils.Toggle("rewarded enabled", ref settings.rewardedEnabled, ref changed);
            EditorGUIUtils.Toggle("interstitial enabled", ref settings.interstitialsEnabled, ref changed);
            if (BuildModeSettings.instance.IOSEnabled)
                ShowPlatformSettings(settings.ios, "ios", ref changed);
            if (BuildModeSettings.instance.AndroidEnabled)
                ShowPlatformSettings(settings.android, "android", ref changed);
            GoogleMobileAdsMediation.ShowAllMediations(ref changed);
            if (changed)
                Save();
        }
        void Save()
        {
            EditorUtils.SetDirty(settings);
            pluginSettings.IsAdMobEnabled = settings.enabled;
            pluginSettings.AdMobAndroidAppId = settings.android.appId;
            pluginSettings.AdMobIOSAppId = settings.ios.appId;
            EditorUtils.SetDirty(pluginSettings);
        }
        void ShowPlatformSettings(GoogleMobileAdsModuleSettings.PlatformSettings settings, string platform, ref bool changed)
        {
            EditorGUIUtils.TextField($"{platform} app id", ref settings.appId, ref changed);
            if (this.settings.rewardedEnabled)
                EditorGUIUtils.TextField($"{platform} rewarded ad unit id", ref settings.rewardedAdUnitId, ref changed);
            if (this.settings.interstitialsEnabled)
                EditorGUIUtils.TextField($"{platform} interstitial ad unit id", ref settings.interstitialAdUnitId, ref changed);
        }
        public override string DoReleaseChecks()
        {
            sb.Clear();
            EnabledValid(sb);
            KeysValid(sb);
            MediationsValid(sb);
            return sb.ToString();
        }
        StringBuilder sb = new StringBuilder();
        bool EnabledValid(StringBuilder sb = null)
        {
            if (!pluginSettings.IsAdMobEnabled)
            {
                sb?.AppendLine("Google Mobile Ads not enabled");
                return false;
            }
            return true;
        }
        bool KeysValid(StringBuilder sb = null)
        {
            if (settings.useTestAdUnitIdsFromGoogle)
                return true;
            var valid = true;
            if (!PlatformValid(BuildModeSettings.instance.IOSEnabled, "ios", settings.ios, sb))
                valid = false;
            if (!PlatformValid(BuildModeSettings.instance.AndroidEnabled, "android", settings.android, sb))
                valid = false;
            return valid;
        }
        private bool PlatformValid(bool platformEnabled, string platform, 
            GoogleMobileAdsModuleSettings.PlatformSettings settings, StringBuilder sb = null)
        {
            if (!platformEnabled)
                return true;
            var valid = true;
            if (string.IsNullOrEmpty(settings.appId))
            {
                sb?.AppendLine($"{platform} app id not set");
                valid = false;
            }
            if (this.settings.rewardedEnabled && string.IsNullOrEmpty( settings.rewardedAdUnitId))
            {
                sb?.AppendLine($"{platform} rewarded ad unit id not set");
                valid = false;
            }
            if (this.settings.interstitialsEnabled && string.IsNullOrEmpty(settings.interstitialAdUnitId))
            {
                sb?.AppendLine($"{platform} interstitial ad unit id not set");
                valid = false;
            }
            return valid;
        }
        bool MediationsValid(StringBuilder sb = null) => GoogleMobileAdsMediation.AllValid(sb);
#endif
    }
}