using FriendsGamesTools.Analytics;
using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class FlurrySetupManager : LibrarySetupManager<FlurrySetupManager>, IAnalyticsModule
    {
        public override string SomeClassNameWithNamespace => "FlurrySDK.Flurry";
        public override string Define => "FLURRY";
        public override HowToModule HowTo() => new FlurrySetupManager_HowTo();
        protected override string SDKDownloadURL
            => "https://github.com/flurry/unity-flurry-sdk/archive/master.zip";
        protected override bool FilterPackageNameInZip(string packagePath)
            => base.FilterPackageNameInZip(packagePath) && !packagePath.Contains("push");
        protected override int GetPackageIndToImport(List<string> unitypackageFiles)
        {
            var names = unitypackageFiles.Clone();
            names.Sort();
            return unitypackageFiles.IndexOf(names.Last());
        }
        protected override bool canCheckUpdate => true;

#if !FLURRY
        public override bool configured => false;
#else
        FlurrySettings settings => SettingsInEditor<FlurrySettings>.instance;
        void Save() => EditorUtility.SetDirty(settings);
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            EditorGUIUtils.TextField($"Flurry App Key", ref settings.key, ref changed);
            if (changed)
                Save();
        }
        public override bool configured => !string.IsNullOrEmpty(settings.key);
#endif
    }
}