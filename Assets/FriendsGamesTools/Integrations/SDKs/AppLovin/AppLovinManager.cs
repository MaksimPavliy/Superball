#if APP_LOVIN
using FriendsGamesTools.Ads;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class AppLovinManager : IntegrationManager<AppLovinManager>, IAdsSource
    {
        AppLovinSettings settings => AppLovinSettings.instance;
        public bool rewardedVideoUsed => settings.rewardedVideosEnabled;
        public bool interstitialAdUsed => settings.interstitialsEnabled;
        public bool testAdsEnabledInSDK => settings.testModeEnabled;
        public void SetUserConscent(bool accepted) => throw new System.Exception("GDPR not implemented");

        private AppLovinRewarded _rewardedVideo;
        private AppLovinInterstitial _interstitial;
        public IRewardedVideoSource rewarded => _rewardedVideo;
        public IInterstitialSource interstitial => _interstitial;
        protected override void Awake()
        {
            base.Awake();
            // Applovin does not work in unity.
            if (!Application.isEditor)
            {
                AppLovin.SetSdkKey(settings.applovinSDKKey);
                AppLovin.InitializeSdk();
                if (testAdsEnabledInSDK)
                    AppLovin.SetTestAdsEnabled("true");
                if (rewardedVideoUsed)
                {
                    _rewardedVideo = new AppLovinRewarded();
                    _rewardedVideo.Init();
                }
                if (interstitialAdUsed)
                {
                    _interstitial = new AppLovinInterstitial();
                    _interstitial.Init();
                }
                AppLovin.SetUnityAdListener(name); // Make this gameobject with current name receive unity ad event callbacks.
            }
        }
        void onAppLovinEventReceived(string eventName)
        {
            if (rewardedVideoUsed)
                _rewardedVideo.RewardedVideoAppLovinEventReceived(eventName);
            if (interstitialAdUsed)
                _interstitial.RewardedVideoAppLovinEventReceived(eventName);
        }
        public bool editorTestModeEnabled => Application.isEditor;
    }
}
#elif SDKs
namespace FriendsGamesTools.Integrations
{
    public class AppLovinManager : IntegrationManager<AppLovinManager>
    {
        public bool rewardedVideoUsed => false;
        public bool interstitialAdUsed => false;
        public bool testAdsEnabledInSDK => false;
        public bool editorTestModeEnabled => false;
    }
}
#endif