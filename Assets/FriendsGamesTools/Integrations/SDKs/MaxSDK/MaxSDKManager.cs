#if MAX_SDK
using FriendsGamesTools.Ads;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKManager : IntegrationManager<MaxSDKManager>, IAdsSource
    {
        MaxSDKSettings settings => MaxSDKSettings.instance;
        public bool rewardedVideoUsed => settings.rewardedVideosEnabled;
        public bool interstitialAdUsed => settings.interstitialsEnabled;
        public bool bannerAdUsed => settings.bannerAdsEnabled;
        string maxSDKKey => settings.SDKKey;
        string rewardedAdUnitId => settings.currPlatform.rewardedAdUnitId;
        string interstitialAdUnitId => settings.currPlatform.interstitialAdUnitId;
        string bannerAdUnitId => settings.currPlatform.bannerAdUnitId;
        public bool inited { get; private set; }

        private MaxSDKRewardedVideo _rewarded;
        public IRewardedVideoSource rewarded => _rewarded;
        private MaxSDKInterstitial _interstitial;
        public IInterstitialSource interstitial => _interstitial;
        private MaxSDKBanner _banner;
        public IBannerSource banner => _banner;
        private bool isNoAds =>
#if IAP
            IAP.IAPManager.instance.interstitialsRemoved;
#else
            false;
#endif
        public void SetUserConscent(bool accepted)
        {
            if (logging)
                Debug.Log($"MaxSdk.SetHasUserConsent({accepted})");
            MaxSdk.SetHasUserConsent(accepted);
        }
        public bool conscentRequired { get; private set; }

        public bool logging;
        protected override void Awake()
        {
            base.Awake();
            InitSDKWhenInternetAvailable();
        }
        public static bool internet => Application.internetReachability != NetworkReachability.NotReachable;
        async void InitSDKWhenInternetAvailable()
        {
            if (logging)
                Debug.Log($"MaxSDK.Awake will be called when internet available");
            while (!internet)
                await Awaiters.EndOfFrame;


            if (logging)
                Debug.Log($"MaxSDK.Awake");

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                if (logging)
                    Debug.Log($"MaxSDK.Inited");
                // AppLovin SDK is initialized, start loading ads.
                if (rewardedVideoUsed)
                {
                    if (logging)
                        Debug.Log($"MaxSDK init rewarded video called, key {rewardedAdUnitId}");

                    _rewarded = new MaxSDKRewardedVideo(rewardedAdUnitId);
                }
                if (interstitialAdUsed && !isNoAds)
                {
                    if (logging)
                        Debug.Log($"MaxSDK init interstitial called, key {interstitialAdUnitId}");

                    _interstitial = new MaxSDKInterstitial(interstitialAdUnitId);
                }
                if (bannerAdUsed && !isNoAds)
                {
                    if (logging)
                        Debug.Log($"MaxSDK init banner called, key {bannerAdUnitId}");

                    _banner = new MaxSDKBanner(bannerAdUnitId);
                }

                switch (settings.conscent)
                {
                    case ConscentAppliementOption.Applied:
                        conscentRequired = true;
                        break;
                    case ConscentAppliementOption.NotApplied:
                        conscentRequired = false;
                        break;
                    case ConscentAppliementOption.RealValueFromMax:
                        conscentRequired = sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies;
                        break;
                }
                if (logging)
                    Debug.Log($"MaxSDK conscent (option={settings.conscent}) conscentRequired = {conscentRequired}");

                inited = true;
            };
            MaxSdk.SetSdkKey(maxSDKKey);
            MaxSdk.InitializeSdk();
            if (logging)
                Debug.Log($"MaxSDK.Init called key = {maxSDKKey}");
        }

        public void ShowMaxSDKDebugTool()
        {
            if (logging)
                Debug.Log($"ShowMaxSDKDebugTool, {MaxSdk.IsInitialized()}");
            if (MaxSdk.IsInitialized())
                MaxSdk.ShowMediationDebugger();
        }
    }
}
#elif SDKs
using FriendsGamesTools.Integrations;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKManager : IntegrationManager<MaxSDKManager>
    {
        public bool rewardedVideoUsed => false;
        public bool interstitialAdUsed => false;
        public bool bannerAdUsed => false;
        public bool logging;

        public void ShowMaxSDKDebugTool() { }
    }
}
#endif
