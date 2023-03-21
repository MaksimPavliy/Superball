#if QUESTS
namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class EventsForQuests : Analytics.AnalyticsSource
    {
        public static EventsForQuests instance { get; private set; }
        public static void Init()
        {
            if (instance == null)
                instance = new EventsForQuests();
        }
        public override bool ready => true;
        public override string ModuleName => "QUESTS";
        public override void Send(string eventName, params (string key, object value)[] parameters)
        {
            GameRoot.instance.Get<QuestsController>().currQuest?.OnEvent(eventName, parameters);
        }
    }
}
#endif