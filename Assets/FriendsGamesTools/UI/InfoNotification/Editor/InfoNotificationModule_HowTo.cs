namespace FriendsGamesTools
{
    public class InfoNotificationModule_HowTo : HowToModule
    {
        public override string forWhat => "info notification";
        protected override void OnHowToGUI()
        {
            InfoNotificationView.ShowOnGUI("call <b>InfoNotificationView.Show(text)</b>");
        }
        protected override string docsURL => "";
        ExampleScript InfoNotificationView = new ExampleScript("InfoNotificationView");
    }
}