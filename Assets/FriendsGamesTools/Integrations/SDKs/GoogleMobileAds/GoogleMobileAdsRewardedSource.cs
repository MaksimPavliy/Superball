#if GOOGLE_MOBILE_ADS
using FriendsGamesTools.Ads;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace FriendsGamesTools.Integrations.GoogleMobileAds
{
    public class GoogleMobileAdsRewardedSource : IRewardedVideoSource
    {
        GoogleMobileAdsModuleSettings settings => GoogleMobileAdsModuleSettings.instance;
        private RewardedAd rewardedAd;
        public GoogleMobileAdsRewardedSource()
        {
            StartLoading();
        }
        void StartLoading()
        {
            if (rewardedAd != null)
                Unsubscribe();
            rewardedAd = new RewardedAd(settings.currPlatform.rewardedAdUnitId); // Doc says this class should be recreated each ad load.
            Subscribe();
            rewardedAd.LoadAd(GoogleMobileAdsManager.CreateAdRequest());
        }
        public bool available => !isShowing && (rewardedAd?.IsLoaded() ?? false);
        Action<bool> onCompleted;
        bool isShowing;
        public void Show(Action<bool> onCompleted)
        {
            if (!available)
            {
                onCompleted?.Invoke(false);
                return;
            }
            isShowing = true;
            this.onCompleted = onCompleted;
            rewardedAd.Show();
        }
        void OnShowingFinished(bool success)
        {
            isShowing = false;
            var onCompleted = this.onCompleted;
            this.onCompleted = null;
            onCompleted?.Invoke(success);
            StartLoading();
        }

        #region Google's callbacks
        void Subscribe()
        {
            rewardedAd.OnAdClosed += OnAdClosed;
            rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
            rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
            rewardedAd.OnAdLoaded += OnAdLoaded;
            rewardedAd.OnAdOpening += OnAdOpening;
            rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
        }



        void Unsubscribe()
        {
            rewardedAd.OnAdClosed -= OnAdClosed;
            rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
            rewardedAd.OnAdFailedToShow -= OnAdFailedToShow;
            rewardedAd.OnAdLoaded -= OnAdLoaded;
            rewardedAd.OnAdOpening -= OnAdOpening;
        }
        private void OnAdOpening(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnAdOpening");
        }
        private void OnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnAdLoaded");
        }
        private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnAdFailedToShow");
            OnShowingFinished(false);
        }
        private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnAdFailedToLoad");
            if (isShowing)
                OnShowingFinished(false);
        }
        private void OnAdClosed(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnAdClosed");
            if (isShowing)
                OnShowingFinished(false);
        }
        private void OnUserEarnedReward(object sender, Reward e)
        {
            Debug.Log("GoogleMobileAdsRewardedSource.OnUserEarnedReward");
            if (isShowing)
                OnShowingFinished(true);
        }
        #endregion
    }
}
#endif