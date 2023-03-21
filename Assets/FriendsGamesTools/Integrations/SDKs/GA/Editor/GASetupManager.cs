using FriendsGamesTools.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using FriendsGamesTools.EditorTools.BuildModes;
#if GA
using GameAnalyticsSDK;
#endif


namespace FriendsGamesTools.Integrations
{
    public class GASetupManager : LibrarySetupManager<GASetupManager>, IAnalyticsModule
    {
        public const string define = "GA";
        public override string SomeClassNameWithNamespace => "GameAnalyticsSDK.GameAnalytics";
        public override string Define => define;
        public override HowToModule HowTo() => new GASetupManager_HowTo();
        protected override string SDKDownloadURL 
            => "http://download.gameanalytics.com/unity/GA_SDK_UNITY.unitypackage";
        protected override bool canCheckUpdate => true;

#if !GA
        public override bool configured => false;
#else
        public override bool configured
        {
            get
            {
                int platformsCount = 0;
                if (GetPlatformAdded(TargetPlatform.Android))
                {
                    platformsCount++;
                    if (!GetPlatformConfigured(TargetPlatform.Android))
                        return false;
                }
                if (GetPlatformAdded(TargetPlatform.IOS))
                {
                    platformsCount++;
                    if (!GetPlatformConfigured(TargetPlatform.IOS))
                        return false;
                }
                return platformsCount > 0;
            }
        }
        GameAnalyticsSDK.Setup.Settings settings => GameAnalytics.SettingsGA;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            //if (GUILayout.Button("Select GA SDK settings"))
            //    EditorApplication.ExecuteMenuItem("Window/GameAnalytics/Select Settings");

            bool changed = false;
            ShowPlatform(TargetPlatform.IOS, ref changed);
            ShowPlatform(TargetPlatform.Android, ref changed);
            if (changed)
            {
                EditorUtils.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }
        void ShowPlatform(TargetPlatform platform, ref bool changed)
        {
            var ind = settings.Platforms.FindIndex(p => p == platform.ToRuntimePlatform());
            var enabled = ind != -1;
            if (EditorGUIUtils.Toggle(platform.ToString(), ref enabled, ref changed))
            {
                if (enabled)
                {
                    settings.AddPlatform(platform.ToRuntimePlatform());
                    ind = settings.Platforms.Count - 1;
                }
                else
                    settings.RemovePlatformAtIndex(ind);
            }
            if (!enabled)
                return;

            var gameKey = settings.GetGameKey(ind);
            if (EditorGUIUtils.TextField($"{platform} gameKey", ref gameKey, ref changed))
                settings.UpdateGameKey(ind, gameKey);

            var secretKey = settings.GetSecretKey(ind);
            if (EditorGUIUtils.TextField($"{platform} secretKey", ref secretKey, ref changed))
                settings.UpdateSecretKey(ind, secretKey);
        }
        bool GetPlatformAdded(TargetPlatform platform) 
            => settings.Platforms.Contains(platform.ToRuntimePlatform());
        bool GetPlatformConfigured(TargetPlatform platform)
        {
            var ind = settings.Platforms.FindIndex(p => p == platform.ToRuntimePlatform());
            if (ind == -1)
                return false;
            return !string.IsNullOrEmpty(settings.GetGameKey(ind)) && !string.IsNullOrEmpty(settings.GetSecretKey(ind));
        }
        public override string DoReleaseChecks()
        {
            var err = base.DoReleaseChecks();
            if (BuildModeSettings.instance.IOSEnabled && !GetPlatformConfigured(TargetPlatform.IOS))
                err += "\nios platform is not configured";
            if (BuildModeSettings.instance.AndroidEnabled && !GetPlatformConfigured(TargetPlatform.Android))
                err += "\nandroid platform is not configured";
            return err;
        }
#endif
    }
}


