namespace FriendsGamesTools.Integrations
{
    public class FBSetupManager_HowTo : HowToModule
    {
        public override string forWhat => "Facebook sdk";
        protected override void OnHowToGUI()
        {
            FBManager.ShowOnGUI("download plugin, restart Unity and put this script to the game");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1Bw8aeQtybTJ__fy9wea90Sc2pL3hKdU435JV0FPVIaM/edit?usp=sharing";
        ExampleScript FBManager = new ExampleScript("FBManager");
    }
}


