using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public abstract class ProjectSettingItem
    {
        public abstract string name { get; }
        public abstract string description { get; }
        public virtual bool canSetup => false;
        public virtual bool ignoreByDefault => false;
        public virtual bool setupByDefault => true;
        ProjectSettingItemConfig.HowToApply defaultAction => ignoreByDefault ? ProjectSettingItemConfig.HowToApply.Ignore :
            (canSetup ? (setupByDefault ? ProjectSettingItemConfig.HowToApply.AutoSetup : ProjectSettingItemConfig.HowToApply.ReleaseCheck)
            : ProjectSettingItemConfig.HowToApply.ReleaseCheck);
        public static BuildModeSettings settings => BuildModeSettings.instance;
        ProjectSettingItemConfig GetConfig() => BuildModeSettings.instance.configs.Find(c => c.settingName == name);
        ProjectSettingItemConfig _config;
        public ProjectSettingItemConfig config {
            get {
                if (_config == null)
                    _config = GetConfig();
                if (_config == null)
                    _config = new ProjectSettingItemConfig { settingName = name, howToApply = defaultAction };
                return _config;
            }
        }
        public void SetHowToApply(ProjectSettingItemConfig.HowToApply howToApply)
        {
            var c = GetConfig();
            if (c == null)
            {
                c = new ProjectSettingItemConfig { settingName = name };
                BuildModeSettings.instance.configs.Add(c);
            }
            c.howToApply = howToApply;
        }
        static StringBuilder sb = new StringBuilder();
        public void SetupIfNeeded()
        {
            sb.Clear();
            GetReleaseCheckError(sb);
            if (sb.Length > 0)
                Setup();
        }
        protected virtual void Setup() { }
        public virtual void SetupPlatform(BuildTargetGroup platform) { }
        public abstract void GetReleaseCheckError(StringBuilder sb);
        bool Toolbar(ref bool changed)
        {
            const float width = 400;
            if (canSetup)
                return EditorGUIUtils.Toolbar(description, ref config.howToApply, ref changed, width);
            var howToApply = ProjectSettingItemConfig.HowToApplyCantSetup.ReleaseCheck;
            if (config.howToApply == ProjectSettingItemConfig.HowToApply.Ignore)
                howToApply = ProjectSettingItemConfig.HowToApplyCantSetup.Ignore;
            EditorGUIUtils.Toolbar(description, ref howToApply, ref changed, width);
            var newHowToApply = ProjectSettingItemConfig.HowToApply.ReleaseCheck;
            if (howToApply == ProjectSettingItemConfig.HowToApplyCantSetup.Ignore)
                newHowToApply = ProjectSettingItemConfig.HowToApply.Ignore;
            var currChanged = newHowToApply != config.howToApply;
            if (currChanged)
            {
                changed = true;
                config.howToApply = newHowToApply;
            }
            return currChanged;
        }
        public void OnGUI(ref bool changed)
        {
            bool recompileNeeded = false;
            if (Toolbar(ref changed))
            {
                SetHowToApply(config.howToApply);
                if (config.howToApply == ProjectSettingItemConfig.HowToApply.AutoSetup)
                    recompileNeeded = true;
            }
            if (recompileNeeded)
                CompilationCallback.ForceRecompilation();
        }
    }
}
