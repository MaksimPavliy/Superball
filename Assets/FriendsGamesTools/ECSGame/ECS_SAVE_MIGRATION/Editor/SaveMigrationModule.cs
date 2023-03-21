using FriendsGamesTools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class SaveMigrationModule : ECSModule
    {
        public static SaveMigrationModule instance { get; private set; }
        public SaveMigrationModule() => instance = this;
        public const string define = "ECS_SAVE_MIGRATION";
        public override string Define => define;
        public override List<string> dependFromModules => new List<string> { ECSModuleFolder.define, WindowsModule.define };
        public override HowToModule HowTo() => new SaveMigrationModule_HowTo();

        public void OnVersionIncreased(int oldVersion, int newVersion)
        {
#if ECS_SAVE_MIGRATION
            SaveCurrMetaDataForVersion(newVersion);
            GenerateMigrationClassIfPossible(oldVersion, newVersion);
#endif
        }
#if ECS_SAVE_MIGRATION

        #region Migrations
        static MigrationSettings settings => SettingsInEditor<MigrationSettings>.instance;
        MigrationSettings.VersionMetaData SaveCurrentVersionMeta(int version = -1)
        {
            if (version == -1)
                version = DataVersion.versionInd;
            var meta = WorldMetaData.CreateCurrent();
            meta.version = version;
            var savedMetaData = new MigrationSettings.VersionMetaData
            {
                version = version,
                metaData = meta.EncodeToString(),
                shownVersion = Application.version,
                whatsNew = ""
            };
            settings.metaDatas.RemoveAll(m => m.version == version);
            settings.metaDatas.Add(savedMetaData);
            EditorUtils.SetDirty(settings);
            return savedMetaData;
        }
        private void SaveCurrMetaDataForVersion(int version)
        {
            SaveCurrentVersionMeta(version);
            inited = false;
            InitIfNeeded();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            InitIfNeeded();
            OnVipePlayersGUI();
            ECSModuleFolder.ShowIncVersionGUI();
            ShowExistingMigrations(ref changed);
            DebugMigration(ref changed);
            UpdateWindow(ref changed);
            OnTestSavesGUI(ref changed);
            ShowErrors();
            if (changed)
                EditorUtils.SetDirty(settings);
        }
        public static bool MessageIfNotOnMaster() {
            if (BuildInfo.instance.branch == "master")
                return false;
            EditorGUIUtils.ShowInfoWindow("Should be on master branch in git! Please don't break this rule.");
            return true;
        }
        void OnVipePlayersGUI()
        {
            if (GUILayout.Button("Vipe all players") && !MessageIfNotOnMaster())
                EditorGUIUtils.DoIfConfirmed("Vipe all players?", VipeAllPlayers);
        }
        void VipeAllPlayers()
        {
            settings.metaDatas.Clear();
            settings.SetChanged();
            MigrationCodegen.RemoveAll();
            AssetDatabase.Refresh();
        }

        bool inited;
        string currMetaDataErrors;
        bool allDiffsSaved;
        void InitIfNeeded()
        {
            if (inited) return;
            inited = true;
            var sb = new StringBuilder();
            var sbInner = new StringBuilder();
            var currMetaValid = WorldMetaDataUtils.IsCurrentMetaValid(sbInner);
            if (!currMetaValid)
                sb.AppendLine($"Current metadata is invalid, erros = \n{sbInner.ToString()}");
            if (settings.metaDatas.Count == 0)
            {
                allDiffsSaved = false;
                sb.AppendLine($"Missing game versions, data migration will be impossible. Always save version before release");
            }
            else
            {
                settings.metaDatas.SortBy(m => m.version);
                var curr = WorldMetaData.CreateCurrent();
                var prev = WorldMetaData.DecodeFromString(settings.metaDatas.Last().metaData);
                sbInner.Clear();
                var diffsCount = MigrationManager.LogDiffs(prev, curr, sbInner);
                allDiffsSaved = diffsCount == 0;
                if (!allDiffsSaved)
                    sb.AppendLine($"Unsaved metadata diffs ({diffsCount}):\n{sbInner.ToString()}");
            }
            migrationBaseClasses.Clear();
            ForEachMigration((oldVersion, newVersion) => {
                ExampleScript script = null;
                Type classType = null;
                var baseClassName = Migration.GetMigrationBaseClassName(oldVersion, newVersion);
                var baseClassType = Migration.GetMigrationBaseClassType(baseClassName);
                if (baseClassType == null)
                    sb.AppendLine($"migration class {baseClassName} is not generated");
                else
                {
                    script = new ExampleScript(baseClassName);
                    classType = Migration.GetMigrationClass(baseClassType, sb);
                }
                migrationBaseClasses.Add(oldVersion, (script, baseClassType, classType));
            });
            currMetaDataErrors = sb.ToString();
        }
        StringBuilder sb = new StringBuilder();
        void ShowErrors()
        {
            var errors = DoReleaseChecks();
            if (!string.IsNullOrEmpty(errors))
                EditorGUIUtils.Error(errors);
        }
        public override string DoReleaseChecks()
        {
            InitIfNeeded();
            sb.Clear();
            if (!string.IsNullOrEmpty(currMetaDataErrors))
                sb.AppendLine(currMetaDataErrors);
            MigrationsValid(sb);
            TestAllSaves(sb);
            return sb.ToString();
        }
        Dictionary<int, (ExampleScript script, Type baseType, Type type)> migrationBaseClasses = new Dictionary<int, (ExampleScript script, Type baseType, Type type)>();
        void ForEachMigration(Action<int, int> action)
        {
            for (int i = 0; i < settings.metaDatas.Count - 1; i++)
            {
                var oldVersion = settings.metaDatas[i].version;
                var newVersion = oldVersion + 1;
                action(oldVersion, newVersion);
            }
        }
        int showDetailsInMigrationToVersion = -1;
        string shownMigrationMetaChanges = "";
        void ShowExistingMigrations(ref bool changed)
        {
            var currChanged = changed;
            EditorGUIUtils.InHorizontal(() => {
                EditorGUIUtils.ShowValid(allDiffsSaved);
                GUILayout.Label(allDiffsSaved ? $"all diffs saved" : "unsaved diffs exist");
            });
            if (settings.metaDatas.Count < 2)
                return;
            EditorGUIUtils.LabelAtCenter($"existing migrations:", FontStyle.Bold);
            ForEachMigration((oldVersion, newVersion) =>
            {
                GUILayout.BeginHorizontal();
                var currDetails = showDetailsInMigrationToVersion == newVersion;
                if (EditorGUIUtils.ShowOpenClose(ref currDetails))
                {
                    if (currDetails)
                    {
                        showDetailsInMigrationToVersion = newVersion;
                        sb.Clear();
                        var count = MigrationManager.LogDiffs(settings.GetMetaData(oldVersion), settings.GetMetaData(newVersion), sb);
                        sb.Insert(0, $"metadata changes count = {count}\n");
                        shownMigrationMetaChanges = sb.ToString();
                    }
                    else
                        showDetailsInMigrationToVersion = -1;
                }
                var types = migrationBaseClasses[oldVersion];
                EditorGUIUtils.ShowValid(MigrationValid(oldVersion, newVersion));
                EditorGUIUtils.RichMultilineLabel($"<b>{oldVersion}</b>-><b>{newVersion}</b>", 100);
                var currSettings = settings.Get(newVersion);
                EditorGUIUtils.RichMultilineLabel($"<b>v{currSettings.shownVersion}</b>");
                var whatsNewText = string.IsNullOrEmpty(currSettings.whatsNew) ? "<color=#00000055>No 'what's new'</color>" : currSettings.whatsNew.Split('\n')[0].WithMaxLength(10);
                EditorGUIUtils.RichMultilineLabel(whatsNewText, 120);
                if (types.script != null)
                    types.script.ShowOnGUI("");
                else
                {
                    if (GUILayout.Button($"Generate migration {oldVersion}->{newVersion}"))
                        GenerateMigrationClassIfPossible(oldVersion, newVersion);
                }
                GUILayout.EndHorizontal();

                if (!currDetails)
                    return;

                EditorGUIUtils.TextField("shown version", ref currSettings.shownVersion, ref currChanged);
                EditorGUIUtils.TextArea("what's new", ref currSettings.whatsNew, ref currChanged);
                EditorGUIUtils.Toggle("no 'what's new' is ok", ref currSettings.noWhatsNewOk, ref currChanged);
                EditorGUIUtils.TextArea("dont forget", ref currSettings.dontForget, ref currChanged);
                EditorGUIUtils.RichMultilineLabel(shownMigrationMetaChanges);
            });
            changed = currChanged;
        }
        static MigrationSettings.VersionMetaData currVersionMetaData 
            => settings.metaDatas.Find(m => m.shownVersion == Application.version);
        bool MigrationValid(int oldVersion, int newVersion, StringBuilder sb = null)
        {
            var valid = true;
            var types = migrationBaseClasses[oldVersion];
            if (types.type == null)
            {
                sb?.AppendLine($"migration {oldVersion}->{newVersion} class not found");
                valid = false;
            }
            if (types.baseType == null)
            {
                sb?.AppendLine($"migration {oldVersion}->{newVersion} codegeneration not completed");
                valid = false;
            }
            var versionConfig = settings.Get(newVersion);
            if (versionConfig == null)
            {
                sb?.AppendLine($"version {newVersion} metadata not saved");
                return false;
            }
            if (string.IsNullOrEmpty(versionConfig.shownVersion))
            {
                sb?.AppendLine($"shownVersion not set for {newVersion}");
                valid = false;
            }
            if (string.IsNullOrEmpty(versionConfig.whatsNew) && !versionConfig.noWhatsNewOk)
            {
                sb?.AppendLine($"whats new not set for data version {newVersion} (shown version {versionConfig.shownVersion})");
                valid = false;
            }
            if (!string.IsNullOrEmpty(versionConfig.dontForget))
            {
                sb?.AppendLine($"migration dont forget: {versionConfig.dontForget}");
                valid = false;
            }
            return valid;
        }
        bool MigrationsValid(StringBuilder sb = null)
        {
            var valid = true;
            ForEachMigration((oldVersion, newVersion) => valid &= MigrationValid(oldVersion, newVersion, sb));
            // Check curr version equals shown curr version.
            var currVersionSettings = settings.currVersion;
            if (currVersionSettings==null)
            {
                sb?.AppendLine($"curr data version({DataVersion.versionInd}) settings does not saved");
                return false;
            }
            if (currVersionSettings.shownVersion != Application.version)
            {
                sb?.AppendLine($"latest version 'what's new' window shows version {currVersionSettings.shownVersion}, but game version is {Application.version}");
                valid = false;
            }
            if (settings.metaDatas.Count > 1 && !settings.noNewVersionWindowOk && !WindowEditorUtils.WindowValid(settings.newVersionWindow, sb))
                valid = false;
            return valid;
        }
        void GenerateMigrationClassIfPossible(int oldVersion, int newVersion)
        {
            var oldMeta = settings.GetMetaData(oldVersion, true);
            var newMeta = settings.GetMetaData(newVersion, true);
            var migrationClassNeeded = oldMeta != null && newMeta != null;
            MigrationCodegen.Generate(oldMeta, newMeta, migrationClassNeeded, aotClassNeeded: true);
        }
        #endregion

        #region What's new window
        void UpdateWindow(ref bool changed)
        {
            EditorGUIUtils.Toggle("no whats new is ok?", ref settings.noNewVersionWindowOk, ref changed);
            if (settings.noNewVersionWindowOk) return;
            WindowEditorUtils.ShowWindow<NewVersionWindow, WhatsNewPrefab>(settings.newVersionWindow, ref changed);
        }
        #endregion

        #region Debug explore save
        private void DebugMigration(ref bool changed)
        {
            DebugShowMigrationWindows(ref changed);
            DebugExploreMigrations(ref changed);
        }

        bool debugMigrationsEnabled;
        string saveStr = "";
        void DebugExploreMigrations(ref bool changed)
        {
            if (EditorGUIUtils.ShowOpenClose(ref debugMigrationsEnabled, "debug migrations from email") && debugMigrationsEnabled)
                saveStr = File.ReadAllBytes(Serialization.path).EncodeToString();
            if (!debugMigrationsEnabled)
                return;

            EditorGUIUtils.TextField("copy save from email here", ref saveStr, ref changed);
            if (GUILayout.Button("use this save"))
                Serialization.ApplySaveFromStr(saveStr);

            if (GUILayout.Button("Test save emailing"))
                Serialization.SendSaveToDeveloper("Test save emailing");
        }

        void DebugShowMigrationWindows(ref bool changed)
        {
            var enabled = settings.debugShowMigrationFrom != -1;
            GUILayout.BeginHorizontal();
            if (EditorGUIUtils.Toggle("debug show 'what's new'", ref enabled, ref changed))
            {
                if (enabled)
                    settings.debugShowMigrationFrom = DataVersion.versionInd - 1;
                else
                    settings.debugShowMigrationFrom = -1;
            }
            if (settings.debugShowMigrationFrom != -1)
                EditorGUIUtils.IntField("from version", ref settings.debugShowMigrationFrom, ref changed);
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Old saves for testing migration
        bool testSavesOpened;
        void OnTestSavesGUI(ref bool changed)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.ShowOpenClose(ref testSavesOpened);
            EditorGUIUtils.ShowValid(TestSavesValid());
            GUILayout.Label("test saves");
            GUILayout.EndHorizontal();

            if (!testSavesOpened)
                return;

            foreach (var version in settings.metaDatas)
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(TestSaveValid(version));
                GUILayout.Label(version.version.ToString());
                EditorGUIUtils.PushEnabling(string.IsNullOrEmpty(version.testSave));
                currTestSaveCompleted &= !EditorGUIUtils.Toggle("allow no save", ref version.noTestSaveOk, ref changed, width:100, labelWidth:80);
                EditorGUIUtils.PopEnabling();
                currTestSaveCompleted &= !EditorGUIUtils.TextField("test save data", ref version.testSave, ref changed);
                if (GUILayout.Button("Test"))
                {
                    sb.Clear();
                    TestOneSave(version, sb);
                    var res = sb.Length == 0 ? $"success! version {version.version} updated to recent version":sb.ToString();
                    Debug.Log(res);
                }
                if (GUILayout.Button("Put to current"))
                    File.WriteAllBytes(Serialization.path, SerializationUtils.DecodeFromString(version.testSave));
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Test all saves"))
            {
                var success = TestAllSaves(sb);
                Debug.Log(success ? "Success! All tests ok" : "Failure! Some test saves are failed to upgrade");
            }
            if (GUILayout.Button("Put current save to test saves"))
                SaveCurrentVersionSave(ref changed);
        }
        bool TestSavesValid(StringBuilder sb = null)
        {
            var valid = true;
            foreach (var version in settings.metaDatas)
                if (!TestSaveValid(version, sb))
                    valid = false;
            return valid;
        }
        bool TestSaveValid(MigrationSettings.VersionMetaData version, StringBuilder sb = null)
        {
            if (string.IsNullOrEmpty(version.testSave) && !version.noTestSaveOk)
            {
                sb?.AppendLine($"Data version {version.version} has no test save, cant check migration validity");
                return false;
            }
            return true;
        }
        string currTestSaveErrors;
        bool currTestSaveCompleted;
        StringBuilder testSaveSB = new StringBuilder();
        bool TestAllSaves(StringBuilder sb)
        {
            var valid = true;
            if (!currTestSaveCompleted)
            {
                testSaveSB.Clear();
                foreach (var version in settings.metaDatas)
                    valid &= TestOneSave(version, testSaveSB);
                currTestSaveErrors = testSaveSB.ToString();
                currTestSaveCompleted = true;
            }
            sb.Append(testSaveSB);
            return valid;
        }
        public static string testSavePath => Application.persistentDataPath + "/MigrationTestSaveWorld.dat";
        bool TestOneSave(MigrationSettings.VersionMetaData version, StringBuilder sb)
        {
            if (version.noTestSaveOk)
                return true;
            if (!TestSaveValid(version, sb))
                return false;
            File.WriteAllBytes(testSavePath, SerializationUtils.DecodeFromString(version.testSave));
            var save = UnversionedWorldData.LoadVersionIndependant(testSavePath);
            if (save == null)
            {
                sb?.AppendLine($"save for version {version.version} cant be loaded");
                return false;
            }
            if (!Migration.EnsureUpToDate(ref save, null))
            {
                sb?.AppendLine($"save migration {version.version} failed");
                return false;
            }
            return true;
        }
        void SaveCurrentVersionSave(ref bool changed)
        {
            changed = true;
            currTestSaveCompleted = false;
            var version = settings.currVersion;
            if (version == null)
                version = SaveCurrentVersionMeta();
            version.testSave = File.ReadAllBytes(Serialization.path).EncodeToString();
            version.noTestSaveOk = false;
        }
        #endregion
#endif
    }
}