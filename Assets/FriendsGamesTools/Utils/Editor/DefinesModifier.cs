using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class DefinesModifier
    {
        static List<BuildTargetGroup> platforms;
        static List<string[]> existingDefines;
        static int currPlatformInd => platforms.IndexOf(EditorUserBuildSettings.selectedBuildTargetGroup);
        public static void InitOnLoad()
        {
            RefreshExistingDefines();
            FixFGTDefines();
            //UnityEngine.Debug.Log($"defines = {existingDefines.PrintCollection(", ")}");
        }
        static void RefreshExistingDefines()
        {
            platforms = new List<BuildTargetGroup> {
                 BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone, BuildTargetGroup.WebGL
            };
            existingDefines = new List<string[]>();
            platforms.ForEach(platform =>
            {
                var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
                var platformDefines = definesString.Split(';');
                platformDefines = platformDefines.ToList().SortedBy(define => define.GetHashCode()).ToArray(); // Some defined order.
                existingDefines.Add(platformDefines);
            });
            var settings = SettingsInEditor<DefinesSettings>.instance;
            var currDefines = existingDefines[platforms.IndexOf(TargetPlatformUtils.currentBuildTargetGroup)];
            settings.SetDefinesInEditMode(currDefines);
            // Enable FGT modules toggles.
            FriendsGamesToolsWindow.allModules.ForEach(m => m.InitEnabling());
        }

        private static void FixFGTDefines()
        {
            if (Application.isPlaying) return;
            // If any FGT module define exists on only some platforms, re-add it.
            var fgtDefines = new List<string>();
            existingDefines.ForEach(platformDefines => {
                platformDefines.ForEach(define => {
                    if (FriendsGamesToolsWindow.allModules.Any(m => m.Define == define) && !fgtDefines.Contains(define))
                        fgtDefines.Add(define);
                });
            });
            var fgtDefinesToFix = new List<string>();
            fgtDefines.ForEach(fgtDefine => {
                existingDefines.ForEach(platformDefines => {
                    if (!platformDefines.Contains(fgtDefine))
                        fgtDefinesToFix.Add(fgtDefine);
                });
            });
            if (fgtDefinesToFix.Count == 0)
                return;
            Debug.Log($"Fixing FGT defines {fgtDefinesToFix.PrintCollection()}");
            ModifyDefines(fgtDefinesToFix, null);
        }

        public static bool DefineExists(string define) => existingDefines[currPlatformInd].Contains(define);
        public static void ModifyDefines(List<string> addedDefines, List<string> removedDefines)
        {
            if (addedDefines != null)
                addedDefines = addedDefines.Filter(d => platforms.Any(p => !existingDefines[platforms.IndexOf(p)].Contains(d)));
            if (removedDefines != null)
                removedDefines = removedDefines.Filter(d => platforms.Any(p => existingDefines[platforms.IndexOf(p)].Contains(d)));
            var anyChanges = false;
            if (addedDefines != null && addedDefines.Count > 0)
            {
                Debug.Log($"add defines {addedDefines.PrintCollection()}");
                anyChanges = true;
            }
            if (removedDefines != null && removedDefines.Count > 0)
            {
                Debug.Log($"removed defines {removedDefines.PrintCollection()}");
                anyChanges = true;
            }
            if (!anyChanges)
                return;
            platforms.ForEach(platform =>
            {
                List<string> newDefines = new List<string>();
                foreach (var existingDefine in existingDefines[platforms.IndexOf(platform)])
                {
                    if (removedDefines == null || !removedDefines.Contains(existingDefine))
                        newDefines.Add(existingDefine);
                }
                if (addedDefines != null)
                {
                    foreach (var addedDefine in addedDefines)
                    {
                        if (!newDefines.Contains(addedDefine))
                            newDefines.Add(addedDefine);
                    }
                }
                // EditorUserBuildSettings.selectedBuildTargetGroup
                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, string.Join(";", newDefines.ToArray()));
            });
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshExistingDefines();
        }
        public static void AddDefine(string define)
        {
            ModifyDefines(new List<string> { define }, new List<string>());
        }
        public static void RemoveDefine(string define)
        {
            ModifyDefines(new List<string>(), new List<string> { define });
        }
        public static void ReplaceDefine(string replaced, string replaceWith)
        {
            ModifyDefines(new List<string> { replaceWith }, new List<string> { replaced });
        }
    }
}