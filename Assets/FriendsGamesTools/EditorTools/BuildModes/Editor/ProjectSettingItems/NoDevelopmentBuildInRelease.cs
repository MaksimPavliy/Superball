using System.Text;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class NoDevelopmentBuildInRelease : ProjectSettingItem
    {
        public override string name => "NO_DEV_BUILD_IN_RELEASE";
        public override string description => "Development build should be disabled in release";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!ok)
                sb.AppendLine($"'development build' enabled in release");
        }
        bool ok => !BuildModeSettings.release || !UnityEditor.EditorUserBuildSettings.development;
        public override bool canSetup => true;
        protected override void Setup()
        {
            if (!ok)
                UnityEditor.EditorUserBuildSettings.development = false;
        }
    }
}