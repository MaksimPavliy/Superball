#if GOOGLE_MOBILE_ADS
using FriendsGamesTools.Ads;
using FriendsGamesTools.EditorTools.BuildModes;
using GoogleMobileAds.Api;
using UnityEngine;

namespace FriendsGamesTools.Integrations.GoogleMobileAds
{
    public class GoogleMobileAdsManager : IntegrationManager<GoogleMobileAdsManager>, IAdsSource
    {
        GoogleMobileAdsRewardedSource _rewarded;
        public IRewardedVideoSource rewarded => _rewarded;
        GoogleMobileAdsInterstitialSource _interstitial;
        public IInterstitialSource interstitial => _interstitial;
        static GoogleMobileAdsModuleSettings settings => GoogleMobileAdsModuleSettings.instance;
        public void SetUserConscent(bool accepted) => throw new System.Exception("GDPR not implemented");
        public bool conscentRequired => throw new System.Exception("GDPR not implemented");

        protected override void Awake()
        {
            base.Awake();
            if (settings.useTestAdUnitIdsFromGoogle)
                SetTestAdUnitIdsFromGoogle();
            Debug.Log("initing mobile ads...");
            AppLovinInitialize();
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("mobile ads inited");
                _rewarded = new GoogleMobileAdsRewardedSource();
                _interstitial = new GoogleMobileAdsInterstitialSource();
            });
        }
        void AppLovinInitialize()
        {
            var AppLovin = ReflectionUtils.GetTypeByName("GoogleMobileAds.Api.Mediation.AppLovin.AppLovin", true, true);
            AppLovin.CallStaticMethod("Initialize");
            //global::GoogleMobileAds.Api.Mediation.AppLovin.AppLovin.Initialize();
        }
        void SetTestAdUnitIdsFromGoogle()
        {
            settings.ios.interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
            settings.android.interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
            settings.ios.rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
            settings.android.rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        }

        public static AdRequest CreateAdRequest()
        {
            var builder = new AdRequest.Builder();
            if (!BuildModeSettings.release)
                settings.testDevices.ForEach(testDeviceKey => builder = builder.AddTestDevice(testDeviceKey));
            return builder.Build();
        }

#if GOOGLE_MOBILE_ADS_TEST_SUITE
        public static void ShowDebugTool()
            => GoogleMobileAdsMediationTestSuite.Api.MediationTestSuite.Show();
#endif
    }
}
#elif GOOGLE_MOBILE_ADS_DISABLED
namespace FriendsGamesTools.Integrations.GoogleMobileAds
{
    public class GoogleMobileAdsManager : IntegrationManager<GoogleMobileAdsManager>
    {

    }
}
#endif