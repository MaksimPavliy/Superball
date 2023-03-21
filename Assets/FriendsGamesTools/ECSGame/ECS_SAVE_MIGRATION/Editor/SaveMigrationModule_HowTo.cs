namespace FriendsGamesTools.ECSGame
{
    public class SaveMigrationModule_HowTo : HowToModule
    {
        public override string forWhat => "to migrate player data to new versions";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel(
                $"SAVE & INC data version just before each game release\n" +
                $"Derive your migration class from base class that will be generated for you\n" +
                $"Put any custom migration logic there or just leave it as is in most cases");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1qgJu25C4U27KFIAHGlc3gha2yBZwPCzd-TZ9Xf2-5BQ/edit?usp=sharing";
    }
}