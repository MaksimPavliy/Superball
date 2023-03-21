namespace FriendsGamesTools.Integrations
{
    public class AppsFlyerSetupManager_HowTo : HowToModule
    {
        public override string forWhat => "AppsFlyer sdk";
        protected override void OnHowToGUI()
        {
            AppsFlyerManager.ShowOnGUI("download plugin, restart Unity and put this script to the game");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1A68UaDDRqQTXb8PZLhJm33E7RUKQbc18fY7QT0SoLDM/edit?usp=sharing";
        ExampleScript AppsFlyerManager = new ExampleScript("AppsFlyerManager");
    }
}


