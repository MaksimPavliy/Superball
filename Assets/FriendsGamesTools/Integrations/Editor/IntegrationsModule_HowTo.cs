namespace FriendsGamesTools.Integrations
{
    public class IntegrationsModule_HowTo : HowToModule
    {
        public override string forWhat => "adding different SDK for ads, analytics etc";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel(
                "For using ads, enable ADS module and SDK module for needed integration");
        }
        protected override string docsURL => "";
    }
}