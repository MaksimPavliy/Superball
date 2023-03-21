using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DEBUG_PANEL_HowTo : HowToModule
    {
        protected override string docsURL => "https://docs.google.com/document/d/1NEEh4X2esVt3UajzWCBOf3yyybOoZl2ZLKDZl5DAjSs/edit?usp=sharing";
        public override string forWhat => "debug panel";

        protected override void OnHowToGUI()
        {
            DebugPanel.ShowOnGUI("Drag this prefab to your UI");
            DebugPanelItemView.ShowOnGUI("You can add your prefabs to debug panel",
                "Derive from this script and put it to prefab");
        }
        ExamplePrefab DebugPanel = new ExamplePrefab("DebugPanel");
        ExampleScript DebugPanelItemView = new ExampleScript("DebugPanelItemView");
    }
}