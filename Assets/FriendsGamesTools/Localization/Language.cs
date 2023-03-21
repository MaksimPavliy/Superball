using System.Threading;
using UnityEngine;

namespace FriendsGamesTools
{
    public enum Language
    {
        // CHs = Chinese simplified, CHt = traditional
        EN = 0, UA = 1, RU = 2, CHs = 3, CHt = 4, HIN=5, IND=6, ESP=7
    }

    public static class LanguageUtils
    {
        public static Language ToLanguage(this SystemLanguage systemLang)
        {
            switch (systemLang)
            {
                case SystemLanguage.English: return Language.EN;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified: return Language.CHs;
                case SystemLanguage.ChineseTraditional: return Language.CHt;
                case SystemLanguage.Ukrainian: return Language.UA;
                case SystemLanguage.Russian: return Language.RU;
                case SystemLanguage.Indonesian: return Language.IND;
                case SystemLanguage.Spanish: return Language.ESP;
                case SystemLanguage.Unknown:

                    System.Globalization.CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                    
                    if (currentCulture.ThreeLetterISOLanguageName=="hin")
                    {
                        return Language.HIN;
                    }
                return LocalizationSettings.instance.defaultPlayerLanguage;
                
                default: return LocalizationSettings.instance.defaultPlayerLanguage;
            }
        }
        public static bool IsChinese(this Language lang) => lang == Language.CHs || lang == Language.CHt;
    }
}