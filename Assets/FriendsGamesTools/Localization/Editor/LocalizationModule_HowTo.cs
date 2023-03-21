using UnityEngine;

namespace FriendsGamesTools
{
    public class LocalizationModule_HowTo : HowToModule
    {
        public override string forWhat => "Automated localizations";
        protected override void OnHowToGUI()
        {
            LocalizableView.ShowOnGUI("Add this to all <b>TextMeshProUGUI</b>",
                "Setup <b>localizationKey</b> in inspector\n" +
                "or set text from code: <b>localizableView.text = Localization.Get(\"LOCALIZATION_KEY\")</b>\n" +
                "(this method also supports parameters like string.Format does)");
            //RobotoWithChineseFGT.ShowOnGUI("Remember to use fonts that support your languages",
            //    "for example, this font supports Latin, Chinese(simplified) and Cyrrilic characters");
            SwitchLanguagesView.ShowOnGUI("Player can change language from <b>SettingsWindow</b> or DebugPanel ",
                "But you can use this prefab to allow language changing somewhere else");
        }
        protected override string docsURL => "";

        ExampleScript LocalizableView = new ExampleScript("LocalizableView");
        ExamplePrefab SwitchLanguagesView = new ExamplePrefab("SwitchLanguagesView");
    }
}