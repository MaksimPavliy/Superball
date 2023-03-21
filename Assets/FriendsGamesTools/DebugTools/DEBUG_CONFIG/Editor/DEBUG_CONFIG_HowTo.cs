using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DEBUG_CONFIG_HowTo : HowToModule
    {
        protected override string docsURL => "https://docs.google.com/document/d/1FqGeHBXuQDaM2ils64ZaFCocejPir1c86ARo5fETWx4/edit?usp=sharing";
        public override string forWhat => "to create game balance editor automatically";

        protected override void OnHowToGUI()
        {
            BalanceSettings.ShowOnGUI("Derive your balance script from this class",
                "add your <b>double</b>, <b>float</b>, <b>int</b>, <b>bool</b> values to balance\n" +
                "and <b>[Serializable] classes</b> and <b>Lists</b> with these values\n");

            DebugMenuEditor.ShowOnGUI("Open your debug panel prefab and enable bool <b>config</b> in inspector",
                "Then config tab appears automatically");
        }
        ExampleScript BalanceSettings = new ExampleScript("BalanceSettings");
        ExampleScript DebugMenuEditor = new ExampleScript("DebugMenuEditor");
    }
}