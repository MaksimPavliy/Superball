using UnityEngine;

namespace FriendsGamesTools
{
    public class OtherModule_HowTo : HowToModule
    {
        public override string forWhat => "some other tools (not yet categorized)";
        protected override void OnHowToGUI()
        {
            Billboard.ShowOnGUI("Put this on what should be oriented towards camera");
            FingerTrailView.ShowOnGUI("Adds finger trail effect to swipe gestures");
            RateApp.ShowOnGUI("Cross-platform rate app",
                "Call <b>RateApp.Open()</b> to use it");
            Outlinable.ShowOnGUI("Put this on <b>prefab</b> with 3D-model to make it outlinable",
                "Use bool <b>outlined</b> to turn on/off outlines\n" +
                "Weld dist should be small distance relatively to your model\n" +
                "Set layer to some layer that will be drawn last, even over posteffects");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1DqbzZfHkZIHMn6I-dANALgvnFv6EhwLovq1f3egAFVo/edit?usp=sharing";
        ExampleScript Billboard = new ExampleScript("Billboard");
        ExampleScript FingerTrailView = new ExampleScript("FingerTrailView");
        ExampleScript RateApp = new ExampleScript("RateApp");
        ExampleScript Outlinable = new ExampleScript("Outlinable");
    }
}


