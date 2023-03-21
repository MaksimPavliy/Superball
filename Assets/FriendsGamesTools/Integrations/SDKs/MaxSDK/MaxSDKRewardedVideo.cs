#if MAX_SDK
using System;
using FriendsGamesTools.Ads;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKRewardedVideo : IRewardedVideoSource
    {
        private string rewardedAdUnitId;
        MaxSDKManager manager => MaxSDKManager.instance;
        public bool available
            => MaxSDKManager.internet && manager.inited && manager.rewardedVideoUsed && MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
        public bool isShowing { get; private set; }
        private bool logs => MaxSDKManager.instance.logging;
        public static MaxSDKRewardedVideo instance { get; private set; }
        private Action<bool> onCompleted;

        public MaxSDKRewardedVideo(string rewardedAdUnitId)
        {
            instance = this;
            this.rewardedAdUnitId = rewardedAdUnitId;

            Init();
        }

        private void Init()
        {
            AssertRewardedEnabled();
            if (logs)
                Debug.Log($"MaxSDKRewardedVideo.Init rewardedAdUnitId={rewardedAdUnitId}");

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            StartLoading();
        }

        private void AssertRewardedEnabled()
        {
            Debug.Assert(MaxSDKManager.instance.rewardedVideoUsed, "MaxSDK rewarded ads are turned off in max sdk");
        }

        public void StartLoading()
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.StartLoading");

            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        public void Show(Action<bool> onCompleted)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.ShowRewardedVideo");

            AssertRewardedEnabled();
            this.onCompleted = onCompleted;

            if (!MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
            {
                if (logs) Debug.Log($"MaxSDK video not ready");

                onCompleted?.Invoke(false);
                this.onCompleted = null;
                return;
            }

            if (logs) Debug.Log($"MaxSDKRewardedVideo.video ready");

            isShowing = true;
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
#if UNITY_EDITOR
            if (logs) Debug.Log($"MaxSDK video showing considered instantly complete in unity editor");

            onCompleted?.Invoke(true);
            this.onCompleted = null;
#endif
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdLoadedEvent {adUnitId}");
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        }

        private async void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdFailedEvent: {adUnitId}, code: {info.Code}, message: {info.Message}");

            // Rewarded ad failed to load. We recommend re-trying in 3 seconds.
            await Awaiters.SecondsRealtime(3);

            StartLoading();
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdFailedToDisplayEvent {adUnitId}, code: {errorInfo.Code}, message: {errorInfo.Message}");

            // Rewarded ad failed to display. We recommend loading the next ad
            ShowingComplete(false);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdDisplayedEvent {adUnitId}");
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdClickedEvent {adUnitId}");
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdDismissedEvent {adUnitId}");

            // Rewarded ad is hidden. Pre-load the next ad
            ShowingComplete(false);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo info)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.OnRewardedAdReceivedRewardEvent {adUnitId}");

            // Rewarded ad was displayed and user should receive the reward
            ShowingComplete(true);
        }

        private void ShowingComplete(bool success)
        {
            if (logs) Debug.Log($"MaxSDKRewardedVideo.ShowingComplete success={success}");

            isShowing = false;
            onCompleted?.Invoke(success);
            onCompleted = null;
            StartLoading();
        }
    }
}
#endif