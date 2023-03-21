using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_UNLOCK_HowTo : HowToModule
    {
        public override string forWhat => "unlocking something in game";
        protected override void OnHowToGUI()
        {
            Locked.ShowOnGUI("To make entity locked add this component on it");
            GUILayout.Space(5);
            UnlockController.ShowOnGUI("Write any unlock logic in class derived from this",
                "Define when unlock is available in <b>GetAvailablePrivate()</b>\n" +
                "Subtract money in <b>Unlock()</b> override");
            GUILayout.Space(5);
            UnlockView.ShowOnGUI("Show unlock UI - derive it from this class",
                "Tell it what entity to show in <b>entity</b> override\n" +
                "Feed your controller into <b>controller</b> override\n" +
                "If you need any custom view logic, put it in <b>UpdateView</b> override");
            GUILayout.Space(20);
        }
        protected override string docsURL => "https://docs.google.com/document/d/1kIF7sD2cjmO0rg_Gt9wEP20J2nKaeeflFVKzn0kRBgs/edit?usp=sharing";

        ExampleScript Locked = new ExampleScript("Locked");
        ExampleScript UnlockController = new ExampleScript("UnlockController");
        ExampleScript UnlockView = new ExampleScript("UnlockView");
    }
}


