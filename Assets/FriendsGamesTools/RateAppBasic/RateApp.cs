#if RATE_APP_BASIC

namespace FriendsGamesTools
{
    public static class RateApp
    {
        public static void Open()
        {
#if !UNITY_EDITOR && UNITY_IOS
            UnityEngine.iOS.Device.RequestStoreReview();
#elif !UNITY_EDITOR && UNITY_ANDROID
            UnityEngine.Application.OpenURL("market://details?id=" + UnityEngine.Application.identifier);
#else
            RateAppNativeWindow.Show();
#endif
        }
    }
}
#endif