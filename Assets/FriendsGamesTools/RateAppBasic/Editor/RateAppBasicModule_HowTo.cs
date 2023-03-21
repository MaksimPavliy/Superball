namespace FriendsGamesTools
{
    public class RateAppBasicModule_HowTo : HowToModule
    {
        public override string forWhat => "basic rate app";
        protected override void OnHowToGUI()
        {
            RateApp.ShowOnGUI("call <b>RateApp.Open()</b> to rate app. Works for Apple App Store and Google Play Market",
                "<i>Warning! rate app cant be shown on <b>TestFlight</b>, you have to use local build for check</i>");
            // https://developer.apple.com/documentation/storekit/skstorereviewcontroller/2851536-requestreview?language=objc
        }
        protected override string docsURL => "https://docs.google.com/document/d/1D7aDjpuiqC-kA5s3CEFZQSbrhAa_kiZ--rQTJe_h4Q8/edit?usp=sharing";
        ExampleScript RateApp = new ExampleScript("RateApp");
    }
}