#if APP_LOVIN
namespace FriendsGamesTools.Integrations
{
    public class AppLovinSettings : SettingsScriptable<AppLovinSettings>
    {
        protected override string SubFolder => "AppLovin";
        public string applovinSDKKey;
        public bool interstitialsEnabled = true;
        public bool rewardedVideosEnabled = true;
        public bool testModeEnabled;
    }
}
#endif