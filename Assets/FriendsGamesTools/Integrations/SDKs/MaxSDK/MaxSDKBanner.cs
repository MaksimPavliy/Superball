#if MAX_SDK
using FriendsGamesTools.Ads;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKBanner : IBannerSource
    {
        private string adUnitId;
        private MaxSDKManager manager => MaxSDKManager.instance;
        private bool logs => MaxSDKManager.instance.logging;
        public bool available => MaxSDKManager.internet && manager.bannerAdUsed;

        public MaxSDKBanner(string adUnitId)
        {
            this.adUnitId = adUnitId;

            Init();
        }

        private void Init()
        {
            AssertBannerEnabled();

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += this.Banner_OnAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += this.Banner_OnAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += this.Banner_OnAdClickedEvent;
        }

        private void AssertBannerEnabled()
        {
            Debug.Assert(MaxSDKManager.instance.bannerAdUsed, "MaxSDK banner ads are turned off in max sdk");
        }

        private void StartLoading()
        {
            if (logs) Debug.Log($"MaxSDK banner StartLoading");

            MaxSdk.CreateBanner(adUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.ShowBanner(adUnitId);
        }

        private void Banner_OnAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            if (logs) Debug.Log($"MaxSDK Banner_OnAdClickedEvent");
        }

        private void Banner_OnAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
        {
            // Banner ad failed to load. MAX will automatically try loading a new ad internally.
            if (logs) Debug.Log($"MaxSDK Banner_OnAdLoadFailedEvent with error code: " + arg2.Code);
        }

        private void Banner_OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            // Banner ad is ready to be shown.
            // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
            if (logs) Debug.Log($"MaxSDK Banner_OnAdLoadedEvent");
        }

        public void Show()
        {
            if (logs) Debug.Log($"MaxSDK Banner_Show");

            StartLoading();
        }

        public void Hide()
        {
            if (logs) Debug.Log($"MaxSDK Banner_Hide");

            MaxSdk.HideBanner(adUnitId);
        }

        public void Destroy()
        {
            if (logs) Debug.Log($"MaxSDK Banner_Destroy");

            MaxSdk.DestroyBanner(adUnitId);
        }
    }
}
#endif