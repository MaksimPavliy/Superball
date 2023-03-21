using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class CrossMarketingModule : RootModule
    {
        public const string define = "CROSS_MARKETING";
        public override string Define => define;
        public override HowToModule HowTo() => new CrossMarketingModule_HowTo();

#if CROSS_MARKETING
        static CrossMarketingConfig dataConfig => CrossMarketingConfig.instance;
        static CrossMarketingShowSettings showConfig => SettingsInEditor<CrossMarketingShowSettings>.instance;
        void InitViewConfig() {
            var changed = false;
            foreach (var game in dataConfig.data) {
                var view = showConfig.data.Find(v => v.id == game.id);
                if (view != null) continue;
                showConfig.data.Add(new CrossMarketingShowFlag { id = game.id, show = true });
                changed = true;
            }
            if (changed)
                showConfig.SetChanged();
        }

        protected override void OnCompiledEnable() {
            base.OnCompiledEnable();
            InitViewConfig();
        }

        static bool FGTEditable => FGTLocalSettings.instance.FGTEditable;
        protected override void OnCompiledGUI() {
            base.OnCompiledGUI();

            var changed = false;

            FGTSettingsUtils.AppleAppIdInput("Current game Apple ID", 140);

            GUILayout.Space(10);
            if (GUILayout.Button($"import"))
                Import(ref changed);

            GUILayout.Space(10);
            EditorGUIUtils.LabelAtCenter("GAMES LIST", FontStyle.Bold);
            GUILayout.Space(10);

            ShowApps(ref changed);

            if (FGTEditable)
                ShowAddGameButton(ref changed);

            if (changed) {
                EditorUtils.SetDirty(dataConfig);
                showConfig.SetChanged();
            }
        }

        private void Import(ref bool changed) {
            ResetData();
            var text = StringUtils.PasteFromClipboard().ToLf();
            bool success = CrossMarketingFileProcessor.ImportCSV(text, dataConfig.data, showConfig.data);
            changed = success;
            if (success)
                Debug.Log("Import successfully");
            else
                Debug.LogError($"Import failed");
        }

        private void ResetData() {
            dataConfig.data.Clear();
            showConfig.data.Clear();
        }

        private void ShowAddGameButton(ref bool changed) {
            if (GUILayout.Button("Add game")) {
                var nextId = GetNextId();
                var newData = new CrossMarketingAppData() { id = nextId };
                var show = new CrossMarketingShowFlag() { id = nextId, show = true };

                dataConfig.data.Add(newData);
                showConfig.data.Add(show);

                changed = true;
            }
        }

        int GetNextId() {
            var id = 1;
            do {
                var exists = dataConfig.data.Any(c => c.id == id) || showConfig.data.Any(c => c.id == id);
                if (!exists) return id;
                id++;
            } while (true);
        }
        private void ShowName(int index, ref string appName, ref bool changed) {
            GUILayout.BeginHorizontal();

            EditorGUIUtils.RichLabel($"{index}.", fontStyle: FontStyle.Bold, width: 20);
            EditorGUIUtils.TextField("Name:", ref appName, ref changed, 365, 20, 50);
            EditorGUIUtils.ShowValid(!appName.IsNullOrEmpty());

            GUILayout.EndHorizontal();
        }
        private void ShowAppData(int index, CrossMarketingAppData app, ref bool changed) {

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUIUtils.PushEnabling(FGTEditable);
            GUILayout.BeginVertical();

            ShowName(index, ref app.appName, ref changed);

            GUILayout.Space(10);

            DrawAppDetails(app, ref changed);

            GUILayout.EndVertical();
            EditorGUIUtils.PopEnabling();
            DrawShownToggle(index, app, ref changed);
            if (FGTEditable)
                DrawRemoveButton(app, ref changed);

            GUILayout.EndHorizontal();
        }

        private void DrawAppDetails(CrossMarketingAppData app, ref bool changed) {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            DrawStoreSettingsGroup("Apple ID ", ref app.showIOS, ref app.appleID, ref changed);
            DrawStoreSettingsGroup("Google ID", ref app.showAndroid, ref app.androidPackageId, ref changed);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("", GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
            EditorGUIUtils.ObjectField("", ref app.icon, ref changed, 60);
            EditorGUIUtils.ShowValid(app.icon != null);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        private void DrawRemoveButton(CrossMarketingAppData app, ref bool changed) {
            GUILayout.BeginVertical();
            if (GUILayout.Button("X", EditorStyles.miniButtonLeft, GUILayout.Width(18f))) {
                RemoveAppData(app);
                changed = true;
            }
            GUILayout.EndVertical();
        }
        private void DrawShownToggle(int index, CrossMarketingAppData app, ref bool changed) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("  Show");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!GetCanBeShown(index, app, ref changed));
            EditorGUIUtils.Toggle("", ref showConfig.data[index].show, ref changed);
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        private bool GetCanBeShown(int index, CrossMarketingAppData app, ref bool changed) {
            if (!app.available && showConfig.data[index].show) {
                showConfig.data[index].show = false;
                changed = false;
            }
            return app.available;
        }
        private void DrawStoreSettingsGroup(string label, ref bool show, ref string id, ref bool changed) {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.Toggle("", ref show, ref changed, 30, 1);
            EditorGUIUtils.TextField(label, ref id, ref changed, 250, 20, 80);
            EditorGUIUtils.ShowValid(CrossMarketingAppData.IdIsValid(id));
            GUILayout.EndHorizontal();
        }
        private void ShowApps(ref bool changed) {
            GuiLine();
            for (int i = 0; i < dataConfig.data.Count; i++) {
                var app = dataConfig.data[i];
                ShowAppData(i, app, ref changed);
                GuiLine();
            }
        }

        private void RemoveAppData(CrossMarketingAppData app) {
            foreach (var item in showConfig.data.ToList()) {
                if (app.id == item.id) {
                    showConfig.data.Remove(item);
                }
            }
            dataConfig.data.Remove(app);
        }
        private void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public override string DoReleaseChecks() {
            if (dataConfig == null || showConfig == null || dataConfig.data == null || showConfig.data == null)
                return $"CrossMarking module is not inited.";
            return null;
        }
#endif
    }
}