using System;
using System.Collections.Generic;

namespace FriendsGamesTools.Integrations
{
    public class GoogleMobileAdsModuleSettings : SettingsScriptable<GoogleMobileAdsModuleSettings>
    {
        // TODO: Move admob app id (android and ios) from GoogleMobileAds/Resources/GoogleMobileAdsSettings to this class.
        // How to obtain test device key: https://developers.google.com/admob/unity/test-ads#enable_test_devices
        public List<string> testDevices = new List<string>();

        [Serializable]
        public class PlatformSettings
        {
            public string appId = "";
            public string rewardedAdUnitId = "";
            public string interstitialAdUnitId = "";
        }
        public PlatformSettings ios = new PlatformSettings();
        public PlatformSettings android = new PlatformSettings();
        public PlatformSettings currPlatform => TargetPlatformUtils.current == TargetPlatform.Android ? android : ios;
        public bool useTestAdUnitIdsFromGoogle = false;
        public bool interstitialsEnabled = true;
        public bool rewardedEnabled = true;

        public bool enabled = true;
        public string applovinAPIKey = "";
    }
}