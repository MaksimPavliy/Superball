#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace FriendsGamesTools.Integrations
{
    public abstract class LibrarySetupManager : ModuleManager
    {
        public abstract string SomeClassNameWithNamespace { get; }
        bool _libraryCodeExists, _libraryCodeInited;
        public bool LibraryCodeExists {
            get {
                if (!_libraryCodeInited) {
                    _libraryCodeInited = true;
                    _libraryCodeExists = ReflectionUtils.DoesClassExist(SomeClassNameWithNamespace);
                }
                return _libraryCodeExists;
            }
        }
        public abstract bool configured { get; }
        public override string DoReleaseChecks() => configured ? null : "not configured";
        public override string parentModule => IntegrationsModule.define;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(IntegrationsModule.define);
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            ShowUpdateGUI();
            ShowDoesLibraryCodeExistGUI();
        }
        protected override void OnNotCompiledGUI()
        {
            base.OnNotCompiledGUI();
            ShowDoesLibraryCodeExistGUI();
            ShowDownloadSDKButton("download");
        }
        void ShowDoesLibraryCodeExistGUI()
        {
            if (LibraryCodeExists)
            {
                if (!configured)
                    EditorGUIUtils.ColoredLabel($"{Define} exists, but not configured", Color.red);
                else
                    EditorGUIUtils.ColoredLabel($"{Define} exists and configured", EditorGUIUtils.green);
            }
            else
                EditorGUIUtils.ColoredLabel($"{Define} not exists", Color.gray);
        }
        public override void Update()
        {
            base.Update();
            UpdateLibraryCompilation();
        }
        void UpdateLibraryCompilation()
        {
            if (LibraryCodeExists == compiled)
                return;
            if (compiled)
                RemoveFromCompilation();
            else if (!DefinesModifier.DefineExists(disabledDefine))
                AddToCompilation();
        }
        string disabledDefine => $"{Define}_DISABLED";
        protected override void OnEnableCheckboxChanged(bool isChecked)
        {
            var addedDefines = new List<string>();
            var removedDefines = new List<string>();
            if (isChecked)
            {
                addedDefines.Add(Define);
                removedDefines.Add(disabledDefine);
            } else
            {
                addedDefines.Add(disabledDefine);
                removedDefines.Add(Define);
            }

            if (!LibraryCodeExists && isChecked)
            {
                DownloadSDK();
                addedDefines.Clear();
            }

            DefinesModifier.ModifyDefines(addedDefines, removedDefines);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        protected virtual string SDKSharingURL => null;
        protected virtual string SDKDownloadURL
        {
            get
            {
                if (string.IsNullOrEmpty(SDKSharingURL))
                    return null;
                return SDKSharingURL.Replace("https://drive.google.com/open?id=", "https://drive.google.com/uc?export=download&id=");
            }
        }
        public bool canDownloadSDK => !string.IsNullOrEmpty(SDKDownloadURL);
        // Enabling/disabling adds define/disabledDefine. Cant be enabled when library code not exists - download sdk for that.
        public override bool canBeEnabled => LibraryCodeExists;
        void ShowDownloadSDKButton(string caption)
        {
            if (!canDownloadSDK)
                return;
            if (GUILayout.Button(caption))
                DownloadSDK();
        }
        void DownloadSDK() => DownloadUnzipAndImport(SDKDownloadURL);
        protected virtual bool canCheckUpdate => false;
        protected virtual void ShowUpdateGUI()
        {
            EditorGUIUtils.PushEnabling(canCheckUpdate);
            ShowDownloadSDKButton("check update");
            EditorGUIUtils.PopEnabling();
        }
        protected virtual bool unzipAfterDownload => SDKDownloadURL.EndsWith(".zip");
        protected virtual void DownloadUnzipAndImport(string sdkUrl)
            => LibrarySetupUtils.DownloadUnzipAndImport(sdkUrl, unzipAfterDownload,
                FilterPackageNameInZip, GetPackageIndToImport);
        protected virtual bool FilterPackageNameInZip(string packagePath)
            => !packagePath.Contains("sample") && !packagePath.Contains("old-builds");
        protected virtual int GetPackageIndToImport(List<string> unitypackageFiles) => -1;
        
    }
    public abstract class LibrarySetupManager<TSelf> : LibrarySetupManager
        where TSelf:LibrarySetupManager<TSelf>
    {
        public static TSelf instance { get; private set; }
        public static bool ExistsAndConfigured
        {
            get
            {
                if (instance == null) return false;
                instance.OnEnable();
                return instance.configured;
            }
        }
        public LibrarySetupManager() => instance = (TSelf)this;
    }
}
#endif