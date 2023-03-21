#if APPS_FLYER
using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class AppsFlyerManager : IntegrationManager<AppsFlyerManager>
    {
        void Start()
        {
            AppsFlyer.initSDK(AppsFlyerSettings.instance.APPSFLYER_DEV_KEY, FGTSettings.instance.appleAppId, this);
            AppsFlyer.startSDK();
        }


#if ANALYTICS
        public class AppsFlyerAnalytics : Analytics.AnalyticsSource
        {
            public override bool ready => true;
            public override string ModuleName => "APPS_FLYER";
            public override void Send(string eventName, params (string key, object value)[] parameters) {
                var dict = new Dictionary<string, string>();
                if (parameters != null) {
                    foreach (var (key, value) in parameters)
                        dict.Add(key, value.ToString());
                }
                AppsFlyer.sendEvent(eventName, dict);
            }
        }
        protected override Analytics.AnalyticsSource GetAnalytics() => new AppsFlyerAnalytics();
#endif
    }
}
#elif SDKs
namespace FriendsGamesTools.Integrations
{
    public class AppsFlyerManager : IntegrationManager<AppsFlyerManager> { }
}
#endif