using FriendsGamesTools.Ads;
using FriendsGamesTools.EditorTools;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelModule : DebugToolsModule
    {
        public const string define = "DEBUG_PANEL";
        public override string Define => define;
        public override HowToModule HowTo() => new DEBUG_PANEL_HowTo();
        public override List<string> dependFromModules => base.dependFromModules
            .Adding(UIModule.define)
            .Adding(EditorToolsModule.define);

#if DEBUG_PANEL
        DebugPanelSettings settings => SettingsInEditor<DebugPanelSettings>.instance;
        public override void OnEnable()
        {
            base.OnEnable();
            SettingsInEditor<DebugPanelSettings>.EnsureExists();
        }

        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            // Set opening.
            GUILayout.BeginHorizontal();
            EditorGUIUtils.IntField("open debug panel after ", ref settings.openPanelPresses, ref changed);
            EditorGUIUtils.FloatField("presses during", ref settings.openPanelPressDuration, ref changed);
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();
            EditorGUIUtils.Toggle("starts shown", ref settings.startsShown, ref changed);
            // Set password protection.
            GUILayout.BeginHorizontal();
            EditorGUIUtils.Toggle("password on not develop", ref settings.passwordIfNotDevelop, ref changed);
            if (settings.passwordIfNotDevelop)
                EditorGUIUtils.IntField("password", ref settings.password, ref changed);
            GUILayout.EndHorizontal();
            // Modules shown.
            GUILayout.BeginHorizontal();
            GUILayout.Label("items:");            
            GUILayout.EndHorizontal();
            settings.itemViews.ForEach(item=> {
                StartHorizontal();
                var (tab, name) = item.whereToShow;
                var enabled = !settings.disabledModules.Contains(name);
                if (EditorGUIUtils.Toggle("", ref enabled, ref changed, 20))
                {
                    if (enabled)
                        settings.disabledModules.Remove(name);
                    else
                        settings.disabledModules.Add(name);
                }
                GUILayout.Label($"{tab}->{name}", GUILayout.Width(400));
                EndHorizontal();
            });
            StartHorizontal();
            if (GUILayout.Button("update", GUILayout.Width(200)))
                UpdateItems();               
            EndHorizontal();
            void StartHorizontal() {
                GUILayout.BeginHorizontal();
                GUILayout.Space(100);
            }
            void EndHorizontal() {
                GUILayout.Space(200);
                GUILayout.EndHorizontal();
            }
            if (changed)
                settings.SetChanged();

        }
        void UpdateItems()
        {
            DebugPanelSettings.instance.itemViews.Clear();
            AssetsIterator.IterateAllPrefabsInProject((path, prefab) =>
            {
                var item = prefab.GetComponent<DebugPanelItemView>();
                if (item != null)
                    DebugPanelItemView.UpdateAddToSettings(item);
            });
        }
#endif
    }
}


