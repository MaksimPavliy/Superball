using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using FriendsGamesTools.DebugTools;
using System.Text;
using FriendsGamesTools.EditorTools.BuildModes;

namespace FriendsGamesTools
{
    public abstract class ModuleManager // call OnGUI Update OnEnable OnFocus OnLostFocus
    {
        #region Showing in editor window
        public enum ShowingState { Collapsed, Detailed, HowTo }
        public virtual bool hasCollapsedView => true;
        public virtual bool hasDetailedView => true;
        public virtual void ShortcutOnGUI() { }
        public abstract string parentModule { get; }
        public virtual void OnGUI(bool collapsed)
        {
            Debug.Assert(!collapsed || hasCollapsedView, $"Cant show collapsed gui for {GetType().Name} module manager");
            if (collapsed)
            {
                EditorGUILayout.BeginHorizontal();
                EnablingOnGUI("");
                EditorGUIUtils.RichLabel(Define, fontStyle: FontStyle.Bold, width: 200);
                OnCollapsedGUI();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUIUtils.RichLabel(Define, TextAnchor.MiddleCenter, eatAllWidth: true, fontStyle: FontStyle.Bold);
                if (hasHowTo)
                    howTo.OnGUI();
                else
                    GUILayout.Label("this module is not yet documented");
                ExamplesOnGUI();
                DependanciesOnGUI();
                EnablingOnGUI("Enabled");
                if (compiled)
                    OnCompiledGUI();
                else
                    OnNotCompiledGUI();
            }
        }
        protected virtual string collapsedDescription => dependenciesDesc;
        protected virtual void OnCollapsedGUI() {
            if (hasDetailedView)
            {
                const float ButtonWidth = 150;
                if (GUILayout.Button("Details", GUILayout.Width(ButtonWidth)))
                    FriendsGamesToolsWindow.instance.OnShowModulePressed(this);
            }
            var desc = collapsedDescription;
            if (desc != null)
            {
                const int MaxLength = 45;
                if (desc.Length > MaxLength)
                {
                    desc = desc.Substring(0, MaxLength);
                    if (desc.EndsWith("<b"))
                        desc = desc.Substring(0, desc.Length - 2);
                    if (desc.EndsWith("</b"))
                        desc = desc.Substring(0, desc.Length - 3);
                    if (desc.EndsWith("</"))
                        desc = desc.Substring(0, desc.Length - 2);
                    if (desc.EndsWith("<"))
                        desc = desc.Substring(0, desc.Length - 1);
                    if (Regex.Matches(desc, "<b>").Count > Regex.Matches(desc, "</b>").Count)
                        desc += "</b>";
                    desc += "...";
                }
                EditorGUIUtils.RichMultilineLabel(desc, width: 350);
            }
        }
        protected virtual void OnNotCompiledGUI() { }
        protected virtual void OnCompiledGUI() { }

        public virtual void OnEnable()
        {
            if (compiled)
                OnCompiledEnable();
            InitEnabling();
            UpdateDebugPanel();
        }
        protected virtual void OnCompiledEnable() { }
        public virtual void OnDisable() => UpdateDebugPanel();
        public void OnFocus()
        {
            if (compiled)
                OnCompiledFocus();
        }
        protected virtual void OnCompiledFocus() { }
        public void OnLostFocus()
        {
            if (compiled)
                OnCompiledLostFocus();
        }
        protected virtual void OnCompiledLostFocus() { }
        public virtual void Update()
        {
            UpdateEnabling();
            if (compiled)
                OnCompiledUpdate();
        }
        protected virtual void OnCompiledUpdate() { }
        #endregion

        #region Enabling
        public virtual bool canBeEnabled => true; // Dont bother about dependancies here.
        public virtual bool enabledWithoutDefine => false;
        bool editable => canBeEnabled && !enabledWithoutDefine;
        bool isEnabledToggle;
        public void InitEnabling()
        {
            isEnabledToggle = compiled || enabledWithoutDefine;
        }
        private void UpdateEnabling()
        {
            if (FriendsGamesToolsWindow.batchDefines)
                return;
            var (addedDefine, removedDefine) = UpdateEnablingBatched();
            if (!string.IsNullOrEmpty(addedDefine))
                AddToCompilation();
            if (!string.IsNullOrEmpty(removedDefine))
                RemoveFromCompilation();
        }
        public (string addedModule, string removedModule) UpdateEnablingBatched()
        {
            if (isEnabledToggle != compiled)
            {
                if (isEnabledToggle)
                    return (Define, null);
                else
                    return (null, Define);
            }
            else
                return (null, null);
        }
        protected virtual void EnablingOnGUI(string toggleTitle)
        {
            const int width = 30;
            EditorGUIUtils.PushEnabling(editable);
            var nextIsEnabledToggle = EditorGUILayout.Toggle(toggleTitle, isEnabledToggle,
                    string.IsNullOrEmpty(toggleTitle) ? new GUILayoutOption[] { GUILayout.Width(width) } : new GUILayoutOption[] { });
            if (editable)
            {
                if (nextIsEnabledToggle && !isEnabledToggle && !(CheckDependancies(false) || FriendsGamesToolsWindow.batchDefines))
                    nextIsEnabledToggle = false;
                if (isEnabledToggle != nextIsEnabledToggle)
                {
                    isEnabledToggle = nextIsEnabledToggle;
                    OnEnableCheckboxChanged(isEnabledToggle);
                }
            }
            EditorGUIUtils.PopEnabling();
        }
        void DependanciesOnGUI()
        {
            var desc = dependenciesDesc;
            if (!string.IsNullOrEmpty(desc))
                EditorGUILayout.LabelField(desc);
        }
        string dependenciesDesc
        {
            get
            {
                string str = string.Empty;
                if (dependFromModules.Count > 0)
                    str = $"depends on {dependFromModules.PrintCollection(", ", "")} ";
                if (dependFromPackages.Count > 0)
                    str += $"depends on {dependFromPackages.PrintCollection(", ", "")} ";
                return str;
            }
        }
        protected virtual void OnEnableCheckboxChanged(bool isChecked) { }
        #endregion

        #region Compiling
        public abstract string Define { get; } // Define that makes module code compile.
        public bool compiled => DefinesModifier.DefineExists(Define);
        protected virtual void AddToCompilation()
        {
            if (!CheckDependancies(false))
                return;
            // Also add modules this one depends on.
            DefinesModifier.ModifyDefines(GetDependFromModulesRecursively(), null);
        }
        protected virtual void RemoveFromCompilation()
        {
            DefinesModifier.RemoveDefine(Define);
        }
        #endregion

        #region Dependancies
        public bool CheckDependancies(bool includingModules = true) => 
            (!includingModules || CheckDependanciesFromOtherModules()) 
            && CheckCollisionsWithOtherModules() 
            && CheckDependanciesFromPackages();
        public virtual List<string> dependFromModules => new List<string>(); // Defines of modules this module depend on.
        public List<string> GetDependFromModulesRecursively(List<string> parentModules = null)
        {
            var dependRecursively = parentModules ?? new List<string>();
            if (dependRecursively.Contains(Define))
                return dependRecursively;
            dependRecursively.Add(Define);
            foreach (var dependFromModuleDefine in dependFromModules)
                dependRecursively = FriendsGamesToolsWindow.GetModule(dependFromModuleDefine).GetDependFromModulesRecursively(dependRecursively);
            return dependRecursively;
        }
        protected bool CheckDependanciesFromOtherModules()
        {
            if (dependFromModules == null)
                return true;
            var notFilledDependancies = FriendsGamesToolsWindow.allModules.Filter(m
                    => !m.compiled && dependFromModules.Contains(m.Define));
            if (notFilledDependancies.Count > 0)
            {
                if (!FriendsGamesToolsWindow.batchDefines)
                    Debug.LogError($"cant enable {Define}," +
                        $" enable {notFilledDependancies.ConvertAll(m => m.Define).PrintCollection(", ")} module(s) first");
                return false;
            }
            return true;
        }
        public virtual List<string> dependFromPackages => new List<string>(); // Defines of packages this module depend on.
        protected bool CheckDependanciesFromPackages()
        {
            if (dependFromPackages == null)
                return true;
            var notFilledDependancies = dependFromPackages.Filter(package 
                => !PackagesManager.IsInProject(package));
            if (notFilledDependancies.Count > 0)
            {
                if (!FriendsGamesToolsWindow.batchDefines)
                    Debug.LogError($"cant enable {Define}," +
                    $" enable {notFilledDependancies.PrintCollection(", ")} package(s) first (Menu->Window->Package Manager)");
                return false;
            }
            return true;
        }
        public virtual List<string> collidesWithModules => new List<string>(); // Cant be enabled with.
        protected bool CheckCollisionsWithOtherModules()
        {
            if (collidesWithModules == null)
                return true;
            var currentCollisions = FriendsGamesToolsWindow.allModules.Filter(m
                    => m.compiled && collidesWithModules.Contains(m.Define));
            if (currentCollisions.Count > 0)
            {
                if (!FriendsGamesToolsWindow.batchDefines)
                    Debug.LogError($"cant enable {Define}," +
                        $" disable {currentCollisions.ConvertAll(m => m.Define).PrintCollection(", ")} module(s) first");
                return false;
            }
            return true;
        }
        #endregion

        #region How to
        public virtual HowToModule HowTo() => null;
        private HowToModule _howTo;
        bool howToInited;
        public HowToModule howTo
        {
            get
            {
                if (!howToInited)
                {
                    _howTo = HowTo();
                    howToInited = true;
                }
                return _howTo;
            }
        }
        public bool hasHowTo => howTo != null;
        public virtual string DoReleaseChecks() => null;
        protected bool PlatformValid(TargetPlatform platform, Func<bool> check, StringBuilder sb, string error)
        {
            if (!BuildModeSettings.instance.PlatformEnabled(platform))
                return true;
            if (!check())
            {
                sb?.AppendLine(error);
                return false;
            }
            return true;
        }
        #endregion

        #region Examples
        void ExamplesOnGUI()
        {
#if EXAMPLES
            var examples = SettingsInEditor<Examples.ExamplesSettings>.GetSettingsInstance(false);
            if (examples == null)
                return;
            var currModuleExamples = examples.examples.Filter(e => e.forModules.Contains(Define));
            if (currModuleExamples.Count == 0)
                return;
            GUILayout.Label("Examples:");
            currModuleExamples.ForEach(e =>
            {
                EditorGUILayout.ObjectField(e.scene.m_SceneAsset, typeof(SceneAsset), false);
            });
#endif
        }
        #endregion

        #region Debug panel
        protected virtual string debugViewPath => null;
        private FGTModuleDebugPanel GetDebugPanelPrefab()
        {
            var path = debugViewPath;
            if (string.IsNullOrEmpty(path)) return null;
            path = $"{FriendsGamesManager.MainPluginFolder}/{path}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<FGTModuleDebugPanel>(path);
            Debug.Assert(prefab!=null, $"Debug panel for module {Define} not found at {path}");
            return prefab;
        }
        public void UpdateDebugPanel() => DebugPanelItemView.UpdateAddToSettings(GetDebugPanelPrefab());
        #endregion
    }
    public abstract class RootModule : ModuleManager
    {
        public override string parentModule => FGTRootModule.define;
    }
}


