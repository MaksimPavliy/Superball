using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Ads
{
    public abstract class VideoAdNativeWindow : NativeWindow
    {
        [SerializeField] GameObject closeParent;
        [SerializeField] GameObject videoFinished, videoInProgress;
        [SerializeField] TextMeshProUGUI videoRemaining;
        [SerializeField] Button debugCompleteInstantly;
#if ADS
        protected abstract float showCloseDelay { get; }
        protected const float videoDuration = 30;
        float elapsed;
        protected float videoRemainingTime => videoDuration - elapsed;
        protected bool videoIsFinished => videoRemainingTime < 0;
        protected override void Show()
        {
            base.Show();
            debugCompleteInstantly.onClick.AddListener(OnCompleteInstantlyPressed);
            elapsed = 0;
        }
        protected abstract void OnCompleteInstantlyPressed();
        protected override void Update()
        {
            base.Update();
            elapsed += UnityEngine.Time.unscaledDeltaTime;
            videoRemaining.text = videoRemainingTime.ToShownTime();
            videoFinished.SetActive(videoIsFinished);
            videoInProgress.SetActive(!videoIsFinished);
            closeParent.SetActive(showCloseDelay < elapsed);
        }
#endif
    }
}
