#if APP_LOVIN
using FriendsGamesTools.Ads;
using System;

namespace FriendsGamesTools.Integrations
{
    public class AppLovinInterstitial : IInterstitialSource
    {
        public bool available 
            => AppLovinManager.instance.editorTestModeEnabled || AppLovin.HasPreloadedInterstitial();
        public bool isShowing { get; private set; }
        public void Init() => StartLoading();
        Action onCompleted;
        public void Show(Action onCompleted)
        {
            if (AppLovinManager.instance.editorTestModeEnabled)
            {
                onCompleted?.Invoke();
                return;
            }
            this.onCompleted = onCompleted;
            AppLovin.ShowInterstitial();
        }
        public void StartLoading() => AppLovin.PreloadInterstitial();
        void OnShowingCompleted()
        {
            isShowing = false;
            onCompleted?.Invoke();
            onCompleted = null;
        }
        public void RewardedVideoAppLovinEventReceived(string eventName)
        {
            if (eventName.Contains("HIDDENINTER"))
            {
                StartLoading();
                OnShowingCompleted();
            }
        }
    }
}
#endif