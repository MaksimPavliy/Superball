using UnityEngine;

namespace FriendsGamesTools.Ads
{
    public class AdsModule_HowTo : HowToModule
    {
        public override string forWhat => "showing ads";
        protected override void OnHowToGUI()
        {
            GUILayout.Label("First, ensure you have any SDK integration module enabled and set up.");
            AdsManager.ShowOnGUI("Derive your ads manager from this class and put it on scene",
                "Override <b>adsIntegrationSource</b> to select SDK to use for ads\n" +
                "Call <b>AdsManager.instance.rewarded.available</b> and <b>Show(onCompleted)</b> for rewarded ads using\n" +
                "Call <b>AdsManager.instance.interstitial.available</b> and <b>Show(onCompleted)</b> for interstitial ads using\n" +
                "Optional: override <b>OnAdShown()</b> and <b>OnAdHidden()</b> to do smth while any ad is shown.\n" +
                "There are similar methods for rewarded and interstitial ads separately");

            WatchAdButtonView.ShowOnGUI("Use this script for buttons that give something for watching ads",
                "Select <b>type</b> to choose interstitial or rewarded ad\n" +
                "it respects ad availability\n" +
                "call <b>SubscribeAdWatched(onWatched)</b> to give some reward for player");

            X1X3EarningView.ShowOnGUI("Use this script to give some amount of something x1 free or x3 for ad",
                "set <b>x1Amount</b> to show amount player receives x1\n" +
                "set <b>x3Amount</b> to show amount player receives x3\n" +
                "set <b>x3Button</b> to give player ability to press watch button properly\n" +
                "call <b>OnAdWatched(action)</b> to act when player watched ad");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1QAT_UuQPewtcQxtMFwRlq5vgGpaejW_j0v6iAKx8-_o/edit?usp=sharing";
        ExampleScript AdsManager = new ExampleScript("AdsManager");
        ExampleScript WatchAdButtonView = new ExampleScript("WatchAdButtonView");
        ExampleScript X1X3EarningView = new ExampleScript("X1X3EarningView");
    }
}