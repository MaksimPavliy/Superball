using FriendsGamesTools.Ads;
using UnityEngine;

namespace Superball
{
    public class SuperballWatchAdButtonView : WatchAdButtonView
    {
        public GameObject adLoadingParent;
#if ADS
        protected override void InterstitialShow()
        {
            SuperballAdsManager.instance.ShowInterstitial(nameInAnalytics.ToString(),
                () => onWatchFinished.Invoke(true),
                () => onWatchFinished.Invoke(false));
        }

        protected override void RewardedShow()
        {
            SuperballAdsManager.instance.ShowRewarded(nameInAnalytics.ToString(),
                 () => onWatchFinished.Invoke(true));
        }

        private void Update()
        {
            if (adLoadingParent != null)
                adLoadingParent.gameObject.SetActive(!available);
        }
#endif
    }
}