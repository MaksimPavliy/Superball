using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class MessageWindowCustomization : WindowCustomization
    {
        [SerializeField] TextMeshProUGUI header, message, okText;
        [SerializeField] Image window, button;
        MessageWindowSettings settings => MessageWindowSettings.instance;
        protected override void Customize()
        {
            SetFont(header, settings.headerFont);
            SetFont(message, settings.messageFont);
            SetFont(okText, settings.okFont);
            SetPic(window, settings.windowPic);
            SetPic(button, settings.buttonPic);
        }
    }
}
