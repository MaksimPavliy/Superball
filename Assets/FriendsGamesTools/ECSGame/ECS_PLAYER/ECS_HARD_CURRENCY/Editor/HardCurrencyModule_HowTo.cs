using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class HardCurrencyModule_HowTo : HowToModule
    {
        public override string forWhat => "hard currency";
        protected override void OnHowToGUI()
        {
            HardCurrencyController.ShowOnGUI("derive your game hard currency controller from this class",
                "<b>AddMoney(income)</b> and <b>PayMoney(income)</b> throughout the game\n" +
                "call <b>DebugMultiply(multiplier)</b> if you need it for debug\n");
            HardCurrencyView.ShowOnGUI("put this class to UI to show hard currency and income");
            GUILayout.Space(10);
        }
        protected override string docsURL => "";
        ExampleScript HardCurrencyController = new ExampleScript("HardCurrencyController");
        ExampleScript HardCurrencyView = new ExampleScript("HardCurrencyView");
    }
}


