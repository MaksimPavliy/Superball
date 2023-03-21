using System.Collections.Generic;
using FriendsGamesTools.EditorTools.BuildModes;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class BuildModeSettings:SettingsScriptable<BuildModeSettings>
    {
        protected override string SubFolder => "BuildMode";

        public BuildModeType _mode;
        public static BuildModeType mode => instance != null ? instance._mode : BuildModeType.Develop;
        public static bool develop => mode == BuildModeType.Develop;
        public static bool test => mode == BuildModeType.Test;
        public static bool release => mode == BuildModeType.Release;

        public bool IOSEnabled = true;
        public bool AndroidEnabled = true;
        public bool PlatformEnabled(TargetPlatform platform) => platform == TargetPlatform.IOS ? IOSEnabled : AndroidEnabled;

        public string releaseErrors;

        public List<ProjectSettingItemConfig> configs = new List<ProjectSettingItemConfig>();
    }
}
namespace FriendsGamesTools
{
    public static class BuildMode
    {
        public static bool develop => BuildModeSettings.develop;
        public static bool test => BuildModeSettings.test;
        public static bool release => BuildModeSettings.release;
        public static BuildModeType type => BuildModeSettings.mode;
    }
}
