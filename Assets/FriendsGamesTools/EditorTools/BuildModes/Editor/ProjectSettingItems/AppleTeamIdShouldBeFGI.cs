using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class AppleTeamIdShouldBeFGI : ProjectSettingItem
    {
        public override string name => "APPLE_TEAM_ID_FGI";
        public override string description => "Team id in xcode should be ours";
        public override void GetReleaseCheckError(StringBuilder sb) {
            if (!BuildModeSettings.instance.IOSEnabled)
                return;
#if UNITY_IOS
            if (PlayerSettings.iOS.appleDeveloperTeamID != FriendsGamesConstants.AppleTeamId)
                sb.AppendLine($"apple team id should be FGI's");
#endif
        }
        public override bool canSetup => true;
        protected override void Setup() {
            PlayerSettings.iOS.appleDeveloperTeamID = FriendsGamesConstants.AppleTeamId;
        }
    }
}