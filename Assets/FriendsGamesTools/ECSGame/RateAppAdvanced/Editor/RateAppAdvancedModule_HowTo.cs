namespace FriendsGamesTools
{
    public class RateAppAdvancedModule_HowTo : HowToModule
    {
        public override string forWhat => "advanced rate app";
        protected override void OnHowToGUI()
        {
            RateAppAdvancedController.ShowOnGUI("inherit this script",
                "call <b>ShowRateAppIfPossible</b> when player is excited and tend to rate app positively\n");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1AlI2elDpgaB1vAXjzwJK4ckXp7Wj2ZogYdV6V78zWwg/edit?usp=sharing";

        ExampleScript RateAppAdvancedController = new ExampleScript("RateAppAdvancedController");
    }
}