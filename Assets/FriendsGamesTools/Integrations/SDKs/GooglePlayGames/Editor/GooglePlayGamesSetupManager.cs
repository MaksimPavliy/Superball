#if GOOGLE_PlAY_GAMES
using GooglePlayGames.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class GooglePlayGamesSetupManager : LibrarySetupManager<GooglePlayGamesSetupManager>
    {
        public override HowToModule HowTo() => new GooglePlayGamesSetupManager_HowTo();

        public override string SomeClassNameWithNamespace => "GooglePlayGames.PluginVersion";
        public const string define = "GOOGLE_PlAY_GAMES";
        public override string Define => define;

#if !GOOGLE_PlAY_GAMES
        public override bool configured => false;
#else
        public override bool configured
        {
            get
            {
                if (!inited)
                    Init();
                return resourceXmlDataLooksValid && settingsClassGenerated;
            }
        }
#endif
        protected override string SDKDownloadURL 
            => "https://github.com/playgameservices/play-games-plugin-for-unity/archive/master.zip";
        protected override bool canCheckUpdate => true;

#if GOOGLE_PlAY_GAMES
        bool inited;
        void Init()
        {
            LoadResourceXmlData();
            inited = true;
        }
        string resourceXmlData;
        void LoadResourceXmlData() => resourceXmlData = GPGSProjectSettings.Instance.Get(GPGSUtil.ANDROIDRESOURCEKEY, resourceXmlData);
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            Init();
        }
        bool resourceXmlDataLooksValid {
            get {
                if (string.IsNullOrEmpty(resourceXmlData))
                {
                    LoadResourceXmlData();
                    return false;
                }
                if (!resourceXmlData.Contains("<?xml version"))
                    return false;
                var appIdSet = resourceXmlData.GetLineWith("\"app_id\"").HasDigits();
                if (!appIdSet)
                    return false;
                var packageNameLine = resourceXmlData.GetLineWith("\"package_name\"");
                if (!packageNameLine.Contains(ApplicationIdValidator.androidId))
                    return false;
                return true;
            }
        }
        bool settingsClassGenerated => ReflectionUtils.DoesClassExist("GPGSIds");
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            resourceXmlData = EditorGUILayout.TextField("resourceXmlData", resourceXmlData);
            if (GUILayout.Button("Setup"))
            {
                if (!resourceXmlDataLooksValid)
                {
                    Debug.LogError("please make xml valid");
                    return;
                }
                const string webClientId = "";
                const string classDirectory = "Assets/GooglePlayGames";
                const string className = "GPGSIds";
                GPGSAndroidSetupUI.PerformSetup(
                    webClientId, classDirectory, className, resourceXmlData, null);
            }
            if (!resourceXmlDataLooksValid)
                EditorGUIUtils.ColoredLabel("resourceXmlData does not look valid, " +
                    "it should contain current package name, " +
                    "google play games application id and be a valid xml", Color.red);
            else if (!settingsClassGenerated)
                EditorGUIUtils.ColoredLabel("resourceXmlData looks valid, press 'setup' button", EditorGUIUtils.warningColor);
        }
#endif
    }
}


