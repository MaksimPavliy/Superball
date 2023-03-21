using System.Threading.Tasks;
using FriendsGamesTools;
using FriendsGamesTools.Ads;
using FriendsGamesTools.ECSGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class RewardProgressWindow : EndLevelWindow
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private WatchAdButtonView multiplyRewardButton;
        [SerializeField] private WatchAdButtonView restartLevelButton;
        [SerializeField] Image skinIco, skinIcoFilled;
        [SerializeField] TextMeshProUGUI progressText;
#if ECS_SKIN_PROGRESS || ECS_SKINS
        public static void Show() => Show<RewardProgressWindow>();
        public override string shownText => base.shownText + "\nCOMPLETED!";
        ProgressSkinController controller => GameRoot.instance.Get<ProgressSkinController>();
        protected override void Awake()
        {
            base.Awake();
            nextLevelButton.Safe(() => nextLevelButton.onClick.AddListener(() => OnNextLevelPressed()));
            restartLevelButton.Safe(() => restartLevelButton.SubscribeAdWatched(OnRestartLevelPressed));
            multiplyRewardButton.Safe(() => multiplyRewardButton.SubscribeAdWatched(OnMultiplyRewardPressed));
        }
        protected override async void OnEnable()
        {
            base.OnEnable();
            restartLevelButton.SetActiveSafe(false);
            multiplyRewardButton.SetActiveSafe(false);
            nextLevelButton.SetActiveSafe(false);
            await GivingRewardAnimated(AdType.forWin);
            if (!shown) return;
            multiplyRewardButton.SetActiveSafe(AdsManager.AdsEnabled);
            await Awaiters.Seconds(1f);
            await Awaiters.While(() => showingAnim);
            restartLevelButton.SetActiveSafe(!controller.unlockAvailable && AdsManager.AdsEnabled);
            nextLevelButton.SetActiveSafe(true);
        }
        async void OnMultiplyRewardPressed()
        {
            multiplyRewardButton.SetActiveSafe(false);
            await GivingRewardAnimated(AdType.forAd);
        }
        bool showingAnim;
        protected enum AdType { forWin, forAd }
        async Task GivingRewardAnimated(AdType type)
        {
            await Awaiters.While(() => showingAnim);
            showingAnim = true;
            var startValue = controller.progress;
            if (type == AdType.forWin)
                controller.GiveWinProgress();
            else
                controller.GiveAdProgress();
            var endValue = controller.progress;
            await AsyncUtils.SecondsWithProgress(0.5f, progress => {
                var value = Mathf.Lerp(startValue, endValue, Mathf.SmoothStep(0, 1, progress));
                UpdateFillValue(value);
            });
            if (controller.unlockAvailable)
                UnlockProgressSkinWindow.Show();
            showingAnim = false;
        }
        private void UpdateFillValue(float value)
        {
            ProgressSkinItemView.Show(skinIco, skinIcoFilled, controller.skinIndToUnlock, value);
            progressText.text = value.ToShownPercents();
        }
        protected void OnNextLevelPressed()
        {
            GoToNextLevel();
        }
        void GoToNextLevel()
        {
            shown = false;
            levelsController.ChangeLocation();
            MainMenuWindow.Show();
        }
#endif
    }
}