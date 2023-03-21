namespace FriendsGamesTools
{
    public class HapticModule_HowTo : HowToModule
    {
        public override string forWhat => "haptic feedback";
        protected override void OnHowToGUI()
        {
            Haptic.ShowOnGUI("IOS and Android",
                "just call <b>Haptic.Vibrate()</b> or <b>Haptic.Vibrate(power)</b>\n" +
                "<b>Haptic.available</b> for checks"); // ImpactFeedback.Medium
        }
        protected override string docsURL => "https://docs.google.com/document/d/1lnlS9rcNiAayWu_jOpOC8LIBwwLS_LPwuLVUtqA1MQM/edit?usp=sharing";
        ExampleScript Haptic = new ExampleScript("Haptic");
    }
}