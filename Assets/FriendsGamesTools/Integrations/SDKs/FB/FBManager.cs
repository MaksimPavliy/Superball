#if FACEBOOK
using Facebook.Unity;
using System.Collections.Generic;

namespace FriendsGamesTools.Integrations
{
    public class FBManager : IntegrationManager<FBManager>
    {
        public bool ready { get; private set; }
        void Start() => FB.Init(() => ready = true);

#if ANALYTICS
        public class FBAnalytics : Analytics.AnalyticsSource
        {
            public override bool ready => FBManager.instance.ready;
            public override string ModuleName => "FACEBOOK";
            public override void Send(string eventName, params (string key, object value)[] parameters)
            {
                var dict = new Dictionary<string, object>();
                if (parameters != null)
                {
                    foreach (var (key, value) in parameters)
                        dict.Add(key, value);
                }
                FB.LogAppEvent(eventName, null, dict);
            }
        }
        protected override Analytics.AnalyticsSource GetAnalytics() => new FBAnalytics();
#endif
    }
}
#elif SDKs
namespace FriendsGamesTools.Integrations
{
    public class FBManager : IntegrationManager<FBManager>
    {
        public void Send(string eventName, params (string key, object value)[] parameters) { }
    }
}
#endif