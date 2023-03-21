namespace FriendsGamesTools.Integrations
{
    public class GASetupManager_HowTo : HowToModule
    {
        public override string forWhat => "GameAnalytics sdk";
        protected override void OnHowToGUI()
        {
            GAManager.ShowOnGUI("Put this script on the scene");
        }
        protected override string docsURL => "https://docs.google.com/document/d/19z9Zc227yZufuA2BmH2ClAFIL8XCMfbng5PMasLwxok/edit?usp=sharing";
        ExampleScript GAManager = new ExampleScript("GAManager");
    }
}


