#if GOOGLE_MOBILE_ADS
using FriendsGamesTools.Ads;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace FriendsGamesTools.Integrations.GoogleMobileAds
{
    public class GoogleMobileAdsInterstitialSource : IInterstitialSource
    {
        GoogleMobileAdsModuleSettings settings => GoogleMobileAdsModuleSettings.instance;
        private InterstitialAd interstitialAd;
        public GoogleMobileAdsInterstitialSource()
        {
            StartLoading();
        }
        void StartLoading()
        {
            if (interstitialAd != null)
                Unsubscribe();
            interstitialAd = new InterstitialAd(settings.currPlatform.interstitialAdUnitId); // Doc says this class should be recreated each ad load.
            Subscribe();
            interstitialAd.LoadAd(GoogleMobileAdsManager.CreateAdRequest());
        }

        public bool available => !isShowing && (interstitialAd?.IsLoaded() ?? false);
        Action onCompleted;
        bool isShowing;
        public void Show(Action onCompleted)
        {
            if (!available)
            {
                onCompleted?.Invoke();
                return;
            }
            isShowing = true;
            this.onCompleted = onCompleted;
            interstitialAd.Show();
        }
        void OnShowingFinished()
        {
            isShowing = false;
            var onCompleted = this.onCompleted;
            this.onCompleted = null;
            onCompleted?.Invoke();
            StartLoading();
        }

        #region Google's callbacks
        void Subscribe()
        {
            interstitialAd.OnAdClosed += OnAdClosed;
            interstitialAd.OnAdFailedToLoad += OnAdFailedToLoad;
            interstitialAd.OnAdLeavingApplication += OnAdLeavingApplication;
            interstitialAd.OnAdLoaded += OnAdLoaded;
            interstitialAd.OnAdOpening += OnAdOpening;
        }
        void Unsubscribe()
        {
            interstitialAd.OnAdClosed -= OnAdClosed;
            interstitialAd.OnAdFailedToLoad -= OnAdFailedToLoad;
            interstitialAd.OnAdLeavingApplication -= OnAdLeavingApplication;
            interstitialAd.OnAdLoaded -= OnAdLoaded;
            interstitialAd.OnAdOpening -= OnAdOpening;
        }
        private void OnAdOpening(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsInterstitialSource.OnAdOpening");
        }
        private void OnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsInterstitialSource.OnAdLoaded");
        }
        private void OnAdLeavingApplication(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsInterstitialSource.OnAdLeavingApplication");
        }
        private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("GoogleMobileAdsInterstitialSource.OnAdFailedToLoad");
            OnShowingFinished();
        }
        private void OnAdClosed(object sender, EventArgs e)
        {
            Debug.Log("GoogleMobileAdsInterstitialSource.OnAdClosed");
            OnShowingFinished();
        }
        #endregion
    }
}
#endif