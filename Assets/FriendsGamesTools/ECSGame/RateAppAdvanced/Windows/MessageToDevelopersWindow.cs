using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class MessageToDevelopersWindow : Window
    {
        [SerializeField] TMP_InputField message;
        [SerializeField] GameObject isSending;
#if RATE_APP_ADVANCED
        public static void Show()
        {
            var window = Windows.Get(RateAppAdvancedSettings.instance.messageToDevelopersWindow.prefab);
            window.shown = true;
        }
        void SetIsSending(bool isSending)
        {
            if (this.isSending != null)
                this.isSending.SetActive(isSending);
        }
        public async void OnSendPressed()
        {
            SetIsSending(true);
            var error = await EmailToDevs.Send("Dont like game because", message.text);
            SetIsSending(false);
            shown = false;
            MessageWindow.Show(string.IsNullOrEmpty(error) ? "Message delivered to developers" : error);
        }
#endif
    }
}
