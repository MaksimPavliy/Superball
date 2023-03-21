using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class BackendScriptingShouldBeConfigured : ProjectSettingItem
    {
        public override string name => "BACKEND_SCRIPTING";
        public override string description => "backend should be il2cpp with .net 4.x compatibility";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            void CheckPlatform(BuildTargetGroup platform)
            {
                var err = CheckPlatformSetting(ScriptingImplementation.IL2CPP, platform, PlayerSettings.GetScriptingBackend, PlayerSettings.SetScriptingBackend, false);
                if (!string.IsNullOrEmpty(err))
                    sb.AppendLine(err);
                err = CheckPlatformSetting(ApiCompatibilityLevel.NET_4_6, platform, PlayerSettings.GetApiCompatibilityLevel, PlayerSettings.SetApiCompatibilityLevel, false);
                if (!string.IsNullOrEmpty(err))
                    sb.AppendLine(err);
            }
            if (settings.AndroidEnabled)
                CheckPlatform(BuildTargetGroup.Android);
            if (settings.IOSEnabled)
                CheckPlatform(BuildTargetGroup.iOS);
        }
        public override bool canSetup => true;
        public override void SetupPlatform(BuildTargetGroup platform)
        {
            CheckPlatformSetting(ScriptingImplementation.IL2CPP, platform, PlayerSettings.GetScriptingBackend, PlayerSettings.SetScriptingBackend, true);
            CheckPlatformSetting(ApiCompatibilityLevel.NET_4_6, platform, PlayerSettings.GetApiCompatibilityLevel, PlayerSettings.SetApiCompatibilityLevel, true);
        }

        static string CheckPlatformSetting<T>(T neededValue, BuildTargetGroup platform,
            Func<BuildTargetGroup, T> get, Action<BuildTargetGroup, T> set, bool setup)
            where T : Enum
        {
            var currValue = get(platform);
            if (!EqualityComparer<T>.Default.Equals(currValue, neededValue))
            {
                if (setup)
                    set(platform, neededValue);
                return $"{typeof(T).Name} should be {neededValue} for {platform}, but is {currValue}";
            }
            else
                return null;
        }
    }
}
