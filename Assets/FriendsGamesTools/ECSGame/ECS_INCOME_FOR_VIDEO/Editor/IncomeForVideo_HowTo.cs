namespace FriendsGamesTools.ECSGame
{
    public class IncomeForVideo_HowTo : HowToModule
    {
        public override string forWhat => "income bonus for watching ad video";
        protected override void OnHowToGUI()
        {
            IncomeForVideoController.ShowOnGUI("derive from this script",
                "override <b>available</b> to make bonus available\n" +
                "override <b>multiplier</b> to change multiplying coefficient\n" +
                "override <b>duration</b> to change duration");
            IncomeForVideoIcon.ShowOnGUI("Derive from this script to show icon for proposing income for video");
            IncomeForVideoWindow.ShowOnGUI("derive form this script to make window for this module - its shown when player clicked on icon");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1lUY3L362zZegHxgoKHqHnw_gLzx8wDNfKAhiPqTd2GI/edit?usp=sharing";

        ExampleScript IncomeForVideoController = new ExampleScript("IncomeForVideoController");
        ExampleScript IncomeForVideoIcon = new ExampleScript("IncomeForVideoIcon");
        ExampleScript IncomeForVideoWindow = new ExampleScript("IncomeForVideoWindow");
    }
}