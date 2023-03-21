#if ANALYTICS
using FriendsGamesTools.Analytics;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Ads
{
    public class WatchAdButtonView : MonoBehaviour
    {
        [SerializeField] protected AdType type;
        public Button button;
        [SerializeField] GameObject availableParent;
        [SerializeField] GameObject notAvailableParent;
        public string nameInAnalytics;
#if ADS
        AdsManager ads => AdsManager.instance;
        public virtual bool available => (type == AdType.Interstitial ? ads.interstitial.available : ads.rewarded.available) && !ads.isShowingAd;
        void UpdateAvailable()
        {
            var available = this.available;
            if (button != null)
                button.interactable = available;
            if (availableParent != null)
                availableParent.SetActive(available);
            if (notAvailableParent != null)
                notAvailableParent.SetActive(!available);
        }
        protected virtual void FixedUpdate() => UpdateAvailable();
        protected virtual void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnWatchPressed);
        }
        protected Action<bool> onWatchFinished;
        public void SubscribeAdWatched(Action onWatched)
            => onWatchFinished += success =>
            {
                if (success) onWatched?.Invoke();
            };
        public void SubscribeAdWatched(Action<bool> onWatchFinished)
            => this.onWatchFinished += onWatchFinished;
        Action onWatchAdPressed;
        public void SubscribeWatchAdPressed(Action onWatchAdPressed)
            => this.onWatchAdPressed += onWatchAdPressed;

        protected virtual void InterstitialShow()
        {
            ads.interstitial.Show(() => 
            { 
                onWatchFinished?.Invoke(true);

#if ANALYTICS
                AnalyticsEventManager.instance.OnAdShowingFinished(AdType.Interstitial.ToString(), nameInAnalytics, "watched");
#endif
            });
        }

        protected virtual void RewardedShow()
        {
#if ANALYTICS
            AnalyticsEventManager.instance.OnAdPressed(AdType.RewardedVideo.ToString(), nameInAnalytics, AdsManager.instance.rewarded.available);
#endif
            ads.rewarded.Show(success =>
            {
                if (success)
                {
                    onWatchFinished?.Invoke(true);

#if ANALYTICS
                    AnalyticsEventManager.instance.OnAdShowingFinished(AdType.RewardedVideo.ToString(), nameInAnalytics, "watched");
#endif
                }
                else
                    onWatchFinished?.Invoke(false);
            });
        }

        private void OnWatchPressed()
        {
            onWatchAdPressed?.Invoke();
            if (!available)
                return;
            if (type == AdType.Interstitial)
                InterstitialShow();
            else
                RewardedShow();
        }
#else
        public void SubscribeAdWatched(Action<bool> onWatchFinished) { }
        public void SubscribeAdWatched(Action onWatchFinished) { }
#endif
    }
}
