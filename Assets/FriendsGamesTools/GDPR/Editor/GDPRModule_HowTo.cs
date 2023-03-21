namespace FriendsGamesTools
{
    public class GDPRModule_HowTo : HowToModule
    {
        public override string forWhat => "get GDPR conscent from user";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("Automatically supports only <b>MAX SDK</b>, ask developer to add any other sdk");

            GDPRWindow.ShowOnGUI(
                "call <b>GDPRWindow.ShowIfNeeded()</b> to ask for conscent",
                "Window shows only once");
        }
        protected override string docsURL => "";
        ExampleScript GDPRWindow = new ExampleScript("GDPRWindow");
    }
}