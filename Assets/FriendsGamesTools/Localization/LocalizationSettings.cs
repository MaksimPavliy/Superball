using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{


    public class LocalizationSettings : SettingsScriptable<LocalizationSettings> {
        public Language defaultPlayerLanguage = Language.EN;
        public Language _playerLanguage = Language.EN;
        public List<Language> activeLanguages = new List<Language> { Language.EN };
        public List<LocalizationKeyData> keys = new List<LocalizationKeyData>();
        public ExportFormat exportFormat = ExportFormat.PlainText;
        public enum ExportFormat { CSV, PlainText }
        public List<TextReplacement> replaceOnImport
            = new List<TextReplacement> { new TextReplacement { replaceWhat = "？", replaceToWhat = " ? " } };
        [Serializable]
        public struct TextReplacement { public string replaceWhat, replaceToWhat; }
        public List<LocalizationFont> fontSettings = new List<LocalizationFont> { new LocalizationFont { language = Language.EN } };

#if LOCALIZATION
        public bool chineseActive => activeLanguages.Any(l => l.IsChinese());
        public Language playerLanguage
        {
            get => _playerLanguage;
            set
            {
                _playerLanguage = value;
                if (Application.isPlaying)
                {
                    WithLocalizationCallbackInRuntime.instances.ForEach(view => view.SetLocalizationInRuntime(_playerLanguage, this));
                    LocalizationManager.SaveLanguage();
                }
            }
        }
        public LocalizationKeyData Get(string key)
        {
            // TODO: make fast for runtime.
            return keys.Find(k => k.localizationKey == key);
        }
#if UNITY_EDITOR
        public void CreateLocalization(string localizationKey, string shownText)
            => CreateLocalization(localizationKey, playerLanguage, shownText);
        public void CreateLocalization(string localizationKey, Language lang, string shownText)
        {
            var keyData = Get(localizationKey);
            if (keyData == null) {
                keyData = new LocalizationKeyData();
                keys.Add(keyData);
            }
            keyData.localizationKey = localizationKey;
            //keyData.paramsCount = 0;
            var translation = keyData.GetTranslation(lang);
            if (translation==null)
            {
                translation = new TranslationData();
                keyData.translations.Add(translation);
            }
            translation.language = lang;
            translation.localizedText = PostProcessOnImport(shownText);
            if (!activeLanguages.Contains(lang))
                activeLanguages.Add(lang);
            this.SetChanged();
        }
        private string PostProcessOnImport(string stringToImport)
        {
            replaceOnImport.ForEach(r => stringToImport = stringToImport.Replace(r.replaceWhat, r.replaceToWhat));
            return stringToImport;
        }
#endif

        public void UpdateShownText(LocalizableView localizable)
        {
            var text = localizable.text;
            if (text == null) return;
            text.Show(localizable, this);
        }
        public LocalizationFont GetFontSettings(Language language)
        {
            var font = fontSettings.Find(x => x.language == language);
            return font;
        }
#endif
    }
    [Serializable]
    public class LocalizationKeyData
    {
        public string localizationKey;
        public int paramsCount;
        public string context; // Optional note for translator - describing context of this translation.
        public enum Status { Normal, Obsolete }
        public Status status;
        public List<TranslationData> translations = new List<TranslationData>();

#if LOCALIZATION
        public string Get(Language language) => GetTranslation(language)?.localizedText;
        public TranslationData GetTranslation(Language language)
        {
            // TODO: make fast for runtime.
            return translations.Find(tr => tr.language == language);
        }
       

#endif
    }
    [Serializable]
    public class TranslationData
    {
        public Language language;
        public string localizedText;
    }
    [Serializable]
    public class LocalizationFont
    {
        public Language language;
        public TMP_FontAsset fontAsset;
    }
}