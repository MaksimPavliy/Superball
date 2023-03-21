using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_PLAYER_LEVEL_HowTo : HowToModule
    {
        public override string forWhat => "managing player level and experience";
        protected override void OnHowToGUI()
        {
            PlayerLevelController.ShowOnGUI("derive your level controller from this class",
                "Override <b>GetExpForNextLevel(currLevel)</b> to set experience increasing law\n" +
                "Override <b>ActivateLevelUp(multiplier)</b> to do anything when level increases\n" +
                "Call <b>AddExp(exp)</b> to add experience to player. Override it to write any custom logic\n" +
                "call <b>DebugAddLevel(count)</b> if you need it for debug");
            GUILayout.Space(5);
            PlayerLevelView.ShowOnGUI("put this class to UI to show level and experience");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://docs.google.com/document/d/1Oy2aUk-LD5ZD0pe3N6vRQWyW9s-WxCAXKvcc3_vXQBg/edit?usp=sharing";
        ExampleScript PlayerLevelController = new ExampleScript("PlayerLevelController");
        ExampleScript PlayerLevelView = new ExampleScript("PlayerLevelView");
    }
}


