using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_PLAYER_MONEY_HowTo : HowToModule
    {
        public override string forWhat => "player money in game, calculating income per minute";
        protected override void OnHowToGUI()
        {
            PlayerMoneyController.ShowOnGUI("derive your game money controller from this class",
                "call <b>SetStartMoney(amount)</b> when initing new player\n" +
                "<b>AddMoney(income)</b> and <b>PayMoney(income)</b> throughout the game\n" +
                "call <b>DebugMultiply(multiplier)</b> if you need it for debug\n" +
                "use <b>incomeMultiplier</b> to know how much money player get for 1 income added using AddMoney()\n" +
                "override <b>incomeAdditionalMultiplier</b> to apply custom multipliers");
            PlayerMoneyView.ShowOnGUI("put this class to UI to show money and income");
            IncomeMultiplierView.ShowOnGUI("use this script to show income durable multiplier",
                "Call PlayerMoneyController.<b>AddMultiplier(2, 60)</b> to multiply player income x2 for 60 seconds\n" +
                "(Requires also ECS_GAME_TIME module, because it ticks offline)");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://docs.google.com/document/d/1qOW9jTIpvkKqQr5hkiGCzO-4jxPk6IcaYhzkxJslxc8/edit?usp=sharing";
        ExampleScript PlayerMoneyController = new ExampleScript("PlayerMoneyController");
        ExampleScript PlayerMoneyView = new ExampleScript("PlayerMoneyView");
        ExampleScript IncomeMultiplierView = new ExampleScript("IncomeMultiplierView");
    }
}


