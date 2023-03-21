using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_UPGRADABLE_COUNT_HowTo : HowToModule
    {
        public override string forWhat => "making entites whose count can be upgraded";
        protected override void OnHowToGUI()
        {
            CountUpgradable.ShowOnGUI("To make entity upgradable add this component on it");
            GUILayout.Space(5);
            CountUpgradableController.ShowOnGUI("Write upgrade logic in class derived from this",
                "override <b>GetMaxCount()</b> to tell how many upgrades allowed.\n" +
                "Define when upgrade is available in <b>GetAvailablePrivate()</b>\n" +
                "Define how to create each of items that are counted in <b>CreateCountedItem()</b>\n" +
                "Call <b>InitCountedItems(entity, count)</b> to init items count while creating player data" +
                "Subtract money in <b>Upgrade()</b> override");
            GUILayout.Space(5);
            CountUpgradableView.ShowOnGUI("Show upgrading UI - derive it from this class",
                "Tell it what entity to show in <b>entity</b> override\n" +
                "Feed your controller into <b>controller</b> override\n" +
                "If you need any custom view logic, put it in <b>UpdateView</b> override");
            GUILayout.Space(20);
        }
        protected override string docsURL => "https://docs.google.com/document/d/1bPH4es-rxnd-qu6Uhu_GSiOPSFWcYQ1yJ2DAYGStB_A/edit?usp=sharing";

        ExampleScript CountUpgradable = new ExampleScript("CountUpgradable");
        ExampleScript CountUpgradableController = new ExampleScript("CountUpgradableController");
        ExampleScript CountUpgradableView = new ExampleScript("CountUpgradableView");
    }
}


