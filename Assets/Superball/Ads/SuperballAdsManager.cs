using FriendsGamesTools.Ads;
using System;
using UnityEngine;

namespace Superball
{
    public class SuperballAdsManager
#if ADS
    : AdsManager<SuperballAdsManager>
#else
    : MonoBehaviour
#endif
    {
#if ADS
        public bool InterstitialTimerReady => GetTotalSeconds(DateTime.UtcNow) - _lastInterstitialSecondsTimestamp >= interstitialMinimumInterval;
        private long _lastInterstitialSecondsTimestamp = -1;
        private int interstitialMinimumInterval = 40;

        private bool _adShown = false;
        Action onAdSuccess;
        Action onAdFailed;

        public bool AdShown
        {
            private set
            {
                _adShown = value;
            }
            get
            {
                return _adShown;
            }
        }

        public void ShowInterstitial(string nameInAnalytics, Action onSuccess, Action onHidden)
        {
            if (!InterstitialTimerReady) return;

            if (!interstitial.available) return;

            AdShown = true;

            onAdSuccess = onSuccess;
            onAdFailed = onHidden;

            HCAnalyticsManager.OnAdPressed(AdType.Interstitial, nameInAnalytics, interstitial.available);

            interstitial.Show(() =>
            {
                onAdSuccess?.Invoke();

                HCAnalyticsManager.OnAdShowingFinished(AdType.Interstitial, nameInAnalytics, "watched");

                onAdSuccess = null;
                onAdFailed = null;

                AdShown = false;
            });
        }

        public void ShowRewarded(string nameInAnalytics, Action onSuccess)
        {
            AdShown = true;

            onAdSuccess = onSuccess;

            HCAnalyticsManager.OnAdPressed(AdType.RewardedVideo, nameInAnalytics, SuperballAdsManager.instance.rewarded.available);

            rewarded.Show(success =>
            {
                AdShown = false;

                if (success)
                {
                    onAdSuccess?.Invoke();

                    HCAnalyticsManager.OnAdShowingFinished(AdType.RewardedVideo, nameInAnalytics, "watched");
                }

                onAdSuccess = null;
            });
        }

        protected override void OnInterstitialHidden()
        {
            base.OnInterstitialHidden();

            ResetInterstitialTimer();
        }

        protected override void OnRewardedHidden(bool success)
        {
            base.OnRewardedHidden(success);

            onAdFailed?.Invoke();
            AdShown = false;
            ResetInterstitialTimer();
        }

        private static long GetTotalSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        private void ResetInterstitialTimer()
        {
            _lastInterstitialSecondsTimestamp = GetTotalSeconds(DateTime.UtcNow);
        }
#endif
    }

}