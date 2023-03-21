namespace FriendsGamesTools.Integrations
{
    public class FlurrySetupManager_HowTo : HowToModule
    {
        public override string forWhat => "Flurry sdk";
        protected override void OnHowToGUI()
        {
            // docs
            // https://developer.yahoo.com/flurry/docs/integrateflurry/unity/
            FlurryManager.ShowOnGUI("download plugin, restart Unity and put this script to the game");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1uuvA2P-GrFnrqYkB8LTJAlaUGBFC-aXrlmxl1iStL8o/edit?usp=sharing";
        ExampleScript FlurryManager = new ExampleScript("FlurryManager");
    }
}