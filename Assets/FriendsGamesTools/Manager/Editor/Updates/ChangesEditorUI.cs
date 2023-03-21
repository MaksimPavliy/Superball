using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools.ModulesUpdates
{
    public static class ChangesEditorUI
    {
        static UpdatesConfig config => UpdatesConfig.instance;
        static UpdatesSettings settings => SettingsInEditor<UpdatesSettings>.instance;
        public static FGTLocalSettings FGTSettings => SettingsInEditor<FGTLocalSettings>.instance;
        public static bool shown => FGTSettings.updatesShown;
        static bool configChanged;
        static void SaveChanges()
        {
            if (configChanged)
            {
                EditorUtils.SetDirty(config);
                configChanged = false;
            }
            EditorUtils.SetDirty(settings);
            EditorUtils.SetDirty(FGTSettings);
        }
        public static void ShowChangesEnabling()
        {
            var changes = false;
            ShowChangesEnabling(ref changes);
            if (changes)
                SaveChanges();
        }
        private static void ShowChangesEnabling(ref bool changes)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.ShowValid(allOk);
            EditorGUIUtils.Toggle("changes shown", ref FGTSettings.updatesShown, ref changes);
            GUILayout.EndHorizontal();
        }
        public static void OnGUI()
        {
            var changes = false;
            EditorGUIUtils.RichLabel("Changelog", anchor: TextAnchor.MiddleCenter, fontStyle: FontStyle.BoldAndItalic);

            GUILayout.BeginHorizontal();
            ShowChangesEnabling(ref changes);
            EditorGUIUtils.Toggle("show only not completed", ref FGTSettings.updatesShowOnlyNotCompleted, ref changes);
            EditorGUIUtils.Toggle("show only relevant", ref FGTSettings.updatesShowOnlyRelevant, ref changes);
            GUILayout.EndHorizontal();
            
            ShowChanges(ref changes);

            ShowCreateNewChange(ref changes);

            if (changes)
                SaveChanges();
        }
        static bool createNewChangeShown => newChange != null;
        static ModuleChange newChange;
        const string AllModules = "ALL";
        private static void ShowCreateNewChange(ref bool changes)
        {
            if (!FGTSettings.FGTEditable)
                return;
            var currChange = false;
            if (!createNewChangeShown)
            {
                if (GUILayout.Button("create new change"))
                    newChange = new ModuleChange { guid = Guid.NewGuid().ToString() };
            } else
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(newChange.AffectedModule == AllModules 
                    || FriendsGamesToolsWindow.allModules.Any(m => m.Define == newChange.AffectedModule));
                EditorGUIUtils.TextField("Affected module", ref newChange.AffectedModule, ref currChange);
                GUILayout.EndHorizontal();

                EditorGUIUtils.TextField("What's changed", ref newChange.whatsChanged, ref currChange);
                EditorGUIUtils.TextArea("Upgrade guide", ref newChange.upgradeGuide, ref currChange);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("create change"))
                {
                    config.changes.Add(newChange);
                    configChanged = true;
                    newChange = null;
                    changes = true;
                }
                if (GUILayout.Button("Cancel", GUILayout.Width(200)))
                    newChange = null;
                GUILayout.EndHorizontal();
            }
        }

        static bool IsCompleted(ModuleChange change) => settings.completedChanges.Contains(change.guid);
        static bool IsRelevant(ModuleChange change) 
            => change.AffectedModule == AllModules || DefinesModifier.DefineExists(change.AffectedModule);
        static void Complete(ModuleChange change) => settings.completedChanges.Add(change.guid);
        private static void ShowChanges(ref bool changes)
        {
            GUILayout.Space(20);
            const int smallWidth = 200;
            var smallWidthOption = GUILayout.Width(smallWidth);
            var shownCount = 0;
            allOk = true;
            foreach (var change in config.changes)
            {
                var completed = IsCompleted(change);
                if (FGTSettings.updatesShowOnlyNotCompleted && completed)
                    continue;

                var moduleEnabled = IsRelevant(change);
                if (FGTSettings.updatesShowOnlyRelevant && !moduleEnabled)
                    continue;

                if (moduleEnabled && !completed)
                    allOk = false;

                shownCount++;
                EditorGUIUtils.PushEnabling(moduleEnabled && !completed);
                EditorGUIUtils.RichMultilineLabel($"<b>{change.AffectedModule}</b>");
                EditorGUIUtils.RichMultilineLabel($"<b>{change.whatsChanged}</b>");
                if (!string.IsNullOrEmpty(change.upgradeGuide))
                    EditorGUIUtils.RichMultilineLabel(change.upgradeGuide);
                EditorGUIUtils.PopEnabling();

                if (completed)
                    GUILayout.Label("completed");
                else
                {
                    if (GUILayout.Button("I've read about this update and completed any upgrade steps needed"))
                        Complete(change);
                }
                GUILayout.Space(20);
            }
            if (shownCount == 0)
                GUILayout.Label("All changes completed");
        }
        public static string DoReleaseChecks()
        {
            var notCompleted = new HashSet<string>();
            foreach (var change in config.changes)
            {
                if (IsCompleted(change))
                    continue;
                notCompleted.Add(change.AffectedModule);
            }
            if (notCompleted.Count == 0)
                return string.Empty;
            else
                return $"FGT updates to modules {notCompleted.PrintCollection(",")} should be completed";
        }
        static bool allOk;
        public static void OnEnable()
        {
            var changed = false;
            allOk = true;
            foreach (var change in config.changes)
            {
                var relevant = IsRelevant(change);
                var completed = IsCompleted(change);
                if (relevant && !completed)
                {
                    FGTSettings.updatesShown = true;
                    changed = true;
                    allOk = false;
                }
                if (relevant || completed)
                    continue;
                Complete(change);
                changed = true;
            }
            if (changed)
                SaveChanges();
        }
    }
}
