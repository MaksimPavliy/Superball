namespace FriendsGamesTools.Integrations
{
    public class AppLovinSetupManager_HowTo : HowToModule
    {
        public override string forWhat => "AppLovin sdk";
        protected override void OnHowToGUI()
        {
            AppLovinManager.ShowOnGUI("download plugin, restart Unity and put this script to the scene",
                "This module can provide ads. Use <b>ADS</b> module to show them");
        }
        protected override string docsURL => "https://docs.google.com/document/d/14A7u8rdL75bUhMEmPpUp33Nzry-FZz-pUM_jiK_a53U/edit?usp=sharing";
        ExampleScript AppLovinManager = new ExampleScript("AppLovinManager");
    }
}


