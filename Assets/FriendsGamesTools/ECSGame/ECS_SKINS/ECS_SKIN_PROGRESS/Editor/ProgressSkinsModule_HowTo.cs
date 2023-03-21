namespace FriendsGamesTools
{
    public class ProgressSkinsModule_HowTo : HowToModule
    {
        public override string forWhat => "Skins unlocked with game progress";
        protected override void OnHowToGUI()
        {
            ProgressSkinController.ShowOnGUI("override this class", "override <b>GetPercentsPerLevel()</b> to set progress anount");
            RewardProgressWindow.ShowOnGUI("put under windows on scene");
            UnlockProgressSkinWindow.ShowOnGUI("put under windows on scene");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1XtRqbSVd_PFG72N_Ef5bZGWCDKIxjtq6Zc0vpME5-yE/edit?usp=sharing";

        ExampleScript UnlockProgressSkinWindow = new ExampleScript("UnlockProgressSkinWindow");
        ExampleScript RewardProgressWindow = new ExampleScript("RewardProgressWindow");
        ExampleScript ProgressSkinController = new ExampleScript("ProgressSkinController");
    }
}