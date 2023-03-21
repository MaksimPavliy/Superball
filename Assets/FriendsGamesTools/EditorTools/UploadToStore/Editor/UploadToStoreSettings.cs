namespace FriendsGamesTools.EditorTools.Upload
{
    public class UploadToStoreSettings : SettingsScriptable<UploadToStoreSettings>
    {
        public bool uploadEnabled = true;
        public string cloudBuildTargetContains = "AppStore";
        public string itunesUserName = FriendsGamesConstants.DeveloperEmail;
        public string itunesAppPass = "gyxb-nsyk-evym-jnkm";
    }
}