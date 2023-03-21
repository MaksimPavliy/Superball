using System;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    [Serializable]
    public class ProjectSettingItemConfig
    {
        public enum HowToApply
        {
            AutoSetup,
            ReleaseCheck,
            Ignore
        }
        public enum HowToApplyCantSetup
        {
            ReleaseCheck,
            Ignore
        }
        public HowToApply howToApply;
        public string settingName;
    }
}
