using System.Text;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ReleaseBuildModeShouldBeInRelease : ProjectSettingItem
    {
        public override string name => "RELEASE_BUILD_MODE_IN_RELEASE";
        public override string description => "build mode should be release";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!BuildModeSettings.release)
                sb.AppendLine(description);
        }
    }
}