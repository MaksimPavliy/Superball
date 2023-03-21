namespace FriendsGamesTools
{
    public class SettingsModule_HowTo : HowToModule
    {
        public override string forWhat => "sound, vibro, other settings";
        protected override void OnHowToGUI()
        {
            OpenSettings.ShowOnGUI("Add this script to settings button");
            SettingsWindow.ShowOnGUI("Put this script to your custom window", "Then drag pregab to 'settings window prefab' below");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1PnXfvl5BvIwHJPr8ZcXMAlmKGMIKrTjww5HqG9oX-sU/edit?usp=sharing";
        ExampleScript OpenSettings = new ExampleScript("OpenSettings");
        ExampleScript SettingsWindow = new ExampleScript("SettingsWindow");
    }
}