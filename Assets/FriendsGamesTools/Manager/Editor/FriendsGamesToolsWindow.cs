using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using FriendsGamesTools.Integrations;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.PushNotifications;
using FriendsGamesTools.DebugTools;
using FriendsGamesTools.Integrations.MaxSDK;
using FriendsGamesTools.EditorTools;
using FriendsGamesTools.EditorTools.Screenshots;
using FriendsGamesTools.EditorTools.BuildModes;
using FriendsGamesTools.ABTests;
using FriendsGamesTools.Reskiner;
using FriendsGamesTools.Analytics;
using FriendsGamesTools.Ads;
using FriendsGamesTools.IAP;
using FriendsGamesTools.ECSGame.DataMigration;
using FriendsGamesTools.ECSGame.ActiveIdle;
using FriendsGamesTools.DebugTools.ChromaKey;
using FriendsGamesTools.ECSGame.Tutorial;
using FriendsGamesTools.ModulesUpdates;
using FriendsGamesTools.EditorTools.AutoBalance;
using FriendsGamesTools.Share;
using FriendsGamesTools.EditorTools.Upload;

namespace FriendsGamesTools
{
    public class FriendsGamesToolsWindow : EditorWindow
    {
        public static FriendsGamesToolsWindow instance { get; private set; }
        [MenuItem(FriendsGamesManager.MainPluginName + "/Menu")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            instance = (FriendsGamesToolsWindow)GetWindow(typeof(FriendsGamesToolsWindow), false, FriendsGamesManager.MainPluginName);
            instance.Show();
        }

        #region Modules navigation
        static List<ModuleManager> modules = new List<ModuleManager>() {
            new FGTRootModule(),

            new ECSModuleFolder(),
            new HCTemplateModule(),
            new GameRootModule(),
            new UpgradableQualityModule(),
            new UpgradableCountModule(),
            new UnlockModule(),
            new PlayerModule(),
            new PlayerMoneyModule(),
            new HardCurrencyModule(),
            new PlayerLevelModule(),
            new IncumeForVideoModule(),
            new GameTimeModule(),
            new BonusModule(),
            new LocationsModule(),
            new TrajectoriesModule(),
            new RateAppAdvancedModule(),
            new SaveMigrationModule(),
            new ActiveIdleSpeeUpModule(),
            new LevelBasedModule(),
            new SkinsModule(),
            new MoneySkinsModule(),
            new ProgressSkinsModule(),

            new ECSISOModuleFolder(),
            new MoveCameraModule(),
            new TrajectoriesIsoModule(),
            new HumanModule(),

            new IntegrationsModule(),
            new MaxSDKSetupManager(),
            new GoogleMobileAdsSetupManager(),
            new GoogleMobileAdsTestSuiteSetupManager(),
            new FBSetupManager(),
            new AppsFlyerSetupManager(),
            new GooglePlayGamesSetupManager(),
            new AppLovinSetupManager(),
            new GASetupManager(),
            new FlurrySetupManager(),
            new GDPRModule(),

            new AdsModule(),
            new IAPModule(),
            new AnalyticsModule(),
            new LocalizationModule(),

            new PushNotificationsModuleFolder(),
            new MobileNotificationsWrapperModule(),
            new ManagerModule(),
            new ShareModule(),
            new CrossMarketingModule(),

            new DebugToolsModuleFolder(),
            new DebugPanelModule(),
            new DebugPerformanceModule(),
            new DebugLogsOnDeviceModule(),
            new DebugPanelConfigModule(),
            new ChromaKeyModule(),

            new EditorToolsModule(),
            new UpdateArtModule(),
            new ScreenshotsModule(),
            new BuildModesModule(),
            new AutoBalanceModule(),
            new UploadToStoreModule(),

            new CameraModule(),
            new TouchesModule(),
            new UIModule(),
            new WindowsModule(),
            new InfoNotificationModule(),
            new SettingsModule(),

            new AudioModule(),
            new OtherModule(),
            new RateAppBasicModule(),
            new PluginsModule(),
            new ReskinerModule(),
            new TutorialModule(),
            new QuestsModule(),
            new UtilsModule(),
            new ABTestModule(),
            new HapticModule(),
        };
        public static IReadOnlyList<ModuleManager> allModules => modules;
        public static T GetModule<T>() where T : ModuleManager
            => allModules.Find(m => m.GetType() == typeof(T)) as T;
        public static ModuleManager GetModule(string define) => allModules.Find(m => m.Define == define);
        FGTLocalSettings settings => FGTRootModule.settings;
        public ModuleManager shownParenModule => shownModules[0];
        bool rootShown => shownParenModule.Define == FGTRootModule.define;
        private List<ModuleManager> shownModules = new List<ModuleManager>();
        void InitModulesNavigation()
        {
            if (string.IsNullOrEmpty(settings.shownParentModuleDefine))
                settings.shownParentModuleDefine = FGTRootModule.define;
            ShowModule(settings.shownParentModuleDefine);
        }
        void ShowModule(string define)
        {
            UpdateBatchDefines(false);
            settings.shownParentModuleDefine = define;
            bool ModuleShouldBeShown(ModuleManager module)
            {
                if (settings.showAllModules && define == FGTRootModule.define)
                    return true;
                else
                    return (module.parentModule == settings.shownParentModuleDefine) // Is child.
                    || (module.Define == FGTRootModule.define && settings.shownParentModuleDefine == FGTRootModule.define) // Is root and root shown.
                    || (module.Define == settings.shownParentModuleDefine); // Is shown parent module.
            }
            var remainingModules = shownModules.Filter(m => ModuleShouldBeShown(m));
            var hiddenModules = shownModules.Filter(m => !remainingModules.Contains(m));
            hiddenModules.ForEach(m => {
                if (focused)
                    m.OnLostFocus();
                m.OnDisable();
            });
            shownModules = modules.Filter(m => ModuleShouldBeShown(m));
            if (settings.alphabetOrder)
            {
                shownModules.SortBy(m => m.Define);
                var shownRoot = shownModules.Find(m=>m.Define== FGTRootModule.define);
                if (shownRoot != null)
                {
                    shownModules.Remove(shownRoot);
                    shownModules.Insert(0, shownRoot);
                }
            }
            shownModules.ForEach(m => {
                if (remainingModules.Contains(m))
                    return;
                m.OnEnable();
            });
            shownModules.ForEach(m=> {
                if (remainingModules.Contains(m))
                    return;
                if (focused)
                    m.OnFocus();
            });
        }
        public void OnShowModulePressed(ModuleManager module)
        {
            ShowModule(module.Define);
        }
        #endregion

        #region Transfer unity events (enabling, focusing) to modules
        private void OnEnable()
        {
            instance = this;
            if (FriendsGamesManagerEditor.inited)
                InitIfNeeded();
        }
        public bool inited => shownModules.CountSafe() > 0;
        public void InitIfNeeded()
        {
            if (inited) return;
            InitModulesNavigation();
            shownModules.ForEach(m => m.OnEnable());
            ChangesEditorUI.OnEnable();
        }
        private void OnDisable()
        {
            shownModules.ForEach(m => m.OnDisable());
        }
        bool focused;
        private void OnFocus()
        {
            focused = true;
            EditorAsync.ExecuteAfter(() => inited, () => shownModules.ForEach(m => m.OnFocus()));
        }
        private void OnLostFocus()
        {
            focused = false;
            shownModules.ForEach(m => m.OnLostFocus());
        }
        #endregion

        Vector2 scrollPos;
        void OnGUI()
        {
            if (!inited) return;
            UpdateBatchDefines();
            EditorGUIUtils.PushEnabling(!EditorApplication.isCompiling);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (ChangesEditorUI.shown)
                ChangesEditorUI.OnGUI();
            else if (FGTWizard.shown)
                FGTWizard.OnGUI();
            else
                CommonOnGUI();
            EditorGUILayout.EndScrollView();
            EditorGUIUtils.PopEnabling();
        }
        void CommonOnGUI()
        {
            EditorGUIUtils.PushEnabling(!rootShown);
            if (EditorApplication.isCompiling)
                EditorGUIUtils.RichLabel("waiting compilation...", TextAnchor.MiddleCenter, true, false, EditorGUIUtils.warningColor, FontStyle.Bold, true);
            else
            {
                if (GUILayout.Button("back"))
                    ShowModule(shownParenModule.parentModule);
            }
            EditorGUIUtils.PopEnabling();
            shownModules.ForEach(m =>
            {
                bool collapsed;
                if (shownParenModule == m)
                    collapsed = false;
                else
                    collapsed = true;
                if (collapsed && !m.hasCollapsedView)
                    collapsed = false;
                if (!collapsed && !m.hasDetailedView)
                    collapsed = true;

                var showChildTitles = shownParenModule == m && shownModules.Count > 1;
                //if (showChildTitles)
                //    EditorGUIUtils.RichLabel("Shown module:", TextAnchor.MiddleCenter, eatAllWidth: true, fontStyle: FontStyle.BoldAndItalic);
                m.OnGUI(collapsed);
                if (m.Define == FGTRootModule.define)
                    ShortcutsOnGUI();

                if (showChildTitles)
                    EditorGUIUtils.RichLabel("Submodules:", TextAnchor.MiddleCenter,
                        eatAllWidth: true, fontStyle: FontStyle.BoldAndItalic);
            });
        }
        void ShortcutsOnGUI()
        {
            allModules.ForEach(m=> {
                if (m.compiled)
                    m.ShortcutOnGUI();
            });
        }
        private void Update()
        {
            if (EditorApplication.isCompiling)
                return;
            shownModules.ForEach(m => m.Update());
        }

        public static bool batchDefines { get; private set; }
        private void UpdateBatchDefines()
        {
            UpdateBatchDefines(Event.current.control || Event.current.command);
        }
        private void UpdateBatchDefines(bool enabled)
        {
            if (batchDefines == enabled)
                return;
            batchDefines = enabled;
            if (enabled)
            {
            } else
            {
                var added = new List<string>();
                var removed = new List<string>();
                shownModules.ForEach(m =>
                {
                    var (currAdded, currRemoved) = m.UpdateEnablingBatched();
                    if (!string.IsNullOrEmpty(currAdded))
                        added.Add(currAdded);
                    if (!string.IsNullOrEmpty(currRemoved))
                        removed.Add(currRemoved);
                });
                if (added.Count > 0 || removed.Count > 0)
                    DefinesModifier.ModifyDefines(added, removed);
            }
        }
    }
}
