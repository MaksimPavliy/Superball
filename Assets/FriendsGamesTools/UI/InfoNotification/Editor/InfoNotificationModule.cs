#if UI
using FriendsGamesTools.UI;
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class InfoNotificationModule : RootModule
    {
        public const string define = "INFO_NOTIFICATION";
        public override string Define => define;
        public override HowToModule HowTo() => new InfoNotificationModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(WindowsModule.define);
        public override string parentModule => UIModule.define;

#if INFO_NOTIFICATION
        InfoNotificationSettings config => SettingsInEditor<InfoNotificationSettings>.instance;
        const string defaultPath = FriendsGamesManager.MainPluginFolder + "/UI/InfoNotification/Resources/InfoNotificationView.prefab";
       
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            var changed = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtils.ShowValid(prefabOk);
            EditorGUIUtils.PrefabField("info notification prefab", ref config.prefab, ref changed);
            if (config.prefab == null)
            {
                config.prefab = AssetDatabase.LoadAssetAtPath<InfoNotificationView>(defaultPath);
                changed = true;
            }
            EditorGUILayout.EndHorizontal();
            if (changed)
                config.SetChanged();
        }
        bool prefabOk => AssetDatabase.GetAssetPath(config.prefab) != defaultPath;
        public override string DoReleaseChecks()
        {
            if (!prefabOk)
                return "info notification prefab not set";
            return base.DoReleaseChecks();
        }
#endif
    }
}