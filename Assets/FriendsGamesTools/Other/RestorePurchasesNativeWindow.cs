using FriendsGamesTools.UI;
using System.Threading.Tasks;

namespace FriendsGamesTools
{
    // TODO: Move all restore purchases functionality to IAP module.
    public class RestorePurchasesNativeWindow : NativeWindow
    {
#if IAP
        static bool success;
        public static new async Task<bool> Show()
        {
            var instance = Show("RestorePurchasesNativeWindow");
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