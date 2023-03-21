using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Ads
{
    public class RewardedNativeWindow : VideoAdNativeWindow
    {
        [SerializeField] Button debugFailInstantly;
#if ADS
        public static void Show(Action<bool> onComplete)
        {
            var inst = Show($"RewardedNativeWindow");
            var window = inst as RewardedNativeWindow;
            window.onComplete = onComplete;
        }
        protected override float showCloseDelay => videoDuration;
        protected override void Show()
        {
            base.Show();
            debugFailInstantly.onClick.AddListener(OnFailInstantlyPressed);
        }
        Action<bool> onComplete;
        bool success;
        protected override void OnCompleteInstantlyPressed()
        {
            success = true;
            Close();
        }
        public override void OnClosePressed()
        {
            if (videoIsFinished)
                success = true;
            base.OnClosePressed();
        }
        protected void OnFailInstantlyPressed() => Close();
        protected override void Close()
        {
            base.Close();
            onComplete?.Invoke(success);
        }
#endif
    }
}
