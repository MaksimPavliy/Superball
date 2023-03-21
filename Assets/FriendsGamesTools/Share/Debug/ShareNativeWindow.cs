using System.Threading.Tasks;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Share
{
    public class ShareNativeWindow : NativeWindow
    {
        [SerializeField] Button okButton, cancelButton;
        [SerializeField] TextMeshProUGUI title, text;
#if SHARE
        static bool success;
        public static async Task<bool> Show(string title, string text)
        {
            var instance = (ShareNativeWindow)NativeWindow.Show("ShareNativeWindow");
            instance.title.text = title;
            instance.text.text = text;
            instance.okButton?.onClick.AddListener(instance.OnOkSuccessPressed);
            instance.cancelButton?.onClick.AddListener(instance.OnClosePressed);
            success = false;
            while (instance != null)
                await Awaiters.EndOfFrame;
            return success;
        }
        public void OnOkSuccessPressed()
        {
            success = true;
            Close();
        }
#endif
    }
}