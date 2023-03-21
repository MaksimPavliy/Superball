namespace FriendsGamesTools.Integrations
{
    public class MaxSDKSetupManager_HowTo : HowToModule
    {
        public override string forWhat => "MaxSDK sdk";
        protected override void OnHowToGUI()
        {
            MaxSDKManager.ShowOnGUI("download plugin, <b>restart Unity</b> and put this script to the game",
                "setup MAX SDK key and <b>build and launch game on ios and/or android</b> to get max sdk initialized\n" +
                "Then ask Volodya to create 'ad unit id's for the game in MAX SDK dashboard" +
                "MAX SDK is a wrapper around lots of ad network mediations\n" +
                "Download needed ad mediations at <b>Menu->AppLovin->Integration manager</b>\n" +
                "This module can provide ads. Use <b>ADS</b> module to show them\n");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1FovqAhjOxcF02RQ69vZqFNcOi68mXzhWOU-d6jIkmq0/edit?usp=sharing";
        ExampleScript MaxSDKManager = new ExampleScript("MaxSDKManager");
    }
}


