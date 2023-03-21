namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class BuildModesModule_HowTo : HowToModule
    {
        public override string forWhat => "switching between release and debug";
        protected override void OnHowToGUI()
        {
            BuildModeSettings.ShowOnGUI("Check current build mode from script",
                "Use <b>Develop</b> when you're working in unity (<i>if (<b>BuildModeSettings.develop</b>)</i> to do debug hardcode)\n" +
                "Use <b>Test</b> mode (<i>if (<b>BuildModeSettings.test</b>)</i> to do something only for test builds)\n" +
                "Use <b>Release</b> mode (<i>if (<b>BuildModeSettings.release</b>)</i> to do something only for store builds)\n");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1XRa1e3JIsajN8vrYZ2E6AnmeFX2ddAsVUUNPOnaPZEM/edit?usp=sharing";
        ExampleScript BuildModeSettings = new ExampleScript("BuildModeSettings");
    }
}