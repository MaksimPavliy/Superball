using UnityEngine;

namespace FriendsGamesTools.Share
{
    public class ShareSettings : SettingsScriptable<ShareSettings>
    {
        public string gameNameIOS;
#if SHARE
        public string gameLink => TargetPlatformUtils.current == TargetPlatform.IOS ?
            GetIOSGameLink(gameNameIOS, FGTSettings.instance.appleAppId)
            : GetAndroidGameLink(Application.identifier);
#endif
        public static string GetAndroidGameLink(string packageId) => $"https://play.google.com/store/apps/details?id={packageId}";
        public static string GetIOSGameLink(string gameNameIOS, string appleAppId) => $"https://apps.apple.com/us/app/{ GetIOSGameID(gameNameIOS, appleAppId)}";
        private static string GetIOSGameID(string gameNameIOS, string appleAppId)
        {
            string name = gameNameIOS.Replace(" - ", "-");
            name = name.Replace(" ", "-");
            name = name.Replace(".", "-");
            name += "/id" + appleAppId;
            return name;
        }
    }
}