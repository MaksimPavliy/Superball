using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_GAME_TIME_HowTo : HowToModule
    {
        public override string forWhat => "to react on player returned from offline, to know how much time player wasted on real life";
        protected override void OnHowToGUI()
        {
            GameTimeController.ShowOnGUI("Derive your time controller from this script",
                "Override <b>ApplyOfflineTime(seconds, type)</b> to react to player returning from offline\n" +
                "<b>time</b> property tells how long player plays this game\n" +
                "<b>totalOnlineTime</b> and <b>totalOfflineTime</b> are also available");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1ScQG8Bczl-lQVafVgmBTy8iMYkg9Lr6S1Si_qq4WtRk";
        ExampleScript GameTimeController = new ExampleScript("GameTimeController");
    }
}


