#if GA
#if ANALYTICS
using FriendsGamesTools.Analytics;
#endif
using GameAnalyticsSDK;
using System;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    [ExecuteAlways]
    public class GAManager : IntegrationManager<GAManager>
    {
        void OnEnable()
        {
            if (!Application.isPlaying)
            {
                const string prefabName = "GameAnalytics";
                var inst = transform.Find(prefabName)?.gameObject ?? null;
                if (inst == null)
                {
                    var prefab = AssetByTypeAndName.Find<GameObject>(prefabName, TypeFilterName.Prefab, false);
                    inst = Instantiate(prefab, transform);
                    inst.name = prefabName;
                }
            }
        }
        void Start()
        {
            GameAnalytics.Initialize();
            ready = true;

            GameAnalytics.SetBuildAllPlatforms(Application.version);
        }
        public bool ready { get; private set; }

#if ANALYTICS
        public class GAAnalytics : Analytics.AnalyticsSource
        {
            public override bool ready => GAManager.instance.ready;
            public override string ModuleName => "GA";
            public override void Send(string eventName, params (string key, object value)[] parameters)
            {
                if (parameters == null || parameters.Length == 0)
                    GameAnalytics.NewDesignEvent(eventName);
                else
                {
                    // Try use sending event with one number.
                    if (parameters.Length == 1)
                    {
                        var val = parameters[0].value;
                        if (val.GetType() == typeof(int) || val.GetType() == typeof(float) || val.GetType() == typeof(double))
                        {
                            var valNumber = Convert.ToSingle(val);
                            GameAnalytics.NewDesignEvent($"{eventName}:{parameters[0].key}", valNumber);
                            return;
                        }
                    }
                    // Send general event.
                    var paramsText = parameters.ConvertAll(p => $":({p.key}={p.value})").PrintCollection();
                    GameAnalytics.NewDesignEvent($"{eventName}{paramsText}");
                }
            }
        }
        protected override AnalyticsSource GetAnalytics() => new GAAnalytics();
#endif
    }
}
#elif SDKs
namespace FriendsGamesTools.Integrations
{
    public class GAManager : IntegrationManager<GAManager>
    {
        public void Send(string eventName, params (string key, object value)[] parameters) { }
    }
}
#endif