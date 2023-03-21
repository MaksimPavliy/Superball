#if MAX_SDK
using FriendsGamesTools.Ads;
using System;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKInterstitial : IInterstitialSource
    {
        private string adUnitId;
        private MaxSDKManager manager => MaxSDKManager.instance;
        public bool available
            => MaxSDKManager.internet && manager.inited && manager.interstitialAdUsed && MaxSdk.IsInterstitialReady(adUnitId);
        
        public bool isShowing { get; private set; }
        private bool logs => MaxSDKManager.instance.logging;
        private Action onCompleted;

        public MaxSDKInterstitial(string adUnitId)
        {
            this.adUnitId = adUnitId;

            Init();
        }

        private void Init()
        {
            AssertInterstitialEnabled();
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

            StartLoading();
        }

        private void AssertInterstitialEnabled()
        {
            Debug.Assert(MaxSDKManager.instance.interstitialAdUsed, "MaxSDK interstitial ads are turned off in max sdk");
        }

        public void StartLoading()
        {
            if (logs) Debug.Log($"MaxSDK interstitial StartLoading");
            MaxSdk.LoadInterstitial(adUnitId);
        }
        
        public void Show(Action onCompleted)
        {
            if (logs) Debug.Log($"MaxSDK ShowInterstitial, available = {available}");

            AssertInterstitialEnabled();
            this.onCompleted = onCompleted;
            isShowing = true;

            if (!available)
                OnShowingStopped();
            else
                MaxSdk.ShowInterstitial(adUnitId);
        }
        void OnShowingStopped()
        {
            if (logs) Debug.Log($"MaxSDK interstitial OnShowingStopped");

            isShowing = false;

            if (onCompleted != null)
            {
                onCompleted();
                onCompleted = null;
            }

            StartLoading();
        }

        #region SDK Callbacks
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            if (logs) Debug.Log($"MaxSDK OnInterstitialLoadedEvent");
        }

        private async void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo info)
        {
            if (logs) Debug.Log($"MaxSDK OnInterstitialFailedEvent: {info.Code}, {info.Message}");

            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
            //Invoke("LoadInterstitial", 3);
            await Awaiters.SecondsRealtime(3);

            StartLoading();
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (logs) Debug.Log($"MaxSDK InterstitialFailedToDisplayEvent: code {errorInfo.Code}, message: {errorInfo.Message}");
            // Interstitial ad failed to display. We recommend loading the next ad
            OnShowingStopped();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDK OnInterstitialDismissedEvent");

            // Interstitial ad is hidden. Pre-load the next ad
            OnShowingStopped();
        }
        #endregion
    }
}
#endif