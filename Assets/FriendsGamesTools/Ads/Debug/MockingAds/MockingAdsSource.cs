#if ADS
using System;

namespace FriendsGamesTools.Ads
{
    public class MockingAdsSource : IAdsSource
    {
        MockedRewardedVideo _rewarded;
        MockedInterstitial _interstitial;
        public IRewardedVideoSource rewarded => _rewarded ?? (_rewarded = new MockedRewardedVideo());
        public IInterstitialSource interstitial => _interstitial ?? (_interstitial = new MockedInterstitial());
        public bool conscentRequired => true;

        public IBannerSource banner => null;

        public void SetUserConscent(bool accepted) {}
    }

    public class MockedRewardedVideo : IRewardedVideoSource
    {
        public bool available => !AdsSettings.instance.mockedAdsSimulateNoAds;
        public async void Show(Action<bool> onCompleted) {
            await AsyncUtils.SimulateInternetDelayIfDebug();
            RewardedNativeWindow.Show(onCompleted);
        }
    }
    public class MockedInterstitial : IInterstitialSource
    {
        public bool available => !AdsSettings.instance.mockedAdsSimulateNoAds;
        public async void Show(Action onCompleted) {
            await AsyncUtils.SimulateInternetDelayIfDebug();
            InterstitialNativeWindow.Show(onCompleted);
        }
    }
}
#endif