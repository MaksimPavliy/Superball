using System.Threading.Tasks;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.PushNotifications
{
    public class AskPermissionNativeWindow : NativeWindow
    {
        [SerializeField] Button okButton, cancelButton;
#if UI
        static bool success;
        public static new async Task<bool> Show()
        {
            var instance = (AskPermissionNativeWindow)NativeWindow.Show("AskPermissionNativeWindow");
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
