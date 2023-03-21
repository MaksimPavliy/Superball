using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_BONUS_HowTo : HowToModule
    {
        public override string forWhat => "random bonus events";
        protected override void OnHowToGUI()
        {
            GUILayout.Label("Create any component to identify your event");
            GUILayout.Space(10);
            BonusEventController.ShowOnGUI("derive your bonus event controller from this",
                "Use component data <b>FlagType</b> to identify your bonus\n" +
                "Override <b>appearInterval</b> to tell how long player should wait this bonus to appear\n" +
                "Override <b>activationWaiting</b> to tell how long its appeared and waiting for player to activate it\n" +
                "You can override <b>activatedDuration</b> and <b>OnUpdate(active)</b> to write durable bonus effect\n" + 
                "Or just override <b>OnActivated()</b> and give instant bonus"
                );
            GUILayout.Space(10);
            BonusEventView.ShowOnGUI("derive your view script from this",
                "Set <b>TBonus</b> as your identifier component\n" +
                "<b>TAds</b> (optional) to show rewarded ads before giving bonus" +
                "Set <b>duration</b> label to show how long bonus wil be active\n" +
                "and <b>AppearedParent</b> GameObject to activate when bonus is available\n" +
                "Call <b>Activate()</b> to make bonus active");
            GUILayout.Space(20);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1h64TGUkkFtmep_bi9mzz8wz3RPcINLITbZwJK2_i3to";

        ExampleScript BonusEventController = new ExampleScript("BonusEventController");
        ExampleScript BonusEventView = new ExampleScript("BonusEventView");
    }
}


