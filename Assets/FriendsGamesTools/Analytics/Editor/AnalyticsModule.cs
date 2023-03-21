using FriendsGamesTools.Integrations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Analytics
{
    public class AnalyticsModule : RootModule
    {
        public const string define = "ANALYTICS";
        public override string Define => define;
        public override HowToModule HowTo() => new AnalyticsModule_HowTo();

#if ANALYTICS
        public static AnalyticsSettings settings => AnalyticsSettings.instance;
        List<ModuleManager> allSourceModules;
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<AnalyticsSettings>.EnsureExists();
            allSourceModules =
                ReflectionUtils.GetAllDerivedTypes(typeof(IAnalyticsModule))
                .Filter(t => t.CanCreateInstance())
                .ConvertAll(t => FriendsGamesToolsWindow.allModules.Find(m => m.GetType() == t));
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            GUILayout.Space(20);

            bool changed = false;
            GUILayout.Label("Send events to:");
            allSourceModules.ForEach(m=> {
                var configured = (m as LibrarySetupManager).configured;
                GUILayout.BeginHorizontal();
                var enabled = !settings.disabledAnalytics.Contains(m.Define);
                if (EditorGUIUtils.Toggle(m.Define, ref enabled, ref changed))
                {
                    if (enabled)
                        settings.disabledAnalytics.Remove(m.Define);
                    else
                        settings.disabledAnalytics.Add(m.Define);
                }
                string state = configured ? "configured" : "not configured";
                Color col = enabled ? (configured ? EditorGUIUtils.green : EditorGUIUtils.red) : EditorGUIUtils.gray;
                EditorGUIUtils.ColoredLabel(state, col);
                GUILayout.EndHorizontal();
            });
            if (changed)
                EditorUtils.SetDirty(settings);
        }
        public override string DoReleaseChecks()
        {
            if (AnalyticsManager.GetSourceTypes(true).Count == 0)
                return "Analytics module enabled, but no SDK to send events to";
            return string.Empty;
        }
#endif
    }
    public interface IAnalyticsModule { }   
}


