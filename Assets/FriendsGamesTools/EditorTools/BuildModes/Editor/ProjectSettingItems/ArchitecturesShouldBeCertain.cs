using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ArchitecturesShouldBeCertain : ProjectSettingItem
    {
        // Google play requires this from fall 2019.
        public override string name => "BUILD_ARCHITECTURES";
        public override string description => "Google play requires armv7 and arm64 to be enabled, ios builds with arm64 for now";
        const AndroidArchitecture androidArchitecture = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        const int iosArchitecture = 1; // 0 - None, 1 - ARM64, 2 - Universal.  
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (settings.AndroidEnabled)
            {
                var err = CheckAndroidArchitectures(false);
                if (!string.IsNullOrEmpty(err))
                    sb.AppendLine(err);
            }
            if (settings.IOSEnabled)
            {
                var err = CheckIOSArchitectures(false);
                if (!string.IsNullOrEmpty(err))
                    sb.AppendLine(err);
            }
        }
        public override bool canSetup => true;
        public override void SetupPlatform(BuildTargetGroup platform)
        {
            if (platform == BuildTargetGroup.Android)
                CheckAndroidArchitectures(true);
            else
                CheckIOSArchitectures(true);
        }
        static string CheckIOSArchitectures(bool setup)
            => CheckSetting(iosArchitecture, () => PlayerSettings.GetArchitecture(BuildTargetGroup.iOS),
                val => PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, val), setup);
        static string CheckAndroidArchitectures(bool setup)
            => CheckSetting(androidArchitecture, () => PlayerSettings.Android.targetArchitectures,
                val => PlayerSettings.Android.targetArchitectures = val, setup);
        static string CheckSetting<T>(T neededValue,
            Func<T> get, Action<T> set, bool doSetup)
            //where T : Enum
        {
            if (!EqualityComparer<T>.Default.Equals(get(), neededValue))
            {
                if (doSetup)
                    set(neededValue);
                return $"setting {typeof(T).Name} should be {neededValue}";
            }
            else
                return null;
        }
    }
}
