using UnityEngine;

namespace FriendsGamesTools.Analytics
{
    public class AnalyticsModule_HowTo : HowToModule
    {
        public override string forWhat => "sending events to different analytics sdk";
        protected override void OnHowToGUI()
        {
            AnalyticsManager.ShowOnGUI("Put this to scene",
                "Call <b>AnalyticsManager.Send(eventName)</b> to send data\n" +
                "Also AnalyticsManager.Send(eventName, (paramName, paramValue), ...) available");
        }
        protected override string docsURL => "https://docs.google.com/document/d/11tyawkdXta3q8WSXp6d0Eo14F1h4uFFZssJJ4_TMgzM/edit?usp=sharing";
        ExampleScript AnalyticsManager = new ExampleScript("AnalyticsManager");
    }
}


