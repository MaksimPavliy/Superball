#if ECS_INCOME_FOR_VIDEO
using FriendsGamesTools.Ads;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.IncomeForVideo
{
    public abstract class IncomeForVideoWindow<TSelf> : Window
        where TSelf : IncomeForVideoWindow<TSelf>
    {
        public static void Show() => Show<TSelf>();
        [SerializeField] protected WatchAdButtonView watch;
        [SerializeField] TextMeshProUGUI multiplier;
        [SerializeField] TextMeshProUGUI duration;
        protected virtual void OnEnable()
        {
            if (multiplier != null)
                multiplier.text = $"x{controller.multiplier.ToString(0)}";
            if (duration != null)
                duration.text = controller.duration.ToShownTime();
        }

        protected virtual void Awake()
        {
            if (watch != null)
                watch.SubscribeAdWatched(OnAdWatched);
        }
        protected IncomeForVideoController controller => GameRoot.instance.Get<IncomeForVideoController>();
        protected virtual void OnAdWatched() {
            shown = false;
            controller.AddMultiplier();
        }
    }
}
#endif