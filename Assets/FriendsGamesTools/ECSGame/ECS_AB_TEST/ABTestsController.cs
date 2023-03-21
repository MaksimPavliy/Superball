#if ECS_AB_TEST
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.Analytics;
using System.Collections.Generic;
using Unity.Entities;

namespace FriendsGamesTools.ABTests
{
    public struct ABTest : IComponentData { bool ignored; }
    [InternalBufferCapacity(8)]
    public struct ABTestData : IBufferElementData { public int value; }
    public sealed partial class ABTestsController : Controller
    {
        public static ABTestsController instance;
        public override void OnInited()
        {
            instance = this;
            base.OnInited();
        }
        public ABTestsConfig config => ABTestsConfig.instance;
        public Entity e => ECSUtils.GetSingleEntity<ABTest>();
        public DynamicBuffer<ABTestData> data => e.GetBuffer<ABTestData>();
        public List<string> eventNames
        {
            get
            {
                var names = new List<string>();
                var data = e.GetBuffer<ABTestData>();
                for (int i = 0; i < config.tests.Count; i++)
                {
                    var value = data[i].value;
                    var eventName = config.tests[i].GetOptionName(value);
                    names.Add(eventName);
                }
                return names;
            }
        }
        public override void InitDefault()
        {
            base.InitDefault();
            var e = ECSUtils.CreateEntity(new ABTest { });
            e.AddBuffer<ABTestData>();
            var data = e.GetBuffer<ABTestData>();
            for (int i = 0; i < config.tests.Count; i++)
                data.Add(new ABTestData { value = GenerateEventInd(config.tests[i]) });
            eventNames.ForEach(eventName => SendToAnalytics(eventName));
        }
        public int GenerateEventInd(TestConfig test)
        {
            if (test.enabled)
                return Utils.RandomFromProbabilities(test.options.ConvertAll(e => (float)e.optionMass).ToArray());
            else
                return test.disabledAlwaysEventInd;
        }
        private void SendToAnalytics(string eventName) => AnalyticsManager.Send(eventName);
    }
}
#endif
