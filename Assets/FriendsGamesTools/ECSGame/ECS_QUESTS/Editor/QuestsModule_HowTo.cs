namespace FriendsGamesTools
{
    public class QuestsModule_HowTo : HowToModule
    {
        public override string forWhat => "Linear quests with progress";
        protected override void OnHowToGUI()
        {
            Quest.ShowOnGUI(
                "Derive quests from this class",
                "override <b>viewConfig</b> to set title, description, ico for quest\n" +
                "or set it to null to make quest <b>hidden</b>\n" +
                "override <b>rewardMoney</b> or <b>rewardHardCurrency</b> to set reward\n" +
                "override <b>UpdateActive()</b> to write quest logic\n" +
                "override <b>OnEvent(name, parameters)</b> to progress quest on events from ANALYTICS module\n" +
                "  - call <b>Complete()</b> to make quest completed\n" +
                "  - or override <b>maxQuestProgressItems</b> and return 3, then call <b>AddProgress()</b> 3 times. Use curr <b>progressItems</b> value.");

            QuestsController.ShowOnGUI("derive from this class",
                "override <b>GetQuestInstances()</b> to define quests order (return list of all quests to use)");

            TutorialAssistantView.ShowOnGUI("Put this prefab under UI canvas",
                "call TutorialAssistantView.<b>Show(desc)</b> to show character that says some text\n" +
                "TutorialAssistantView.<b>shown</b> checks assistant shown\n" +
                "TutorialAssistantView.<b>okShown</b> checks that text is fully shown and ready to be dismissed");

            QuestsView.ShowOnGUI("put this under UI canvas",
                "set quest arrow to <b>highlighterPrefab</b> in inspector, for example to <b>DefaultHighlighter</b>\n" +
                "QuestsView.ShowArrow(button, top)" +
                "QuestsView.HideArrow()");

            DefaultHighlighter.ShowOnGUI("default quest arrow");

            QuestWindow.ShowOnGUI("Put this script to QuestWindow");

            QuestTopView.ShowOnGUI("Put this script to quest icon in UI",
                "along with QuestView script");

            QuestView.ShowOnGUI("Put this script to QuestWindow and QuestTopView to show quest data");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1lpUOATy1Qj0mczPHkfTEDlsiVIu_VLeTrGFa1nxQnYU/edit?usp=sharing";

        ExampleScript Quest = new ExampleScript("Quest");
        ExampleScript QuestsController = new ExampleScript("QuestsController");
        ExamplePrefab TutorialAssistantView = new ExamplePrefab("TutorialAssistantView");
        ExampleScript QuestsView = new ExampleScript("QuestsView");
        ExamplePrefab DefaultHighlighter = new ExamplePrefab("DefaultHighlighter");
        ExampleScript QuestWindow = new ExampleScript("QuestWindow");
        ExampleScript QuestView = new ExampleScript("QuestView");
        ExampleScript QuestTopView = new ExampleScript("QuestTopView");
    }
}