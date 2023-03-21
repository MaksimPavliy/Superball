namespace FriendsGamesTools
{
    public class MoneySkinModule_HowTo : HowToModule
    {
        public override string forWhat => "Skins unlocked for money";
        protected override void OnHowToGUI()
        {
            MoneySkinController.ShowOnGUI("override this class",
                "override <b>GetSkinPrice()</b> to set prices\n" +
                "call <b>BuyRandomSkin()</b> to buy next random skin for <b>nextSkinPrice</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1V-CSxeg45n7sPYynRkSVeUnXKXsloAf8laxg8LgaAsE/edit?usp=sharing";

        ExampleScript MoneySkinController = new ExampleScript("MoneySkinController");
    }
}