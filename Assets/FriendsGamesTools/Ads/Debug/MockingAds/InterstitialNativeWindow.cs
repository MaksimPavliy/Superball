using System;

namespace FriendsGamesTools.Ads
{
    public class InterstitialNativeWindow : VideoAdNativeWindow
    {
#if ADS
        public static void Show(Action onComplete)
        {
            var inst = Show("InterstitialNativeWindow");
            var window = inst as InterstitialNativeWindow;
            window.onComplete = onComplete;
        }

        Action onComplete;
        protected override void OnCompleteInstantlyPressed() => Close();
        protected override void Close()
        {
            base.Close();
            onComplete?.Invoke();
        }
        protected override float showCloseDelay => 5;
#endif
    }
}
