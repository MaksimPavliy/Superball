using FriendsGamesTools.ECSGame;
using FriendsGamesTools.Integrations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class FGTWizard
    {
        public static FGTLocalSettings settings => SettingsInEditor<FGTLocalSettings>.instance;
        public static bool shown => settings.wizardShown;
        public static void Show()
        {
            settings.wizardShown = true;
            settings.wizardQuestionInd = 0;
            settings.wizardAnswers.Clear();
            settings.SetChanged();
        }
        public static void Hide() {
            settings.wizardShown = false;
            settings.SetChanged();
        }
        static void SetAnswer(int ind, bool yes)
        {
            while (settings.wizardAnswers.Count <= ind)
                settings.wizardAnswers.Add(false);
            settings.wizardAnswers[ind] = yes;
            settings.wizardQuestionInd++;
            settings.SetChanged();
        }

        static HashSet<string> ignoredModules = new HashSet<string> {
            FGTRootModule.define,
            ECSModuleFolder.define,
            PlayerModule.define,
            OtherModule.define
        };
        static List<string> orderedModules = new List<string> {
        };
        public static void OnGUI()
        {
            if (GUILayout.Button("back"))
                Hide();
            GUILayout.Space(30);

            var modules = FriendsGamesToolsWindow.allModules.Filter(curr
            => !ignoredModules.Contains(curr.Define) && curr.canBeEnabled && curr.hasHowTo);

            if (settings.wizardQuestionInd < modules.Count)
            {
                var m = modules[settings.wizardQuestionInd];

                EditorGUIUtils.RichMultilineLabel($"question {settings.wizardQuestionInd + 1}/{modules.Count} " +
                    $"about module {m.Define} {(m.compiled ? "(currently enabled)" : "")}");
                EditorGUIUtils.RichMultilineLabel($"<b><i>Do you need {m.howTo.forWhat}?</i></b>");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("yes"))
                    SetAnswer(settings.wizardQuestionInd, true);
                if (GUILayout.Button("no"))
                    SetAnswer(settings.wizardQuestionInd, false);
                m.howTo.OnFullDocumentationGUI("details...");
                GUILayout.EndHorizontal();
            } else
            {
                var selectedModules = new List<string>();
                for (int i = 0; i < modules.Count; i++)
                {
                    if (!settings.wizardAnswers[i])
                        continue;
                    selectedModules.Add(modules[i].Define);
                }
                // Get dependant modules.
                var dependantModules = new List<string>();
                selectedModules.ForEach(curr=> {
                    var currDependant = FriendsGamesToolsWindow.GetModule(curr).GetDependFromModulesRecursively();
                    currDependant.ForEach(c => {
                        if (!selectedModules.Contains(c) && !dependantModules.Contains(c))
                            dependantModules.Add(c);
                    });
                });
                EditorGUIUtils.RichMultilineLabel($"complete\n");
                if (GUILayout.Button("use selected modules"))
                {
                    selectedModules.AddRange(dependantModules);
                    var disabledModules = FriendsGamesToolsWindow.allModules.Filter(m
                        => !selectedModules.Contains(m.Define) && m.canBeEnabled).ConvertAll(m => m.Define);
                    Hide();
                    DefinesModifier.ModifyDefines(selectedModules, disabledModules);
                }
                 EditorGUIUtils.RichMultilineLabel($"\nyou selected modules:\n{selectedModules.PrintCollection("\n")}\n");
                if (dependantModules.Count > 0)
                    EditorGUIUtils.RichMultilineLabel($"also your modules depend on other modules, they will be enabled as well:\n" +
                        $"{dependantModules.PrintCollection("\n")}\n");
                EditorGUIUtils.RichMultilineLabel($"note: you can integrate following SDK later in FGT->integrations\n" +
                    $"{FriendsGamesToolsWindow.allModules.Filter(m => m is LibrarySetupManager).ConvertAll(m => m.Define).PrintCollection("\n")}");
                
            }

            // tell what modules are required for existing ones.
            // Apply modules enabling/disabling

            // Try updating entities.
            // Maybe also update required packages?
            // Maybe delete packages
        }
    }
}