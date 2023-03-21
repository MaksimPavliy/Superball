#if MAX_SDK
using System;
using System.Collections.Generic;
using System.IO;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public abstract class MediationSetupManager
    {
        static List<Type> disabledMediations = new List<Type> { };// typeof(AdColonyMediationSettings), typeof(IronSourceMediationSettings) };
        static Dictionary<Mediations, MediationSetupManager> _instances;
        public static Dictionary<Mediations, MediationSetupManager> instances {
            get {
                if (_instances == null) {
                    _instances = new Dictionary<Mediations, MediationSetupManager>();
                    ReflectionUtils.ForEachDerivedType<MediationSetupManager>(t =>
                    {
                        if (!disabledMediations.Contains(t))
                            Activator.CreateInstance(t);
                    });
                }
                return _instances;
            }
        }
        public MediationSetupManager() => instances.Add(type, this);
        public abstract Mediations type { get; }
        public abstract string folder { get; }
        public string displayName { get {
                var s = type.ToString();
                int ind = s.IndexOf("_");
                if (ind != -1)
                    s = s.Substring(0, ind);
                return s;
            }
        }
        public MaxSDKSettings settings => SettingsInEditor<MaxSDKSettings>.instance;

        bool inited;
        public virtual void Init() {
            EnsureMediationsInstallQuiet();
            UpdateIOSSet();
            UpdateAndroidSet();
            inited = true;
        }
        
        public virtual void OnGUI(ref bool changed)
        {
            UpdateMediationInstalled();
            var shouldBeInstalled = settings.enabledMediations.Contains(type);
            var installed = currState == InstalledState.Installed;
            var canChange = true;
            if (type == Mediations.APPLOVIN_NETWORK) {
                canChange = false;
                shouldBeInstalled = installed;
            }
            EditorGUIUtils.PushEnabling(canChange);
            var toggleChanged = EditorGUIUtils.Toggle($"{displayName} mediation " +
                $"{(currState == InstalledState.StateIsLoading ? "(max sdk loading data...)" : "")}",
                ref shouldBeInstalled, ref changed);
            EditorGUIUtils.PopEnabling();
            if (currState == InstalledState.StateIsLoading || isLoading)
                return;
            if (shouldBeInstalled && !settings.enabledMediations.Contains(type)) {
                settings.enabledMediations.Add(type);
                changed = true;
            } else if (!shouldBeInstalled && settings.enabledMediations.Contains(type)) {
                settings.enabledMediations.Remove(type);
                changed = true;
            }
            if (installed != shouldBeInstalled) {
                if (shouldBeInstalled)
                    StartInstalling();
                else
                    Uninstall();
                installed = shouldBeInstalled;
                changed = true;
            }
            if (!installed)
                return;
            if (settings.ios.enabled && !iosSet) {
                EditorGUIUtils.ColoredLabel("IOS not set", Color.red);
                var (canIOS, whyCantSetIOS) = canSetIOS;
                if (canIOS) {
                    if (GUILayout.Button("Setup IOS")) {
                        SetupIOS();
                        SaveRefreshSettings();
                        UpdateIOSSet();
                    }
                } else
                    EditorGUIUtils.ColoredLabel(whyCantSetIOS, Color.red);
            }

            if (settings.android.enabled && !androidSet) {
                EditorGUIUtils.ColoredLabel("Android not set", Color.red);
                var (canAndroid, whyCantSetAndroid) = canSetAndroid;
                if (canAndroid) {
                    if (GUILayout.Button("Setup Android")) {
                        SetupAndroid();
                        SaveRefreshSettings();
                        UpdateAndroidSet();
                    }
                } else
                    EditorGUIUtils.ColoredLabel(whyCantSetAndroid, Color.red);
            }
        }

        bool isLoading => isLoadingMaxData || isDownloadingPlugin || isImportingPlugin;
        static bool isLoadingMaxData;
        static bool isDownloadingPlugin;
        static bool isImportingPlugin;
        PluginData maxData;
        async void StartInstalling() {
            //Debug.Log($"StartInstalling {type}");
            if (isLoadingMaxData) return;
            if (maxData == null) {
                isLoadingMaxData = true;
                await AppLovinIntegrationManager.Instance.LoadPluginData(data => maxData = data);
                isLoadingMaxData = false;
            }
            isDownloadingPlugin = true;
            var network = maxData.MediatedNetworks.Find(n => n.Name == type.ToString());
            isImportingPlugin = true;
            AssetDatabase.ImportPackageCallback onImportComplete = _ => isImportingPlugin = false;
            AssetDatabase.importPackageCompleted += onImportComplete;
            await AppLovinIntegrationManager.Instance.DownloadPlugin(network);
            await Awaiters.While(() => isImportingPlugin);
            AssetDatabase.importPackageCompleted -= onImportComplete;
            isDownloadingPlugin = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //await Awaiters.SecondsRealtime(1);
            await Awaiters.While(() => EditorApplication.isUpdating);
            //Debug.Log($"complete downloading, {folderFullPath}={Directory.Exists(folderFullPath)}");
        }

        void Uninstall() {
            //Debug.Log($"Uninstall {type}");
            EditorUtils.DeleteFolder(folderFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void SaveRefreshSettings()
        {
            EditorUtils.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public virtual bool GetIOSSet() => true;
        public virtual void SetupIOS() { }
        public virtual (bool can, string whyCant) canSetIOS => (true, "");
        bool _iosSet;
        public bool iosSet { get {
                if (!inited)
                    Init();
                return _iosSet;
            }
            private set => _iosSet = value;
        }
        void UpdateIOSSet() => iosSet = GetIOSSet();
        public bool iosOk => !installed || iosSet;

        public virtual bool GetAndroidSet() => true;
        public virtual void SetupAndroid() { }
        public virtual (bool can, string whyCant) canSetAndroid => (true, "");
        bool _androidSet;
        public bool androidSet {
            get {
                if (!inited)
                    Init();
                return _androidSet;
            }
            private set => _androidSet = value;
        }
        void UpdateAndroidSet() => androidSet = GetAndroidSet();
        public bool androidOk => !installed || androidSet;

        void EnsureMediationsInstallQuiet() {
            EditorUtils.CorrectCode("AssetDatabase.ImportPackage(path, true);", "AssetDatabase.ImportPackage(path, false);",
                "Assets/MaxSdk/Scripts/IntegrationManager/Editor/AppLovinIntegrationManager.cs");
        }

        #region MAX SDK communication
        static AppLovinIntegrationManagerWindow maxWindow;
        public static void FindMaxWindow()
        {
            if (maxWindow != null)
                return;
            var windows = (AppLovinIntegrationManagerWindow[])Resources.FindObjectsOfTypeAll(typeof(AppLovinIntegrationManagerWindow));
            if (windows.Length == 0)
                maxWindow = null;
            else
                maxWindow = windows[0];
        }
        protected enum InstalledState { Installed, NotInstalled, StateIsLoading }
        static PluginData pluginData;
        static AppLovinEditorCoroutine pluginDataLoading;
        InstalledState _currState = InstalledState.StateIsLoading;
        protected InstalledState currState {
            get {
                if (_currState == InstalledState.StateIsLoading)
                    UpdateMediationInstalled();
                return _currState;
            }
            private set => _currState = value;  } 
        public bool installed => currState == InstalledState.Installed;
        string folderFullPath => $"Assets/MaxSdk/Mediation/{folder}";
        void UpdateMediationInstalled()
        {
            if (type == Mediations.APPLOVIN_NETWORK) // AppLovin is installed always.
            {
                currState = InstalledState.Installed;
                return;
            }
            currState = Directory.Exists(folderFullPath) ? InstalledState.Installed : InstalledState.NotInstalled;
        }
        #endregion
    }
}
namespace AppLovinMax.Scripts.IntegrationManager.Editor { }
#endif