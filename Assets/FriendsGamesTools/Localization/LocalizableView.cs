using System;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{

    [RequireComponent(typeof(TextMeshProUGUI)), ExecuteAlways]
    public class LocalizableView : WithLocalizationCallbackInRuntime
    {
        public bool okWithoutLocalization;
        public string localizationKey;
        public event Action Localized;
        public bool localizeFont = true;

#if LOCALIZATION
        TextMeshProUGUI _label;
        public TextMeshProUGUI label => _label ?? (_label = GetComponent<TextMeshProUGUI>());
        [NonSerialized] LocalizedText _localizedText;
    
        public LocalizedText text
        {
            get
            {
                if (_localizedText == null && !okWithoutLocalization)
                {
                    if (!localizationKey.IsNullOrEmpty())
                        _localizedText = LocalizedText.Create(localizationKey);
                }
                return _localizedText;
            }
            set
            {
                if (!Application.isPlaying) return;
                if (value == null)
                {
                    localizationKey = string.Empty;
                    _localizedText = null;
                    return;
                }
                _localizedText = value;
                LocalizationSettings.instance.UpdateShownText(this);
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying) return;
            
            if (localizeFont) gameObject.AddComponent<LocalizableFontView>();

            LocalizationSettings.instance.UpdateShownText(this);
            Localized?.Invoke();
        }
        public string shownText => label.text;
        public override void SetLocalizationInRuntime(Language lang, LocalizationSettings settings)
        {
            text?.Show(this, settings);
            HackForChineseLabels(lang);
            Localized?.Invoke();
        }

        public void ShowKeyText(string localizationKey, string localizedText)
        {
            this.localizationKey = localizationKey;
            label.text = localizedText;
        }

        private class LocalizedFontStyle { public float charsSpacing; public FontStyles style; }
        LocalizedFontStyle defaultStyle;
        private void HackForChineseLabels(Language lang)
        {
            if (defaultStyle == null) defaultStyle = new LocalizedFontStyle {
                charsSpacing = label.characterSpacing, style = label.fontStyle };
            if (lang.IsChinese()) {
                label.characterSpacing = 0;
                label.fontStyle &= ~FontStyles.Bold;
            } else {
                label.characterSpacing = defaultStyle.charsSpacing;
                label.fontStyle = defaultStyle.style;
            }
        }
#endif
    }
}
