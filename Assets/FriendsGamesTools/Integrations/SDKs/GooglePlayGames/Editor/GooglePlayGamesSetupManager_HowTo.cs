namespace FriendsGamesTools.Integrations
{
    public class GooglePlayGamesSetupManager_HowTo : HowToModule
    {
        public override string forWhat => "GooglePlayGames sdk";
        protected override void OnHowToGUI()
        {
            GooglePlayGamesManager.ShowOnGUI("download plugin, restart Unity and put this script to the game",
                "paste <b>resource XML</b> text and press <b>Setup</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1YyD0LAdkFXX6PKlPssg43CZqe14M82oYf78ap0XoPEk/edit?usp=sharing";
        ExampleScript GooglePlayGamesManager = new ExampleScript("GooglePlayGamesManager");
    }
}


