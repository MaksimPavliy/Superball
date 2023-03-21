using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class AppNameShouldBeTheSameEverywhere : ProjectSettingItem
    {
        public override string name => "APP_NAME_SAME";
        public override string description => "app name shoud be the same everywhere";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
#if FACEBOOK
            var appName = FriendsGamesToolsWindow.GetModule<Integrations.FBSetupManager>().GetAppName();
            if (appName != PlayerSettings.productName)
                sb.AppendLine($"app name in PlayerSettings.productName={PlayerSettings.productName} but in fb sdk appName={appName}");
#endif
        }
    }
}
