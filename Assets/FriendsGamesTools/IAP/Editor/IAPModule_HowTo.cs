using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class IAPModule_HowTo : HowToModule
    {
        public override string forWhat => "in-app purchases";
        protected override void OnHowToGUI()
        {
            if (!IAPModule.instance.compiled)
            {
                // How to enable purchases module.
                EditorGUIUtils.RichMultilineLabel(
                    "To enable In App Purchases:\n" +
                    "1. Enable <b>In App Purchasing</b> package <b>Package manager</b>\n" +
                    "2. Press 'enable purchasing'\n" +
                    "3. Answer COPPA question if needed\n" +
                    "4. Go to Unity->Services->IN-APP PURCHASING->Options and enter '<b>Google Public Key</b>', press Verify. Get this key from game account manager\n" +
                    "5. Press 'Import' button to import <b>UnityPurchasing</b> plugin, agree with all it asks");
            }
            else
            {
                if (IAPManager == null)
                    IAPManager = new ExampleScript("IAPManager");
                // How to add purchases.
                EditorGUIUtils.RichMultilineLabel(
                    "configure all required <b>products</b> and <b>subscriptions</b> here\n" +
                    "if there's something not configured, you'll see red marker at it\n");
                IAPManager.ShowOnGUI("Derive from this script and put it on scene",
                    "override <b>ApplyConsumablePurchase(type)</b> and implement giving player some consumable product value\n" +
                    "override <b>ApplyOnNonConsumablePurchase(type)</b> and implement giving player some non-consumable product value\n" +
                    "override <b>ApplySubscriptionPurchase(purchaseType)</b> and implement giving player some subscription value\n" +
                    "   where purchaseType tells whether its new subscription starting or existing prolonged");
                // How to test.
                EditorGUIUtils.RichMultilineLabel(
                    "You can test purchases\n" +
                    " - in unity or in build without exporting anything to stores - set 'debug mode in editor/build' to <b>Mocked</b>\n" +
                    " - in mobile builds with sandbox stores - set debug mode to <b>'Real'</b> and use tester accounts when purchasing");
            }

            // how to export?
            // setup your apple password.
            // get itmsp installed.
            // login once in your google account.
            // check that everything ready and press export.
            // go to stores and check that everything uploaded ok.
            
        }
        protected override string docsURL => "https://docs.google.com/document/d/1J_DsVRU1eK_4WdTaR2u6KEFPni0p7Yh3FZViyFr-X_8/edit?usp=sharing";
        ExampleScript IAPManager;


        // Test iap on ios.
        // Do once.
        // 1. create test users. https://help.apple.com/app-store-connect/#/dev8b997bee1
        // Do for each test.
        // 2. ensure that products are cleared for sale.
        // 3. wait few hours.
        // 4. login on ios device with sandbox tester.
        // 5. Try buy product. You have to see native purchasing window, saying its sandbox.

        // Test iap on android. https://developer.android.com/google/play/billing/billing_testing
        // Do once.
        // 1. add internal testers list. https://support.google.com/googleplay/android-developer/answer/3131213 and/or add gmail testers to Developer Console-> Settings > Account details-> Gmail accounts with testing access https://developer.android.com/google/play/billing/billing_testing
        // Do for each test.
        // 2. Upload app to internal test track (signed, release). This qill require all store info and screenshots to be completed.
        // 3. Wait few hours.
        // 4. Install app on android device. Its version number should be equals version number of internal-test-uploaded apk.
        // 5. Ensure you waited up to 24 hours after entering purchase metadata to store.
        // 6. Try buy products under accounts from internal testers list. 

        // Test subscriptions.
        // apple doc https://help.apple.com/app-store-connect/#/dev7e89e149d
        // google doc https://developer.android.com/google/play/billing/billing_testing
        // Test subscriptions are shortened
        // real duration    apple test      google test
        // 1 week           3 minutes       5 minutes
        // 1 month          5 minutes       5 minutes
        // 3 months         15 minutes      10 minutes
        // 6 months         30 minutes      15 minutes
        // 1 year           1 hour          30 minutes
        // Both apple and google have max 6 auto-renews in test. 
    }
}