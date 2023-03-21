using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_UPGRADABLE_QUALITY_HowTo : HowToModule
    {
        public override string forWhat => "level and/or quality upgrades";
        protected override void OnHowToGUI()
        {
            QualityUpgradable.ShowOnGUI("To make entity upgradable add this component on it");
            GUILayout.Space(5);
            QualityUpgradableController.ShowOnGUI("Derive your controller from this",
                "Override <b>GetMaxLevel(e)</b>\n" +
                "Optionally override GetPrice(e), GetAvailablePrivate(), Upgrade(), moneyToExpCoef etc");
            GUILayout.Space(5);
            QualityUpgradableView.ShowOnGUI("Show upgrading UI - derive it from this class",
                "Tell it what entity to show in <b>entity</b> override\n" +
                "Feed your controller into <b>controller</b> override\n" +
                "If you need any custom view logic, put it in <b>UpdateView</b> override");
            GUILayout.Space(20);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1YHWdVEe9j2zTXyWVRaKQXJNxQjXaESvGiPvwfLdvFwU";

        ExampleScript QualityUpgradable = new ExampleScript("QualityUpgradable");
        ExampleScript QualityUpgradableController = new ExampleScript("QualityUpgradableController");
        ExampleScript QualityUpgradableView = new ExampleScript("QualityUpgradableView");
    }
}