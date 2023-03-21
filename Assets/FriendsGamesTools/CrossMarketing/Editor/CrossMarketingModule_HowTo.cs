namespace FriendsGamesTools
{
    public class CrossMarketingModule_HowTo : HowToModule
    {
        public override string forWhat => "Adding cross links to our games in an easy way";
        protected override void OnHowToGUI() {
            //EditorGUIUtils.RichMultilineLabel($"Add games to a <B>list</B>, setup store links and icons. List will be shared between all games with FGT submodule.");
            //GUILayout.Space(10);
            //EditorGUIUtils.RichMultilineLabel($"Set <B>Shown</B> checkbox if you want a game to be shown in <b>CrossMarketingView</b>.");
            //EditorGUIUtils.RichMultilineLabel($"<B>Shown</B> checkbox affects only current game in development.");

            //GUILayout.Space(10);
            //EditorGUIUtils.RichMultilineLabel($"<b>Don't forget to save your changes!</b>");

            //GUILayout.Space(10);
            CrossMarketingView.ShowOnGUI("Put a prefab with <b>CrossMarketingView</b> to a scene inside a canvas",
                    "It will show a random available game from a list\n" +
                    "Optionally you can call <b>ShowRandomGame()</b> to refresh shown game manually");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1_omqo1TXP3jG83mIuwmfZyLPB2EPR5XqfIwyPMvYwsk/edit?usp=sharing";
        ExamplePrefab CrossMarketingView = new ExamplePrefab("CrossMarketingView");
    }
}