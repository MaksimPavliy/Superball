using FriendsGamesTools.DebugTools;
using FriendsGamesTools.Integrations.MaxSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Ads
{
    public class AdsDebugView : FGTModuleDebugPanel
    {
        public override string tab => "Ads";
        public override string module => "ADS";
        public override bool wholeTab => true;
        [SerializeField] GameObject maxSDKParent;
        [SerializeField] GameObject gmaSDKParent;
        [SerializeField] Toggle rewardedAvailable, interstitialAvailable, bannerAvailable, mockedUnavailable;
        [SerializeField] TextMeshProUGUI adSourceTitle, rewardedSuccesses, rewardedFails, interstitialSuccesses, internet;
        [SerializeField] Button realAds, mockedAds, omitAds;

#if ADS
        new AdsSettings settings => AdsSettings.instance;
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            InitAdsType();
            mockedUnavailable.isOn = settings.mockedAdsSimulateNoAds;
            mockedUnavailable.onValueChanged.AddListener(value => settings.mockedAdsSimulateNoAds = value);
            adSourceTitle.text = AdsManager.adsSourceSDK?.GetType()?.Name ?? "No ads source";
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying) return;
#if MAX_SDK
            maxSDKParent.SetActive(Integrations.MaxSDK.MaxSDKManager.instance != null);
#else
            maxSDKParent.SetActive(false);
#endif
#if GOOGLE_MOBILE_ADS_TEST_SUITE
            gmaSDKParent.SetActive(Integrations.GoogleMobileAds.GoogleMobileAdsManager.instance != null);
#else
            gmaSDKParent.SetActive(false);
#endif
        }
        public void OnMaxSDKDebugPressed()
        {
#if MAX_SDK
            Integrations.MaxSDK.MaxSDKManager.instance.ShowMaxSDKDebugTool();
#endif
        }
        public void OnGMASDKDebugPressed()
        {
#if GOOGLE_MOBILE_ADS_TEST_SUITE
            FriendsGamesTools.Integrations.GoogleMobileAds.GoogleMobileAdsManager.ShowDebugTool();
#endif
        }
        AdsManager ads => AdsManager.instance;
        int rewardedSuccessesCount, rewardedFailsCount, interstitialSuccessesCount;
        protected override void Update()
        {
            base.Update();
            if (!Application.isPlaying) return;
            UpdateAdsTypeView();
            rewardedAvailable.isOn = ads?.rewarded.available ?? false;
            interstitialAvailable.isOn = ads?.interstitial.available ?? false;
            bannerAvailable.isOn = ads?.banner.available ?? false;
            rewardedSuccesses.text = rewardedSuccessesCount.ToString();
            rewardedFails.text = rewardedFailsCount.ToString();
            interstitialSuccesses.text = interstitialSuccessesCount.ToString();
            internet.text = Application.internetReachability.ToString();
        }
        public void OnShowInterstitialPressed()
        {
            ads?.interstitial.Show(() => interstitialSuccessesCount++);
        }
        public void OnShowRewardedPressed()
        {
            ads?.rewarded.Show(success => {
                if (success)
                    rewardedSuccessesCount++;
                else
                    rewardedFailsCount++;
            });
        }
        public void OnBannerPressed()
        {
            ads?.banner.Show();
        }
        void InitAdsType()
        {
            realAds.onClick.AddListener(() => SetAdsType(AdSourceType.real));
            mockedAds.onClick.AddListener(() => SetAdsType(AdSourceType.mocked));
            omitAds.onClick.AddListener(() => SetAdsType(AdSourceType.omitShowingAndSuccess));
        }
        void SetAdsType(AdSourceType type)
        {
            ads.SetSourceType(type);
            UpdateAdsTypeView();
        }
        void UpdateAdsTypeView()
        {
            mockedAds.interactable = ads.sourceTypeRewarded != AdSourceType.mocked;
            realAds.interactable = ads.sourceTypeRewarded != AdSourceType.real;
            omitAds.interactable = ads.sourceTypeRewarded != AdSourceType.omitShowingAndSuccess;
        }
        public void LoadAdPressed()
        {
#if MAX_SDK
            MaxSDKRewardedVideo.instance.StartLoading();
#endif
        }
#endif
        }
    }
