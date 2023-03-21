using FriendsGamesTools.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public class SpendMoneyNativeWindow : NativeWindow
    {
        [SerializeField] TextMeshProUGUI priceLabel;
#if UI
        static bool success;
        public static new async Task<bool> Show(string moneyString)
        {
            var instance = (SpendMoneyNativeWindow)NativeWindow.Show("SpendMoneyNativeWindow");
            instance.priceLabel.text = moneyString;
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