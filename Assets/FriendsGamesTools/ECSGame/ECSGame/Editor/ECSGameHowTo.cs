using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECSGameHowTo : HowToModule
    {
        public override string forWhat => "to write similar idle games domain logic faster and in unified way";
        protected override void OnHowToGUI()
        {
            iosHotFix.ShowOnGUI("take this prefab to your scene. Its temporary IOS hotfix");
            EditorGUIUtils.RichMultilineLabel(
                $"Add submodules usable for your game\n" +
                $"Perhaps <b>ECS_GAMEROOT</b> will be a good start");
            GUILayout.Space(10);
            ItemsView.ShowOnGUI("To make human prefabs instantiate and destroy with their entites, derive from this",
                "<b>GetPrefabInd(e)</b> override allows you to select from different prefabs\n" +
                "<b>UpdateView()</b> override tells how to show single prefab with its data");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1DzcCkVsUICnDYsgrhzrv4iytACKfgyqPsh3NLSE0qa4/edit?usp=sharing";

        ExamplePrefab iosHotFix = new ExamplePrefab("HotfixIOS");
        ExampleScript ItemsView = new ExampleScript("ItemsView");
    }
}