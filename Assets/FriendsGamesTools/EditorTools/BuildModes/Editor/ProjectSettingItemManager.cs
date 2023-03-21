using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public static class ProjectSettingItemManager
    {
        static List<ProjectSettingItem> _items;
        public static List<ProjectSettingItem> items
            => _items ?? (_items = ReflectionUtils.GetAllDerivedTypes<ProjectSettingItem>()
                        .Filter(t => t.CanCreateInstance())
                        .ConvertAll(t => (ProjectSettingItem)Activator.CreateInstance(t)));

        public static bool GetEnabled<T>() where T : ProjectSettingItem
        {
            var item = items.Find(i => i.GetType() == typeof(T));
            var config = BuildModeSettings.instance.configs.Find(c => c.settingName == item.name);
            if (config == null) return true;
            return config.howToApply != ProjectSettingItemConfig.HowToApply.Ignore;
        }

        static BuildModeSettings settings => BuildModeSettings.instance;
        public static void InitOnLoad()
        {
            Setup();
            if (settings.AndroidEnabled)
                SetupPlatform(BuildTargetGroup.Android);
            if (settings.IOSEnabled)
                SetupPlatform(BuildTargetGroup.iOS);
        }
        static void Setup()
        {
            items.ForEach(s => {
                if (s.config.howToApply == ProjectSettingItemConfig.HowToApply.AutoSetup)
                    s.SetupIfNeeded();
            });
        }
        static void SetupPlatform(BuildTargetGroup platform)
        {
            items.ForEach(s => {
                if (s.config.howToApply == ProjectSettingItemConfig.HowToApply.AutoSetup)
                    s.SetupPlatform(platform);
            });
        }
        public static void GetAllReleaseCheckErrors(StringBuilder sb)
        {
            items.ForEach(s => {
                if (s.config.howToApply != ProjectSettingItemConfig.HowToApply.Ignore)
                    s.GetReleaseCheckError(sb);
            });
        }

        public static void OnGUI(ref bool changed)
        {
            foreach (var i in items)
                i.OnGUI(ref changed);
        }
    }
}
