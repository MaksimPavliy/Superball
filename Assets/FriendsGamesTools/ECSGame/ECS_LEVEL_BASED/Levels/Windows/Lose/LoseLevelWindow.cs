using FriendsGamesTools;
using FriendsGamesTools.Ads;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class LoseLevelWindow : EndLevelWindow
    {
        [SerializeField] protected WatchAdButtonView continueForAdButton;
        [SerializeField] protected Button restartLevelButton;
        public float showingDelay = 0.5f;
#if ECS_LEVEL_BASED
        public static void Show() => ShowWithDelay<LoseLevelWindow>(w => w.showingDelay);
        public override string shownText => base.shownText + "\nFAILED!";
        protected override void Awake()
        {
            base.Awake();
            restartLevelButton.Safe(() => restartLevelButton.onClick.AddListener(OnRestartLevelPressed));
            continueForAdButton.Safe(()=> continueForAdButton.SubscribeAdWatched(OnContinueAdWatched));
        }
        protected virtual void OnContinueAdWatched()
        {
            shown = false;
            levelsController.ContinueAfterLose();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            continueForAdButton.SetActiveSafe(proposeAds);
        }
#endif
    }
}