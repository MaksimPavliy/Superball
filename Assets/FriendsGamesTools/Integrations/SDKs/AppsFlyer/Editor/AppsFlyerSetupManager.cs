using System.Collections.Generic;
using System.Text;
using FriendsGamesTools.Analytics;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    // How to for unity from appsflyer  https://support.appsflyer.com/hc/en-us/articles/213766183-AppsFlyer-SDK-Integration-Unity
    // Test on android https://support.appsflyer.com/hc/en-us/articles/210213753

    public class AppsFlyerSetupManager : LibrarySetupManager<AppsFlyerSetupManager>, IAnalyticsModule
    {
        public const string define = "APPS_FLYER";
        public override string SomeClassNameWithNamespace => "AppsFlyerSDK.AppsFlyer";
        public override string Define => define;
        // Find latest release at https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/releases
        protected override string SDKDownloadURL => "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/raw/master/appsflyer-unity-plugin-6.3.2.unitypackage";
        protected override bool canCheckUpdate => false;
        public override HowToModule HowTo() => new AppsFlyerSetupManager_HowTo();

#if APPS_FLYER
        public override bool configured {
            get
            {
                if (!inited)
                    Init();
                return isManifestOK && DevKeyValid() && FGTSettingsUtils.AppleAppIdValid();
            }
        }
        static StringBuilder sb = new StringBuilder();
        public override string DoReleaseChecks()
        {
            sb.Clear();
            ManifestValid(sb);
            DevKeyValid(sb);
            FGTSettingsUtils.AppleAppIdValid(sb);
            return base.DoReleaseChecks();
        }
        bool inited;
        void Init()
        {
            UpdateManifestStatus();
            inited = true;
        }
        protected override void OnCompiledFocus()
        {
            base.OnCompiledFocus();
            Init();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            EditorGUIUtils.InHorizontal(()=> {
                EditorGUIUtils.ShowValid(isManifestOK);
                if (isManifestOK)
                    GUILayout.Label("manifest setup ok");
                else if (GUILayout.Button("Setup android manifest"))
                    ModifyManifest();
            });

            EditorGUIUtils.InHorizontal(()=> {
                var changed = false;
                EditorGUIUtils.ShowValid(DevKeyValid());
                EditorGUIUtils.TextField("APPSFLYER_DEV_KEY", ref settings.APPSFLYER_DEV_KEY, ref changed);
                if (changed)
                    settings.SetChanged();
            });
            EditorGUIUtils.InHorizontal(()=> {
                FGTSettingsUtils.AppleAppIdValid();
                FGTSettingsUtils.AppleAppIdInput();
            });
        }
#else
        public override bool configured => false;
#endif

        #region Manifest
        bool isManifestOK;
        void UpdateManifestStatus()
        {
            var manifest = new AndroidManifestManager();
            isManifestOK = true;
            foreach (var permission in permissions)
            {
                if (!manifest.ContainsPermission(permission))
                {
                    isManifestOK = false;
                    return;
                }
            }
        }
        bool ManifestValid(StringBuilder sb = null)
        {
            if (!isManifestOK)
            {
                sb?.AppendLine("android manifest is not set up");
                return false;
            }
            return true;
        }
        static List<AndroidManifestManager.Permission> permissions = new List<AndroidManifestManager.Permission> {
            AndroidManifestManager.Permission.ACCESS_NETWORK_STATE,
            AndroidManifestManager.Permission.ACCESS_WIFI_STATE,
            AndroidManifestManager.Permission.INTERNET,
         //   AndroidManifestManager.Permission.READ_PHONE_STATE - optional.
        };
        void ModifyManifest()
        {
            var manifest = new AndroidManifestManager();
            foreach (var permission in permissions)
                manifest.EnsurePermisson(permission);
            manifest.Save();
            UpdateManifestStatus();
        }
        #endregion

        #region Settings
#if APPS_FLYER
        AppsFlyerSettings settings => SettingsInEditor<AppsFlyerSettings>.instance;
        bool DevKeyValid(StringBuilder sb = null)
        {
            if (settings.APPSFLYER_DEV_KEY.IsNullOrEmpty())
            {
                sb?.AppendLine("APPSFLYER_DEV_KEY is not set up");
                return false;
            }
            return true;
        }
#endif
#endregion
    }
}

