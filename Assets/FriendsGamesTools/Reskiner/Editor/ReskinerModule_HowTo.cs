
namespace FriendsGamesTools.Reskiner
{
    public class ReskinerModule_HowTo : HowToModule
    {
        public override string forWhat => "replacing all graphics for reskinning the game";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel(
                "assets need to be replaced will show in <b>red</b>,\n" +
                "assets replaced are <b>green</b>,\n" +
                "assets ok to be not replaced are <b>yellow</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1d9m6BKVtBqHFY-8OibDcttSzRTtYNvPYf9ZOwSAlnJQ/edit?usp=sharing";
    }
}


