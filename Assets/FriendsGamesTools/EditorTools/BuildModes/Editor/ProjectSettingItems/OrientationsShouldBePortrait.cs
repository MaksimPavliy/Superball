using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class OrientationsShouldBePortrait : ProjectSettingItem
    {
        public override string name => "PORTRAIT_ONLY";
        public override string description => "Allowed orientations should be portrait";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.Portrait)
                sb.AppendLine("orientation should be portrait");
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        }
    }
}
