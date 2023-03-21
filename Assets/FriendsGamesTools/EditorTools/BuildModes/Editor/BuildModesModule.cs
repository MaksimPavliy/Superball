using FriendsGamesTools.ModulesUpdates;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class BuildModesModule : ModuleManager
    {
        public const string define = "BUILD_MODES";
        public override string Define => define;
        public override string parentModule => EditorToolsModule.define;
        public override HowToModule HowTo() => new BuildModesModule_HowTo();
        public override bool enabledWithoutDefine => true;

        public static BuildModeSettings settings => SettingsInEditor<BuildModeSettings>.instance;
        private static void SetModePrivate(BuildModeType mode)
        {
            settings._mode = mode;
            EditorUtility.SetDirty(settings);
        }

#if BUILD_MODES
        protected override void OnCompiledEnable() => SettingsInEditor<BuildModeSettings>.EnsureExists();

        protected override void OnCompiledGUI() {
            base.OnCompiledGUI();
            var changed = false;

            // Change mode.
            EditorGUIUtils.RichMultilineLabel(BuildInfo.instance.ToString());
            OnModifyModeGUI(ref changed);
            OnModifyBuildVersionAndNumberGUI(ref changed);

            // Other settings and actions.
            GUILayout.Space(20);
            if (GUILayout.Button("do release checks")) {
                DoAllReleaseChecks();
                changed = true;
            }
            if (!string.IsNullOrEmpty(settings.releaseErrors)) {
                EditorGUIUtils.WithColor(Color.red, () => EditorGUIUtils.RichMultilineLabel(settings.releaseErrors));
                if (GUILayout.Button("clear errors")) {
                    settings.releaseErrors = null;
                    changed = true;
                }
            }
            EditorGUIUtils.Toggle($"{TargetPlatform.IOS} setup required", ref settings.IOSEnabled, ref changed);
            EditorGUIUtils.Toggle($"{TargetPlatform.Android} setup required", ref settings.AndroidEnabled, ref changed);

            ProjectSettingItemManager.OnGUI(ref changed);
            //if (GUILayout.Button("setup multidex"))
            //    MultiDexSetupManager.Setup();

            if (changed)
                Save();
        }
        void Save() {
            settings.SetChanged();
            BuildInfoManagerEditor.InitBuildInfoSettings();
        }
        void OnModifyBuildVersionAndNumberGUI(ref bool changed)
        {
            var version = PlayerSettings.bundleVersion;
            if (EditorGUIUtils.TextField("version", ref version, ref changed))
                PlayerSettings.bundleVersion = version;

            var buildNumber = BuildInfoManagerEditor.buildNumber;
            if (EditorGUIUtils.IntField("build number", ref buildNumber, ref changed))
                BuildInfoManagerEditor.buildNumber = buildNumber;
        }
        public override void ShortcutOnGUI()
        {
            base.ShortcutOnGUI();
            var changed = false;
            OnModifyModeGUI(ref changed);
            OnModifyBuildVersionAndNumberGUI(ref changed);
            if (changed)
                Save();
        }
        void OnModifyModeGUI(ref bool changed)
        {
            if (EditorGUIUtils.Toolbar("", ref settings._mode, ref changed))
                SetMode(BuildModeSettings.mode);
        }

        public static void SetMode(BuildModeType mode)
        {
            SetModePrivate(mode);
            
            // Add capabilities only on release.
            // (Adding iap capability prevents building with wildcard)
            var path = "ProjectSettings/ProjectSettings.asset";
            var text = File.ReadAllText(path).WithLineEndings("\n");
            var automaticallyAddCapabilities = settings._mode == BuildModeType.Release;
            text = text.ReplaceLineWith("iOSAutomaticallyDetectAndAddCapabilities", 
                $"  iOSAutomaticallyDetectAndAddCapabilities: {(automaticallyAddCapabilities?1:0)}");
            File.WriteAllText(path, text);
            
            // Release checks from modules.
            if (settings._mode == BuildModeType.Release)
                DoAllReleaseChecks();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DoAllReleaseChecks()
        {
            var sb = new StringBuilder();

            // Check all modules.
            FriendsGamesToolsWindow.allModules.ForEach(m => {
                if (!m.compiled)
                    return;
                var err = m.DoReleaseChecks();
                if (!string.IsNullOrEmpty(err))
                    sb.Append($"{m.Define}:\n{err}\n");
                if (!m.CheckDependancies())
                    sb.AppendLine($"dependancies error: module {m.Define} might not work properly");
            });

            ProjectSettingItemManager.GetAllReleaseCheckErrors(sb);
            var notCompletedChanges = ChangesEditorUI.DoReleaseChecks();
            if (!string.IsNullOrEmpty(notCompletedChanges))
                sb.AppendLine(notCompletedChanges);

            settings.releaseErrors = sb.ToString();
            settings.SetChanged();

            return;
        }
#else
        public static void SetMode(BuildModeType mode) => SetModePrivate(mode);
        public static void DoAllReleaseChecks() { }
#endif
    }
}