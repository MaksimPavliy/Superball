using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class LanguageButton : WithLocalizationCallbackInRuntime
    {
        [SerializeField] Image ico;
        [SerializeField] Button button;
        [SerializeField] GameObject selectedParent;

#if LOCALIZATION
        Language lang;
        LocalizationSettings settings => LocalizationSettings.instance;
        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(OnLanguagePressed);
        }
        public void Show(Language lang)
        {
            this.lang = lang;
            ico.sprite = LanguageIcons.Get(lang);
            UpdateView();
        }
        private void OnLanguagePressed()
        {
            settings.playerLanguage = lang;
        }
        void UpdateView()
        {
            selectedParent.SetActive(lang == settings.playerLanguage);
        }
        public override void SetLocalizationInRuntime(Language lang, LocalizationSettings settings)
        {
            UpdateView();
        }
#endif
    }
}