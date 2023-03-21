#if LOCALIZATION

using System;
using UnityEngine;

namespace FriendsGamesTools
{
    public class LocalizationManager : MonoBehaviourSingleton<LocalizationManager> {

        static LocalizationSettings settings => LocalizationSettings.instance;
        void Start()
        {
            var (saved, lang) = LoadSavedLanguage();
            if (!saved)
            {
                lang = Application.systemLanguage.ToLanguage();
                if (!settings.activeLanguages.Contains(lang))
                    lang = settings.defaultPlayerLanguage;
            }
            settings.playerLanguage = lang;
        }

        const string PrefsKey = "playerLanguage";
        public static (bool exists, Language lang) LoadSavedLanguage() {
            var success = Enum.TryParse<Language>(PlayerPrefs.GetString(PrefsKey), out var lang);
            return (success, lang);
        }
        public static void SaveLanguage()
            => PlayerPrefs.SetString(PrefsKey, settings.playerLanguage.ToString());
    }
}
#endif