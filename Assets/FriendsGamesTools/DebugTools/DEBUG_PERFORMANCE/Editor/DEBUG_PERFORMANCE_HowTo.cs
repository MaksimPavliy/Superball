
namespace FriendsGamesTools.DebugTools
{
    public class DEBUG_PERFORMANCE_HowTo : HowToModule
    {
        protected override string docsURL => "https://docs.google.com/document/d/1z3wNaLVIttfRgm4BJwRNIqWwPQ0bB5V-cjh3tzsolfg/edit?usp=sharing";
        public override string forWhat => "investigating low fps, mobile heating etc";

        protected override void OnHowToGUI()
        {
            ShowFPS.ShowOnGUI("Add this to show FPS");
            TargetFrameRateEditorDebug.ShowOnGUI("Add this script simulate different FPS",
                "Set desired <b>targetFrameRate</b> value\n" +
                "Will not spoil builds, works only for editor");
        }

        ExampleScript ShowFPS = new ExampleScript("ShowFPS");
        ExampleScript TargetFrameRateEditorDebug = new ExampleScript("TargetFrameRateEditorDebug");
    }
}