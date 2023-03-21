using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.Ads
{
    public class AdsModule : RootModule
    {
        public const string define = "ADS";
        public override string Define => define;
        public override HowToModule HowTo() => new AdsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(WindowsModule.define);
        protected override string debugViewPath => "Ads/Debug/AdsDebugView";

#if ADS
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            InitSources();
            releaseErrors = DoReleaseChecks();
        }
        string GetStringOption(AdSourceType adsType)
        {
            switch (adsType)
            {
                case AdSourceType.mocked: return MockingOption;
                case AdSourceType.omitShowingAndSuccess: return OmitAdsOption;
                default:
                case AdSourceType.real:
                    var ind = sourceManagers.FindIndex(s => s.FullName == config.selectedManagerFullName);
                    if (string.IsNullOrEmpty(config.selectedManagerFullName) || ind == -1)
                        return string.Empty;
                    return sourceOptions[ind];
            }
        }
        void OnAdTypeGUI(string title, ref AdSourceType type, ref bool changed, bool autoSelectReal = true)
        {
            var optionString = GetStringOption(type);
            if (EditorGUIUtils.Toolbar(title, ref optionString, sourceOptions, ref changed))
            {
                if (optionString == MockingOption)
                    type = AdSourceType.mocked;
                else if (optionString == OmitAdsOption)
                    type = AdSourceType.omitShowingAndSuccess;
                else
                {
                    type = AdSourceType.real;
                    config.selectedManagerFullName = sourceManagers[sourceOptions.IndexOf(optionString)].FullName;
                }
            }
            if (autoSelectReal && string.IsNullOrEmpty(optionString) && sourceManagers.Count > 0)
            {
                type = AdSourceType.real;
                changed = true;
            }
        }
        public static void UpdateAutoSelectedAdsManager() {
            InitSources();
            if (sourceManagers.Count == 0 || !config.selectedManagerFullName.IsNullOrEmpty())
                return;
            config.selectedManagerFullName = sourceManagers[0].FullName;
            config.SetChanged();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            EditorGUIUtils.Toggle("mocked ads simulate no ads available", ref config.mockedAdsSimulateNoAds, ref changed, labelWidth: 300);
            OnAdTypeGUI("ads in editor", ref config.typeInEditor, ref changed, false);
            OnAdTypeGUI("ads in develop build", ref config.typeInDevelopBuild, ref changed);
            OnAdTypeGUI("ads in test build", ref config.typeInTestBuild, ref changed);
            OnAdTypeGUI("ads in release build", ref config.typeInReleaseBuild, ref changed);
            UpdateAutoSelectedAdsManager();
            if (config.typeInReleaseBuild == AdSourceType.omitShowingAndSuccess)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("I confirm that its ok to omit ads in release", GUILayout.Width(280));
                EditorGUIUtils.Toggle("", ref config.omitAdsConfirmed, ref changed);
                GUILayout.EndHorizontal();
            }
            if (!string.IsNullOrEmpty(releaseErrors))
                EditorGUIUtils.Error(releaseErrors);
            if (changed)
            {
                EditorUtils.SetDirty(config);
                releaseErrors = DoReleaseChecks();
            }
        }
        string releaseErrors;
        StringBuilder sb = new StringBuilder();
        public override string DoReleaseChecks()
        {
            sb.Clear();
            if (config.typeInReleaseBuild == AdSourceType.mocked)
                sb.AppendLine("Debug gray ad windows enabled");
            if (config.typeInReleaseBuild == AdSourceType.omitShowingAndSuccess && !config.omitAdsConfirmed)
                sb.AppendLine("Omit ads probably should be turned off before release");
            return sb.ToString();
        }
        static AdsSettings config => SettingsInEditor<AdsSettings>.instance;
        static List<Type> sourceManagers;
        static string[] sourceOptions;
        const string MockingOption = "GrayDebugWindows";
        const string OmitAdsOption = "OmitAds";
        static void InitSources()
        {
            if (sourceManagers != null)
                return;
            sourceManagers = new List<Type>();
            var IAdsSource = typeof(IAdsSource);
            var MockingAdsSource = typeof(MockingAdsSource);
            ReflectionUtils.IterateTypes(type => {
                if (IAdsSource.IsAssignableFrom(type) && IAdsSource != type && MockingAdsSource != type)
                    sourceManagers.Add(type);
            }, true);
            var sourceOptions = new List<string>();
            sourceOptions.Clear();
            sourceManagers.ForEach(s => sourceOptions.Add(s.Name.Replace("Manager", "")));
            sourceOptions.Add(MockingOption);
            sourceOptions.Add(OmitAdsOption);
            AdsModule.sourceOptions = sourceOptions.ToArray();
        }
#endif
    }
}