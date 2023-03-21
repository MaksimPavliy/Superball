#if LOCALIZATION
using System;
using System.Collections.Generic;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;

namespace FriendsGamesTools
{
    [Serializable]
    public sealed class LocalizedText
    {
        [SerializeField] string localizationKey;
        private struct Param { public string plainText; public LocalizedText localizedText; }
        List<Param> parameters;
        public static LocalizedText Create(string localizationKey, params string[] parameters)
            => CreatePrivate(localizationKey, parameters.CountSafe() > 0 ? parameters.ConvertAll(p => new Param { plainText = p }) : null);
        public static LocalizedText Create(string localizationKey, LocalizedText param, params LocalizedText[] moreParams)
        {
            var parameters = new List<Param>();
            parameters.Add(new Param { localizedText = param });
            moreParams.ForEach(p => parameters.Add(new Param { localizedText = param }));
            return CreatePrivate(localizationKey, parameters);
        }
        private static LocalizedText CreatePrivate(string localizationKey, List<Param> parameters)
        {
            var localized = new LocalizedText();
            localized.localizationKey = localizationKey;
            if (parameters != null)
                localized.parameters = parameters;
            return localized;
        }
        private LocalizedText() { }

        public void Show(LocalizableView localizable, LocalizationSettings settings)
        {
            if (localizationKey.IsNullOrEmpty()) return;
            var localizedText = Get(this, settings);
            localizable.ShowKeyText(localizationKey, localizedText);
        }
        static List<string> formatToFind;
        public static bool GivenAndRequestedParamsCountMatch(string localizedText, int paramsCount)
        {
            if (paramsCount == 0) return true;
            if (formatToFind == null) formatToFind = new List<string>();
            while (formatToFind.Count < paramsCount)
                formatToFind.Add($"{{{formatToFind.Count}}}");
            for (int i = 0; i < paramsCount; i++)
            {
                if (!localizedText.Contains(formatToFind[i]))
                    return false;
            }
            return true;
        }
        private static string Get(LocalizedText toLocalize, LocalizationSettings settings)
        {
            // Try find localization key.
            var key = settings.Get(toLocalize.localizationKey);
            if (key == null)
            {
                Debug.LogError($"Localization key {toLocalize.localizationKey} not found");
                return toLocalize.localizationKey;
            }

            // Try find localization.
            var translation = key.GetTranslation(settings.playerLanguage);
            if (translation == null)
            {
                Debug.LogError($"Localization key {toLocalize.localizationKey} not translated to {settings.playerLanguage}");
                translation = key.GetTranslation(settings.defaultPlayerLanguage);
                if (translation == null)
                    return toLocalize.localizationKey;
            }

            // Try localize text with parameters.
            var localizedText = translation.localizedText;
            var paramsCountGiven = toLocalize.parameters.CountSafe();
            var hasParams = BuildModeSettings.release || GivenAndRequestedParamsCountMatch(localizedText, paramsCountGiven);
            string[] localizedParams = null;
            if (toLocalize.parameters!=null)
                localizedParams = toLocalize.parameters.ConvertAll(p=>
                {
                    if (!p.plainText.IsNullOrEmpty())
                        return p.plainText;
                    else
                        return Get(p.localizedText, settings); // Localize parameters recursively.
                }).ToArray();
            if (!hasParams)
            {
                if (!localizedText.IsNullOrEmpty())
                    Debug.LogError($"'{localizedText}' localization for key {toLocalize.localizationKey} should have {paramsCountGiven} params");
                return $"{(localizedParams != null ? localizedParams.PrintCollection() : "")}{localizedText}";
            }
            if (paramsCountGiven > 0)
                localizedText = string.Format(localizedText, localizedParams);
            return localizedText;
        }
    }
}
#endif