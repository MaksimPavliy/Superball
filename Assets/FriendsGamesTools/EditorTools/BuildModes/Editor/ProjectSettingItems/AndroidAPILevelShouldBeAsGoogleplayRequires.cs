using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class AndroidAPILevelShouldBeAsGoogleplayRequires : ProjectSettingItem
    {
        // Starting August 2021, new apps will be required to target API level 30 (Android 11) and use the Android App Bundle publishing format.
        // Starting November 2021, all app updates will be required to target API level 30 (Android 11)
        public override string name => "GOOGLE_PLAY_API";
        public override string description => "Google Play requires target API level from 30";
        int targetAPI
        {
            get => (int)PlayerSettings.Android.targetSdkVersion;
            set => PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)value;
        }
        const int requiredTargetAPI = 30;
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!settings.AndroidEnabled)
                return;
            if (targetAPI < requiredTargetAPI)
                sb.AppendLine(description);
        }
        public override bool canSetup => true;
        public override void SetupPlatform(BuildTargetGroup platform)
        {
            if (platform == BuildTargetGroup.Android)
                targetAPI = requiredTargetAPI;
        }
    }
}
