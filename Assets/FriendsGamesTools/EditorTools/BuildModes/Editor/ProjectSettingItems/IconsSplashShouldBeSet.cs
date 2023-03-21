using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class IconsSplashShouldBeSet : ProjectSettingItem
    {
        public override string name => "ICONS_SPLASH";
        public override string description => "App icons and splash screen should be set";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            void CheckPlatform(BuildTargetGroup platform)
            {
                if (!platform.ToTargetPlatform().IsInstalledInEditor())
                    return;
                IconKindExists(IconKind.Application, BuildTargetGroup.Unknown, sb); // Check only default icon - its on unknown platform.
                if (DefinesModifier.DefineExists(PushNotifications.MobileNotificationsWrapperModule.define))
                    IconKindExists(IconKind.Notification, platform, sb);
                IconKindExists(IconKind.Settings, platform, sb);
                if (platform == BuildTargetGroup.iOS)
                {
                    IconKindExists(IconKind.Spotlight, platform, sb);
                    IconKindExists(IconKind.Store, platform, sb);
                }
            }
            if (settings.AndroidEnabled)
                CheckPlatform( BuildTargetGroup.Android);
            if (settings.IOSEnabled)
                CheckPlatform(BuildTargetGroup.iOS);

            if (PlayerSettings.SplashScreen.background == null)
                sb.AppendLine($"splash should be set");
        }

        static bool IconKindExists(IconKind type, BuildTargetGroup platform, StringBuilder sb = null)
        {
            if (GetIcon(type, platform) == null)
            {
                sb?.AppendLine($"{type} icon not set for {platform}");
                return false;
            }
            return true;
        }
        static Texture2D GetIcon(IconKind type, BuildTargetGroup platform)
            => PlayerSettings.GetIconsForTargetGroup(platform, type).Find(i => i != null);
        public override bool canSetup => true;
        public override void SetupPlatform(BuildTargetGroup platform)
        {
            var icon = GetIcon(IconKind.Application, BuildTargetGroup.Unknown);
            if (PlayerSettings.SplashScreen.background == null && icon != null)
            {
                var path = AssetDatabase.GetAssetPath(icon);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                PlayerSettings.SplashScreen.background = sprite;
                Debug.Log($"setting splash pic from icon {path}");
                if (sprite == null)
                    Debug.LogError($"change {path} to sprite, its required for making it splashscreen");
            }
        }
    }
}
