#if SDKs
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public abstract class IntegrationManager<TSelf> : MonoBehaviourHasInstance<TSelf>
        where TSelf : IntegrationManager<TSelf>
    {
#if ANALYTICS
        protected virtual FriendsGamesTools.Analytics.AnalyticsSource GetAnalytics() => null;
        public IntegrationManager() => GetAnalytics();
#endif

        protected virtual bool destroyIfAdsNotSelected => false;
        protected override void Awake() {
#if ADS
            if (destroyIfAdsNotSelected && Application.isPlaying && (Ads.AdsManager.adsSourceSDK != (Ads.IAdsSource)this)) {
                Destroy(gameObject);
                return;
            }
#endif
            base.Awake();
        }
    }
}
#endif