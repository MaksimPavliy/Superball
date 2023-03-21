using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class TurnOffUnityLogo : ProjectSettingItem
    {
        public override string name => "UNITY_LOGO_OFF";
        public override string description => "Unity logo should be off";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!Application.HasProLicense())
                return;
            if (PlayerSettings.SplashScreen.show)
                sb.AppendLine("Splash screen should be off");
            if (PlayerSettings.SplashScreen.showUnityLogo)
                sb.AppendLine("Splash screen unity logo should be off");
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            if (!Application.HasProLicense())
                return;
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
    }
}
