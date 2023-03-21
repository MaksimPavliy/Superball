namespace FriendsGamesTools
{
    public class TutorialModule_HowTo : HowToModule
    {
        public override string forWhat => "'Tap this, than tap that'-like tutorial";
        protected override void OnHowToGUI()
        {
            TutorialButton.ShowOnGUI("Prepare tutorial buttons, put this script to them");
            DefaultHighlighter.ShowOnGUI("Set this prefab as highlighter to your tutorial buttons",
                "You can create similar highlighter yourself if you need to customize arrow, darkening, tutorial texts etc");
            TutorialManager.ShowOnGUI("Put this to the scene");
            TutorialChapter.ShowOnGUI("Derive from this script and put it to the scene",
                "<b>condition</b> - return true when its time to show chapter\n" +
                "<b>Showing()</b> - write the very tutorial, use things like\n" +
                "\t<b>await MoveCamera.MoveTo(pos)</b>\n" +
                "\t<b>await TutorialButton.PressingButton(tutorialText)</b>\n" +
                "\t<b>await GameTime.PauseSoft()</b>\n" +
                "\t<b>await TutorialAssistantView.Showing(text, duration)</b>\n" +
                "\tetc");

        }
        protected override string docsURL => "https://docs.google.com/document/d/1xKYm0Ip2vIsPGkLTqeC_Ye0Gslw84YVlUSaxykw07s0/edit?usp=sharing";

        ExampleScript TutorialButton = new ExampleScript("TutorialButton");
        ExamplePrefab DefaultHighlighter = new ExamplePrefab("DefaultHighlighter");
        ExampleScript TutorialManager = new ExampleScript("TutorialManager");
        ExampleScript TutorialChapter = new ExampleScript("TutorialChapter");
    }
}