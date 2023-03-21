namespace FriendsGamesTools.EditorTools.Screenshots
{
    public class ScreenshotsModule_HowTo : HowToModule
    {
        public override string forWhat => "taking screenshots from different simulated devices for appstores in editor";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel(
                "Screenshots are made in different resolutions and simulated notches\n" +
                "Remember to <b>leave this window opened while doing screenshots</b>");
            //"For 5.5-inch (iPhone(6,7,8)(S)+ devices) resolutions are Rendered (bigger than screen) as AppStore need them");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1ICAaS0lsWAINdHl-5zbBh3vCDfc9l3KCixLVWc1V6PM/edit?usp=sharing";
    }
}
