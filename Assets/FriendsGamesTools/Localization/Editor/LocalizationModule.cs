using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FriendsGamesTools.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class LocalizationModule : RootModule
    {
        public const string define = "LOCALIZATION";
        public override string Define => define;
        public override HowToModule HowTo() => new LocalizationModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(UIModule.define);
        public override List<string> dependFromPackages => base.dependFromPackages.Adding("com.unity.textmeshpro@2.1.3");

#if LOCALIZATION
        static LocalizationSettings settings => SettingsInEditor<LocalizationSettings>.instance;
        protected override void OnCompiledEnable()
        {
            SettingsInEditor<LocalizationSettings>.EnsureExists();
            shownTranslations = settings.activeLanguages.Clone();
            shownFonts = settings.fontSettings.Clone();
            langToExport = settings.activeLanguages.Find(lang => lang != Language.EN);
            InitFontExample();
        }
        List<Language> shownTranslations;
        List<LocalizationFont> shownFonts;
        Language langToExport;
        protected override void OnCompiledGUI()
        {
            var changed = false;
            GUILayout.BeginHorizontal();
            var lang = settings.playerLanguage;
            if (EditorGUIUtils.Popup("player language", ref lang, ref changed))
            {
                settings.playerLanguage = lang;
                if (!Application.isPlaying)
                    SetLanguageInOpenedScene(settings.playerLanguage);
                LocalizationManager.SaveLanguage();
            }
            GUILayout.EndHorizontal();

            EditorGUIUtils.Popup("default player language", ref settings.defaultPlayerLanguage, ref changed);

            OnExportGUI(ref changed);
            
            ShowLanguages("Active languages", settings.activeLanguages, settings.fontSettings, ref changed);
            GUILayout.Space(10);

            ShowUpdateKeys(ref changed);

            if (settings.chineseActive)
                ShowUpdateChineseAtlases(ref changed);

            ShowTranslations(ref changed);

            if (changed)
                settings.SetChanged();
        }

        const string fontWithChineseNameOriginal = "RobotoWithChineseFGT";
        string fontWithChineseName => FontAssetCreator.GetGeneratedFontName(fontWithChineseNameOriginal);
        ExampleFontAsset fontWithChinese;
        void InitFontExample() => fontWithChinese = new ExampleFontAsset(fontWithChineseName, "font not yet generated");
        private void ShowUpdateChineseAtlases(ref bool changed)
        {
            fontWithChinese.ShowOnGUI("use this font, it has Chinese");
            if (GUILayout.Button("Update Chinese font atlases")) {
                UpdateChineseFontAtlases();
                changed = true;
            }
        }
        void UpdateChineseFontAtlases()
        {
            var font = UpdateAtlases(Language.CHs, Language.CHt);
            var aggregatedFont = FontAssetCreator.CreateAggregate(fontWithChineseNameOriginal, UIModule.defaultFont, font);
            Selection.activeObject = aggregatedFont;
            EditorGUIUtility.PingObject(aggregatedFont);
            InitFontExample();
        }

        private TMP_FontAsset UpdateAtlases(params Language[] langs)
        {
            // Symbols.
            var symbols = new List<char>();
            settings.keys.ForEach(key => langs.ForEach(lang => key.Get(lang)?.ForEach(ch => symbols.AddIfNotExists(ch))));

            // Source font.
            var sourceFontName = "HanyiSentyChalk";
            var font = new ExampleFontTTF(sourceFontName).asset;

            return FontAssetCreator.Create(font, sourceFontName, symbols.PrintCollection("", ""));
        }

        private void ShowUpdateKeys(ref bool changed)
        {
            if (GUILayout.Button("Update localization keys"))
                UpdateLocalizationKeys();
        }

        private void UpdateLocalizationKeys()
        {
            // Remove duplicate keys.
            for (int i = settings.keys.Count - 1; i >= 0; i--)
            {
                var key = settings.keys[i].localizationKey;
                if (settings.keys.FindIndex(k=>k.localizationKey==key)!=i)
                {
                    Debug.Log($"removing duplicate key {key}");
                    settings.keys.RemoveAt(i);
                }
            }

            // Find keys and params count in project.
            var keysUsedInProject = new List<(string localizationKey, int paramsCount)>();

            // Get keys from prefabs.
            Action<GameObject> iterateGO = (GameObject go) => {
                var text = go.GetComponent<LocalizableView>();
                if (text != null && !text.localizationKey.IsNullOrEmpty())
                    OnKeyFound(text.localizationKey);
                var monobehs = go.GetComponents<Component>();
                monobehs.ForEach(c =>
                {
                    if (c != null)
                        GetKeysFrom(c);
                });
            };
            // Get keys from scriptable objects.
            void GetKeysFrom(object instance)
            {
                var t = instance.GetType();
                var members = t.GetFields().ToList();
                members = members.Filter(member => member.MemberType == MemberTypes.Field && member.GetFieldPropertyType() == typeof(LocalizedText));
                if (members.Count == 0) return;
                members.ForEach(member=> {
                    var localizedText = ReflectionUtils.GetField<LocalizedText>(instance, member.Name);
                    var key = ReflectionUtils.GetField<string>(localizedText, "localizationKey");
                    OnKeyFound(key);
                });
            }
            void OnKeyFound(string key)
            {
                if (keysUsedInProject.Any(k => k.localizationKey == key)) return; // Already exists.
                keysUsedInProject.Add((key, 0));
            }
            // Get keys from code.
            AssetsIterator.IterateAllAssetsRecursively(path => {
                if (path.EndsWith(".prefab"))
                {
                    var prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    prefabRoot.transform.IterateChildren(tr => iterateGO(tr.gameObject));
                }
                else if (path.EndsWith(".cs"))
                    IterateKeysFromCode(path, keysUsedInProject);
                else if (path.EndsWith(".asset"))
                {
                    var scriptable = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    if (scriptable != null)
                        GetKeysFrom(scriptable);
                }
            });
            AssetsIterator.IterateAllSceneGameObjectsInProject((scenePath, hierarcyPath, go) => iterateGO(go));

            // Find old keys to remove.
            settings.keys.ForEach(key => {
                var usedKeyInd = keysUsedInProject.FindIndex(k => k.localizationKey == key.localizationKey);
                var usedInProject = usedKeyInd != -1;
                key.status = usedInProject ? LocalizationKeyData.Status.Normal : LocalizationKeyData.Status.Obsolete;
                if (usedInProject)
                {
                    key.paramsCount = keysUsedInProject[usedKeyInd].paramsCount; // Also update params count.
                    keysUsedInProject.RemoveAt(usedKeyInd);
                }
            });
            // Add new keys.
            keysUsedInProject.ForEach(k => {
                settings.keys.Add( new LocalizationKeyData {
                        localizationKey = k.localizationKey,
                    paramsCount = k.paramsCount,
                    status = LocalizationKeyData.Status.Normal
                });
            });


            settings.SetChanged();
        }
        private static int IndexOf(string substr, int startInd, string str)
        {
            if (startInd == int.MaxValue) return int.MaxValue;
            var ind = str.IndexOf(substr, startInd);
            if (ind == -1) ind = int.MaxValue;
            return ind;
        }
        private void IterateKeysFromCode(string csPath, List<(string, int)> foundedKeys)
        {
            if (csPath.Contains("/Editor/")) return;
            var code = File.ReadAllText(csPath);
            var localizationGet = "Localization.Get";
            int i = 0;
            while (i != int.MaxValue)
            {
                Read(localizationGet);
                Read("(");
                if (IndexOf("\"", i, code) < IndexOf(")", i, code))
                {
                    Read("\"");
                    var startString = i;
                    Read("\"");
                    var endString = i;
                    if (startString != int.MaxValue && endString != int.MaxValue)
                    {
                        var localizationKey = code.Substring(startString, endString - startString - 1);
                        //Debug.Log($"{localizationKey} from {csPath}");
                        var paramsCount = 0;
                        while (IndexOf(",", i, code) < IndexOf(")", i, code))
                        {
                            Read(",");
                            paramsCount++;
                        }
                        if (!foundedKeys.Any(k => k.Item1 == localizationKey))
                            foundedKeys.Add((localizationKey, paramsCount));
                    }
                }
                Read(")");
            }
            void Read(string str) {
                if (i == -1 || i >= code.Length) return;
                i = IndexOf(str, i, code);
                if (i == int.MaxValue) return;
                i += str.Length;
                if (i >= code.Length) i = int.MaxValue;
            }
        }

        bool showContext = true;
        private void ShowTranslations(ref bool changed)
        {
            EditorGUIUtils.Toggle("show context", ref showContext, ref changed, 150, 125);
            ShowLanguages("Show translations", shownTranslations, shownFonts, ref changed);
            for (int i = 0; i < settings.keys.Count; i++)
            {
                var key = settings.keys[i];
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                EditorGUIUtils.SetBackgroundColor(GetStatusColor(key.status));
                EditorGUIUtils.TextField("localization key", ref key.localizationKey, ref changed);
                EditorGUIUtils.RestoreBackgroundColor();
                EditorGUIUtils.IntField("params count", ref key.paramsCount, ref changed, 130, labelWidth: 100);
                if (GUILayout.Button("remove", GUILayout.Width(60)))
                {
                    settings.keys.RemoveAt(i);
                    i--;
                    continue;
                }
                GUILayout.EndHorizontal();
                if (showContext)
                    EditorGUIUtils.TextField("context", ref key.context, ref changed);
                foreach (Language lang in Enum.GetValues(typeof(Language)))
                {
                    if (!shownTranslations.Contains(lang)) continue;
                    GUILayout.BeginHorizontal();
                    var tr = key.GetTranslation(lang);
                    GUILayout.Label(lang.ToString(), GUILayout.Width(30));
                    var localizedText = tr?.localizedText;
                    EditorGUIUtils.SetBackgroundColor(TranslationDataValid(tr, key.localizationKey, key.paramsCount) ? Color.white : Color.red);
                    if (EditorGUIUtils.TextField(string.Empty, ref localizedText, ref changed))
                    {
                        if (tr == null)
                        {
                            tr = new TranslationData { language = lang };
                            key.translations.Add(tr);
                        }
                        tr.localizedText = localizedText;
                    }
                    EditorGUIUtils.RestoreBackgroundColor();
                    GUILayout.EndHorizontal();
                }
            }
        }

        private Color GetStatusColor(LocalizationKeyData.Status status)
        {
            switch (status)
            {
                case LocalizationKeyData.Status.Normal: return Color.white;
                default:
                case LocalizationKeyData.Status.Obsolete: return Color.red;
            }
        }

        private void ShowLanguages(string title, List<Language> languages, List<LocalizationFont> fonts, ref bool changed)
        {
            var changed1 = changed;
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            Utils.ForEach<Language>(lang =>
            {
                var active = languages.Contains(lang);
                if (EditorGUIUtils.Toggle(lang.ToString(), ref active, ref changed1, 50, 25))
                {
                    if (active)
                    {
                        languages.Add(lang);
                        fonts.Add(new LocalizationFont { language = lang, fontAsset = null });
                    }
                    else
                    {
                        languages.Remove(lang);
                        var font = fonts.Find(x => x.language == lang);
                        if (font!=null)
                        {
                            fonts.Remove(font);
                        }
                    }
                }
                GUILayout.Space(20);
            });
            GUILayout.EndHorizontal();
            changed = changed1;
        }

        public static void SetLanguageInOpenedScene(Language lang)
        {
            settings.playerLanguage = lang;
            if (PrefabUtils.openedPrefab != null)
                PrefabUtils.openedPrefab.transform.IterateChildren(tr => SetLanguage(tr.gameObject));
            else
                AssetsIterator.IterateOpenedScene(SetLanguage);
        }
        private static void SetLanguage(GameObject go)
        {
            var localizable = go.GetComponent<LocalizableView>();
            if (localizable == null) return;
            settings.UpdateShownText(localizable);
            go.transform.SetChanged();
        }

        StringBuilder sb = new StringBuilder();
        public override string DoReleaseChecks()
        {
            sb.Clear();
            if (settings.activeLanguages.Count == 0)
                sb.AppendLine("any language should be active in localizations");
            settings.keys.ForEach(key => LocalizationKeyDataValid(key, sb));
            return sb.ToString();
        }
        bool LocalizationKeyDataValid(LocalizationKeyData localizationKeyData, StringBuilder sb = null)
        {
            var valid = true;
            if (localizationKeyData.localizationKey.IsNullOrEmpty())
            {
                sb?.AppendLine("localization key is empty");
                valid = false;
            }
            if (localizationKeyData.paramsCount < 0 || localizationKeyData.paramsCount > 100)
            {
                sb?.AppendLine($"localization key {localizationKeyData.localizationKey} " +
                    $"params count({localizationKeyData.paramsCount}) should be sane");
                valid = false;
            }
            settings.activeLanguages.ForEach(lang =>
            {
                var translation = localizationKeyData.translations.Find(tr => tr.language == lang);
                if (translation == null)
                {
                    sb?.AppendLine($"{localizationKeyData.localizationKey} is not localized to {lang}");
                    valid = false;
                }
                else
                    TranslationDataValid(translation, localizationKeyData.localizationKey, localizationKeyData.paramsCount, sb);
            });
            return valid;
        }
        bool TranslationDataValid(TranslationData translation, string localizationKey, int paramsCount, StringBuilder sb = null)
        {
            if (translation == null) return false;
            if (translation.localizedText.IsNullOrEmpty())
            {
                sb?.AppendLine($"{translation.language} translation for {localizationKey} should not be empty");
                return false;
            }
            if (!LocalizedText.GivenAndRequestedParamsCountMatch(translation.localizedText, paramsCount))
            {
                sb?.AppendLine($"{translation.language} translation '{translation.localizedText}' " +
                    $"for {localizationKey} should have {paramsCount} parameters");
                return false;
            }
            return true;
        }

        #region Export/import

        void OnExportGUI(ref bool changed)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.Popup("export", ref langToExport, ref changed, 100);
            if (GUILayout.Button($"export {langToExport}"))
                Export();
            if (GUILayout.Button($"import"))
            {
                Import();
                changed = true;
            }
            if (GUILayout.Button($"export All"))
                ExportAll();
            if (GUILayout.Button($"import All"))
                ImportAll();
            EditorGUIUtils.Popup("", ref settings.exportFormat, ref changed, 100);
            GUILayout.EndHorizontal();
        }

        private void Export()
        {
            string text, fileName;
            switch (settings.exportFormat)
            {
                default:
                case LocalizationSettings.ExportFormat.PlainText: (text, fileName) = ExportPlain(); break;
                case LocalizationSettings.ExportFormat.CSV: (text, fileName) = ExportChinaCSV(); break;
            }
            StringUtils.CopyToClipboard(text);
            File.WriteAllText(fileName, text, Encoding.UTF8);
            Debug.Log($"{langToExport} localization copied to clipboard and saved to {fileName}");
        }

        private void ExportAll()
        {
            string text, fileName;
            switch (settings.exportFormat)
            {
                default:
                case LocalizationSettings.ExportFormat.PlainText: (text, fileName) = ExportPlain(); break;
                case LocalizationSettings.ExportFormat.CSV: (text, fileName) = ExportAllCSV(); break;
            }
            StringUtils.CopyToClipboard(text);
            File.WriteAllText(fileName, text, Encoding.UTF8);
            Debug.Log($"All localization copied to clipboard and saved to {fileName}");
        }
        private void Import()
        {
            var text = StringUtils.PasteFromClipboard().ToLf();
            var importFormat = settings.exportFormat;
            if (text.IndexOf("\n") != -1 && text.Substring(0, text.IndexOf("\n")).Count(",") > 2) // first line has >2 comma.
                importFormat = LocalizationSettings.ExportFormat.CSV;
            else
                importFormat = LocalizationSettings.ExportFormat.PlainText;
            bool success;
            switch (importFormat)
            {
                default:
                case LocalizationSettings.ExportFormat.PlainText: success = ImportPlain(text); break;
                case LocalizationSettings.ExportFormat.CSV: success = ImportChinaCSV(text); break;
            }
            if (success && settings.chineseActive)
                UpdateChineseFontAtlases();
            if (success)
                Debug.Log("Localization updated successfully");
            else
                Debug.LogError($"Import {importFormat} failed");
        }

        private void ImportAll()
        {
            var text = StringUtils.PasteFromClipboard().ToLf();
            var importFormat = settings.exportFormat;
            if (text.IndexOf("\n") != -1 && text.Substring(0, text.IndexOf("\n")).Count(",") > 2) // first line has >2 comma.
                importFormat = LocalizationSettings.ExportFormat.CSV;
            else
                importFormat = LocalizationSettings.ExportFormat.PlainText;
            bool success;
            switch (importFormat)
            {
                default:
                case LocalizationSettings.ExportFormat.PlainText: success = ImportPlain(text); break;
                case LocalizationSettings.ExportFormat.CSV: success = ImportAllCSV(text); break;
            }
            if (success && settings.chineseActive)
                UpdateChineseFontAtlases();
            if (success)
                Debug.Log("Localization updated successfully");
            else
                Debug.LogError($"Import {importFormat} failed");
        }

        #region China CSV
        List<string> columnNames = new List<string> {
            "n","Key","Eng","China Sim 简体中文","China Tr 繁体中文"
        };
        string columnsLine => columnNames.PrintCollection(",");
        (string text, string fileName) ExportChinaCSV()
        {
            // langToExport
            var sb = new StringBuilder();
            sb.AppendLine(columnsLine);

            var lineNumber = 1;
            settings.keys.ForEach(key => {
                var cells = new List<string> {
                    lineNumber.ToString(),
                    GetKeyWithContext(key.localizationKey, key.context),
                    key.Get(Language.EN),
                    key.Get(Language.CHs),
                    key.Get(Language.CHt)
                };
                var line = UnityCSVLine(cells);
                sb.AppendLine(line);
                lineNumber++;
            });

            var file = $"Chinese translation.csv";
            return (sb.ToString(), file);
        }

        (string text, string fileName) ExportAllCSV()
        {
            // langToExport
            string columns="";
            columns+="n,";
            columns+="Key,";

            for (int i = 0; i < settings.activeLanguages.Count; i++)
            {
                columns += settings.activeLanguages[i].ToString()+",";
            }

            var sb = new StringBuilder();
            sb.AppendLine(columns);

            var lineNumber = 1;
            settings.keys.ForEach(key => {


                var cells = new List<string> {
                    lineNumber.ToString(),
                    GetKeyWithContext(key.localizationKey, key.context),
                };

                foreach (var lang in settings.activeLanguages)
                {
                    cells.Add(key.Get(lang));
                }

                var line = UnityCSVLine(cells);
                sb.AppendLine(line);
                lineNumber++;
            });

            var file = $"All translation.csv";
            return (sb.ToString(), file);
        }
        bool ImportChinaCSV(string text)
        {
            var lines = text.Split('\n').ToList();
            if (lines.GetElementSafe(0) != columnsLine)
                return false;
            lines.RemoveAt(0);
            foreach (var line in lines)
            {
                if (line.IsNullOrEmpty()) continue;
                var cells = SplitCSVLine(line);
                if (cells.Count != columnNames.Count) return false;
                var key = ParseKeyWithContext(cells[1]);
                settings.CreateLocalization(key, Language.EN, cells[2]);
                settings.CreateLocalization(key, Language.CHs, cells[3]);
                settings.CreateLocalization(key, Language.CHt, cells[4]);
            }

            return true;
        }
        bool ImportAllCSV(string text)
        {
            var lines = text.Split('\n').ToList();
            lines.RemoveAt(0);
            foreach (var line in lines)
            {
                if (line.IsNullOrEmpty()) continue;
                var cells = SplitCSVLine(line);
                var key = ParseKeyWithContext(cells[1]);
                for (int i = 0; i < settings.activeLanguages.Count; i++)
                {
                    settings.CreateLocalization(key, settings.activeLanguages[i], cells[2+i]);
                }
            }

            return true;
        }
        // Screening "," characters.
        string UnityCSVLine(List<string> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i]?.Contains(",") ?? false)
                    cells[i] = $"\"{cells[i]}\"";
            }
            return cells.PrintCollection(",");
        }
        List<string> SplitCSVLine(string line)
        {
            var cells = line.Split(',').ToList();
            for (int i = cells.Count - 1; i >= 0; i--) {
                if (!cells[i].StartsWith("\"")) continue;
                while (i + 1 < cells.Count && !cells[i].EndsWith("\"")) {
                    cells[i] += cells[i + 1];
                    cells.RemoveAt(i + 1);
                }
            }
            return cells;
        }
        #endregion

        #region Plain text export/import
        const string stub = "%REPLACE WITH {0} TRANSLATION%";
        private (string text, string fileName) ExportPlain()
        {
            var langs = new List<Language>();
            langs.Add(Language.EN);
            langs.AddIfNotExists(langToExport);
            var sb = new StringBuilder();
            sb.AppendLine(langs.PrintCollection(" "));
            sb.AppendLine();
            settings.keys.ForEach(key => {
                sb.AppendLine(GetKeyWithContext(key.localizationKey, key.context));
                // Write texts.
                langs.ForEach(currLang =>
                {
                    var currText = key.Get(currLang);
                    if (currText.IsNullOrEmpty())
                        currText = string.Format(stub, langToExport);
                    sb.AppendLine(currText);
                });
                sb.AppendLine();
            });
            var text = sb.ToString();
            var file = $"{langToExport} translation.txt";
            return (text, file);
        }
        // Write localization key with context.
        private string GetKeyWithContext(string key, string context)
            => context.IsNullOrEmpty() ? key : $"{key} ({context})";
        private string ParseKeyWithContext(string keyWithContext)
        {
            void RemoveAfter(char ch) {
                if (keyWithContext.IndexOf(ch) != -1)
                    keyWithContext = keyWithContext.Remove(keyWithContext.IndexOf(ch));
            }
            RemoveAfter(' ');
            RemoveAfter('(');
            return keyWithContext;
        }

        private bool ImportPlain(string text)
        {
            var lines = text.Split('\n').ToList();
            var langsStr = lines[0].Split(' '); lines.RemoveAt(0);
            lines.RemoveAt(0);
            var langs = new List<Language>();
            langsStr.ForEach(langStr => {
                if (!Enum.TryParse<Language>(langStr, out var lang))
                    Debug.LogError($"lang {langStr} not found");
                else
                    langs.Add(lang);
            });
            if (langs.Count == 0)
                return false;

            var requiredLinesPerKey = 2 + langs.Count;
            while (lines.Count >= requiredLinesPerKey)
            {
                // Read localization key.
                var key = ParseKeyWithContext(lines[0]); lines.RemoveAt(0);

                langs.ForEach(lang =>
                {
                    var localizedText = lines[0]; lines.RemoveAt(0);
                    if (localizedText.Contains("%REPLACE WITH "))
                        localizedText = string.Empty;
                    settings.CreateLocalization(key, lang, localizedText);
                });
                lines.RemoveAt(0);
            }
            return true;
        }
        #endregion

        #endregion
#endif
    }
}