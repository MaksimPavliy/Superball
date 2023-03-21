using System.Threading.Tasks;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;
#if ADS

namespace FriendsGamesTools.Ads
{
    // Derive to get source-independant ads. 
    public abstract class AdsManager<TSelf> : AdsManager
        where TSelf : AdsManager<TSelf>
    {
        new public static TSelf instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = (TSelf)this;
        }
    }

    public abstract class AdsManager : MonoBehaviour
    {
        [SerializeField] bool stopTimeOnAds = true;
        [SerializeField] bool useInterstitialTimer = true;
        [SerializeField] public float interstitialInterval = 40;
        private static IAdsSource _adsSourceSDK;
        private static bool adsSourceSearched;
        private MockingAdsSource _mockedAdsSource;
        private float adHiddenRealTime = -1;
        private InterstitialSource _interstitial;
        private RewardedVideoSource _rewarded;
        private BannerSource _banner;

        public bool isShowingAd { get; private set; }
        public static bool AdsEnabled => true;
        public bool interstitialWithTimerAvailable => useInterstitialTimer == true ? realTimeSinceAdShown > interstitialInterval : true;
        public static AdsManager instance { get; private set; }
        public float realTimeSinceAdShown => Time.realtimeSinceStartup - adHiddenRealTime;
        private static AdsSettings config => AdsSettings.instance;
        public AdSourceType sourceTypeRewarded => GetSourceType(AdType.RewardedVideo);
        public AdSourceType sourceTypeInterstitial => GetSourceType(AdType.Interstitial);
        public AdSourceType sourceTypeBanner => GetSourceType(AdType.Banner);

        protected virtual void Awake() => instance = this;

        public static IAdsSource adsSourceSDK
        {
            get
            {
                if (!adsSourceSearched && _adsSourceSDK == null && !config.selectedManagerFullName.IsNullOrEmpty())
                {
                    var type = ReflectionUtils.GetTypeByName(config.selectedManagerFullName, true, true);
                    if (type != null)
                    {
                        var obj = FindObjectOfType(type);
                        if (obj != null)
                            _adsSourceSDK = obj as IAdsSource;
                    }
                    adsSourceSearched = true;
                }
                return _adsSourceSDK;
            }
        }

        private IAdsSource GetUsedAdsSourceSDK(AdType type)
        {
            var sourceType = GetSourceType(type);
            switch (sourceType)
            {
                case AdSourceType.real: return adsSourceSDK;
                default:
                case AdSourceType.mocked: return _mockedAdsSource ?? (_mockedAdsSource = new MockingAdsSource());
                case AdSourceType.omitShowingAndSuccess: return null;
            }
        }

        protected virtual void OnAdShown()
        {
            isShowingAd = true;
            if (stopTimeOnAds)
                GameTime.Pause();
        }

        protected virtual void OnAdHidden()
        {
            isShowingAd = false;
            if (stopTimeOnAds)
                GameTime.Unpause();
            adHiddenRealTime = Time.realtimeSinceStartup;
        }

        public async Task WhileAdsShowing() => await Awaiters.While(() => isShowingAd);

        public InterstitialSource interstitial => _interstitial ?? (_interstitial =
            new InterstitialSource(() => GetUsedAdsSourceSDK(AdType.Interstitial)?.interstitial, OnInterstitialShown, OnInterstitialHidden));
        protected virtual void OnInterstitialShown() { OnAdShown(); }
        protected virtual void OnInterstitialHidden() { OnAdHidden(); }

        public RewardedVideoSource rewarded => _rewarded ?? (_rewarded =
            new RewardedVideoSource(() => GetUsedAdsSourceSDK(AdType.RewardedVideo)?.rewarded, OnRewardedShown, OnRewardedHidden));
        protected virtual void OnRewardedShown() { OnAdShown(); }
        protected virtual void OnRewardedHidden(bool success) { OnAdHidden(); }

        public BannerSource banner => _banner ?? (_banner = new BannerSource(() => GetUsedAdsSourceSDK(AdType.Banner)?.banner));

        protected virtual bool turnOffAdsInTutorial => true;
        protected virtual bool inTutorial
#if TUTORIAL
            => !(ECSGame.Tutorial.TutorialManager.instance?.completed ?? true);
#else
            => false;
#endif
        public virtual AdSourceType GetSourceType(AdType type)
        {
#if TUTORIAL
            if (inTutorial && turnOffAdsInTutorial)
                return AdSourceType.omitShowingAndSuccess;
#endif
#if IAP
            if ((type == AdType.Interstitial || type == AdType.Banner) && IAP.IAPManager.instance.interstitialsRemoved)
                return AdSourceType.omitShowingAndSuccess;
#endif
            return GetSourceType();
        }
        protected virtual AdSourceType GetSourceType()
            => Application.isEditor ? config.typeInEditor : config.typeInBuild;
        public virtual void SetSourceType(AdSourceType sourceType)
        {
            if (GetSourceType() == sourceType)
                return;
            if (Application.isEditor)
                config.typeInEditor = sourceType;
            else
                config.typeInBuild = sourceType;
        }

        public bool isInSimulatedDelay
            => !BuildModeSettings.release && (Mathf.Max(0, adHiddenRealTime) + FGTSettings.instance.simulatedInternetDelay > Time.realtimeSinceStartup);
    }

    public enum AdSourceType { real, mocked, omitShowingAndSuccess }
}
#else
    namespace FriendsGamesTools.Ads
{
    public abstract class AdsManager : MonoBehaviour
    {
        public static bool AdsEnabled => false;
    }
}
#endif