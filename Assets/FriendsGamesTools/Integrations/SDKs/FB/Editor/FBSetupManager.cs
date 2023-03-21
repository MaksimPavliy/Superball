#if FACEBOOK
using Facebook.Unity.Settings;
#endif
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
    public class FBSetupManager : LibrarySetupManager<FBSetupManager>, IAnalyticsModule
    {
        public override string SomeClassNameWithNamespace => "Facebook.Unity.Settings.FacebookSettings";
        public override string Define => "FACEBOOK";
        public override HowToModule HowTo() => new FBSetupManager_HowTo();
        protected override string SDKDownloadURL
            => "https://lookaside.facebook.com/developers/resources/?id=facebook-unity-sdk-11.0.0.zip";
        protected override bool canCheckUpdate => true;
        protected override void DownloadUnzipAndImport(string sdkUrl)
        {
            base.DownloadUnzipAndImport(sdkUrl);
            FixDllPlatforms();
        }

        private void FixDllPlatforms()
        {
            var dlls = AssetDatabase.FindAssets("Facebook.Unity.IOS").ToList();
            dlls.AddRange(AssetDatabase.FindAssets("Facebook.Unity.Android").ToList());
            dlls = dlls.ConvertAll(id => AssetDatabase.GUIDToAssetPath(id));
            Debug.Log(dlls.PrintCollection("\n"));
            foreach (var dllPath in dlls)
            {
                var importer = (PluginImporter)AssetImporter.GetAtPath(dllPath);
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SetCompatibleWithPlatform(BuildTarget.Android, dllPath.Contains("Plugins/Android"));
                importer.SetCompatibleWithPlatform(BuildTarget.iOS, dllPath.Contains("Plugins/iOS"));
                AssetDatabase.WriteImportSettingsIfDirty(dllPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

#if !FACEBOOK
        void OnFBExistsGUI() { }
        public override bool configured => false;
#else
        FacebookSettings pluginSettings;
        FBSettings config => SettingsInEditor<FBSettings>.instance;
        public string GetAppName()
        {
            InitIfNeeded();
            return config.FacebookAppName;
        }
        public string GetAppId()
        {
            InitIfNeeded();
            return config.FacebookAppId;
        }
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            Init();
        }
        bool inited;
        AndroidManifestManager manifest;
        void Init()
        {
            pluginSettings = EditorUtils.GetInstanceInProject<FacebookSettings>();
            if (pluginSettings == null)
            {
                // Try v9 plugin
                pluginSettings = ReflectionUtils.GetStaticField(typeof(FacebookSettings), "Instance", false) as FacebookSettings;
            }
            if (pluginSettings == null)
            {
                EditorApplication.ExecuteMenuItem("Facebook/Edit Settings");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                pluginSettings = EditorUtils.GetInstanceInProject<FacebookSettings>();
                Debug.Log($"created facebook settings ({pluginSettings != null})");
            }
            manifest = new AndroidManifestManager();
            inited = true;

            var appName = EditorUtils.GetPrivateField<List<string>>(pluginSettings, "appLabels")[0];
            var appId = EditorUtils.GetPrivateField<List<string>>(pluginSettings, "appIds")[0];
            var changed = config.FacebookAppName != appName || config.FacebookAppId != appId;
            if (config.FacebookAppName.IsNullOrEmpty() && config.FacebookAppId.IsNullOrEmpty() && !appName.IsNullOrEmpty() && !appId.IsNullOrEmpty())
            {
                config.FacebookAppName = appName;
                config.FacebookAppId = appId;
            }
            if (changed)
                Save();
        }
        public void InitIfNeeded()
        {
            if (!inited)
                Init();
        }
        protected override void OnCompiledLostFocus()
        {
            base.OnCompiledLostFocus();
            Save();
        }
        void Save()
        {
            if (Application.isPlaying)
                return;
            EditorUtils.SetPrivateField(pluginSettings, "appLabels", new List<string> { config.FacebookAppName });
            EditorUtils.SetPrivateField(pluginSettings, "appIds", new List<string> { config.FacebookAppId });
            //EditorUtils.CallPrivateMethod(settings, "SettingsChanged");
            EditorUtility.SetDirty(pluginSettings);
            config.SetChanged();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            //if (GUILayout.Button("log private contents"))
            //    Debug.Log(ReflectionUtils.GetMembersDescription(typeof(Facebook.Unity.Editor.ManifestMod)));

            //if (GUILayout.Button("fix dll"))
            //    FixDllPlatforms();
            var changed = false;
            EditorGUIUtils.TextField("Facebook App Name", ref config.FacebookAppName, ref changed);
            if (appNameIsDefault)
                EditorGUIUtils.ColoredLabel($"Facebook App Name has default value, please change it", Color.red);
            if (appNameNotAsGameName)
                EditorGUIUtils.ColoredLabel($"Facebook App Name is different from PlayerSettings.productName = {PlayerSettings.productName}", EditorGUIUtils.warningColor);
            EditorGUIUtils.TextField("Facebook APP ID", ref config.FacebookAppId, ref changed);
            if (!appIdLooksValid)
                EditorGUIUtils.ColoredLabel($"Facebook App Id should be set to a 15-16-digits number", Color.red);
            if (!manifestLooksValid)
            {
                if (GUILayout.Button("regenerate android manifest"))
                {
                    typeof(Facebook.Unity.Editor.ManifestMod).CallStaticMethod("GenerateManifest");
                    manifest = new AndroidManifestManager();
                }
                EditorGUIUtils.ColoredLabel($"Android manifest does not look valid", Color.red);
            }
            if (GUILayout.Button("show fb settings"))
                Selection.objects = new UnityEngine.Object[] { pluginSettings };
            OnLogsGUI(ref changed);
            if (GUILayout.Button("Save"))
                Save();
        }

        void OnLogsGUI(ref bool changed) => EditorGUIUtils.Toggle("logs", ref config.logs, ref changed);

        public override bool configured
        {
            get
            {
                InitIfNeeded();
                return !appNameIsDefault && appIdLooksValid && manifestLooksValid;
            }
        }
        private bool appNameIsDefault => config.FacebookAppName == "App Name" || string.IsNullOrEmpty(config.FacebookAppName);
        private bool appNameNotAsGameName => PlayerSettings.productName != config.FacebookAppName;
        private bool appIdLooksValid => config.FacebookAppId != null && (config.FacebookAppId.Length == 16 || config.FacebookAppId.Length == 15) && long.TryParse(config.FacebookAppId, out var number);
        string manifestEntry => $"<meta-data android:name=\"com.facebook.sdk.ApplicationId\" android:value=\"fb{config.FacebookAppId}\" />";
        private bool manifestLooksValid => !BuildModeSettings.instance.AndroidEnabled || manifest.contents.Contains(manifestEntry);
#endif
    }
}


