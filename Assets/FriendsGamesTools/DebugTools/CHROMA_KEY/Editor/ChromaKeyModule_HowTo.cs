namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class ChromaKeyModule_HowTo : HowToModule
    {
        public override string forWhat => "chroma key (showing parts of game over green solid color for videos creating)";
        protected override string docsURL => "https://docs.google.com/document/d/1j3lwCvnzvus9tEdf5fEHTXMHTjbCvSCrvgC71BM-Nok/edit?usp=sharing";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel(
                "In debug panel prefab, DebugMenuEditor script, turn on <b>chromakey</b> and save prefab\n" +
                "While game is played, go to debug panel, chroma key tab\n" +
                "Turn it on and see all gameobjects in scene, any of them can be turned off\n" +
                "except ones that are required to show debug panel itself");
        }
    }
}