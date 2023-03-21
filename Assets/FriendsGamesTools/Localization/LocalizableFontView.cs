using FriendsGamesTools;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public class LocalizableFontView : MonoBehaviour
    {
#if LOCALIZATION
        private TextMeshProUGUI textMesh;
        private LocalizableView view;
        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            view = GetComponent<LocalizableView>();
            view.Localized += OnLocalized;
        }
        private void OnLocalized()
        {
            var fontSettings = LocalizationSettings.instance.GetFontSettings(LocalizationSettings.instance.playerLanguage);
            if (fontSettings!=null)
            {
                textMesh.font = fontSettings.fontAsset;
             }
        }
#endif
    }

}