namespace FriendsGamesTools
{
    public class SkinsModule_HowTo : HowToModule
    {
        public override string forWhat => "Some items that can be unlocked and activated";
        protected override void OnHowToGUI()
        {
            SkinsWindow.ShowOnGUI("put under windows on scene");
            EditorGUIUtils.RichMultilineLabel($"implement <b>{MoneySkinsModule.define}</b> and/or <b>{ProgressSkinsModule.define}</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1bh2Xfdk8IW1UiBy1Pg_MxMWfNC8CuxEjodQ_Me-Npgc/edit?usp=sharing";
        ExampleScript SkinsWindow = new ExampleScript("SkinsWindow");
    }
}