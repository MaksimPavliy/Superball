using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class SwitchLanguagesView : MonoBehaviour
    {
        [SerializeField] LanguageButton buttonPrefab;

#if LOCALIZATION
        List<LanguageButton> shownButtons = new List<LanguageButton>();
        LocalizationSettings settings => LocalizationSettings.instance;
        private void Awake()
        {
            shownButtons.Add(buttonPrefab);
            Utils.UpdatePrefabsList(shownButtons, settings.activeLanguages, buttonPrefab, transform, (lang, button) => button.Show(lang));
            AsyncUtils.ExecuteAfterFrames(1, () => gameObject.SetActive(false));
            AsyncUtils.ExecuteAfterFrames(2, () => gameObject.SetActive(true));
        }
#endif
    }
}