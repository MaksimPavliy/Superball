#if LOCALIZATION

namespace FriendsGamesTools
{
    public static class Localization {
        public static LocalizedText Get(string localizationKey, params string[] parameters)
            => LocalizedText.Create(localizationKey, parameters);
        public static LocalizedText Get(string localizationKey, LocalizedText param, params LocalizedText[] moreParams)
            => LocalizedText.Create(localizationKey, param, moreParams);
    }
}
#endif