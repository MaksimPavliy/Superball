namespace FriendsGamesTools.EditorTools
{
    public class UpdateArtModule_HowTo : HowToModule
    {
        public override string forWhat => "updating art from Google Drive";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("Install Google Drive client, sync some local folder" +
                " with ArtSource folder of the game on drive\n" +
                "Then use it as <b>folder_external</b>\n" +
                "Then you're ready to update art to <b>folder_in_assets</b>");
                //"The main feature is that it automatically syncs some folder in assets with\n" +
                //"external folder - adds new files, removes removed, updates updated\n" +
                //"and updating is without changing meta files, so inspector links remain\n" +
                //"Optionally you can make postprocessing to pictures.");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1K9qXmjVP4vFuax1h0p4SINQAXYW6rOP9Dt7dPz26T0w/edit?usp=sharing";
    }
}


