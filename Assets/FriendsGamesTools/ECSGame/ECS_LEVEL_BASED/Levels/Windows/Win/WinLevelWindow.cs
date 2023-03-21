using System.Threading.Tasks;
using FriendsGamesTools;
using FriendsGamesTools.Ads;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player.Money;
using FriendsGamesTools.Share;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class WinLevelWindow : EndLevelWindow
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private WatchAdButtonView xAdRewardForButton;
        [SerializeField] private string xAdRewardText = "Get x{0}";
        [SerializeField] private TextMeshProUGUI xAdRewardTitle;
        [SerializeField] protected GainedRewardView rewardView;
        [SerializeField] Transform starsParent;
        public float showingDelay = 0.5f;
        public float starsAppearanceDelay = 0.2f;
#if ECS_LEVEL_BASED
        public static void Show() => ShowWithDelay<WinLevelWindow>(w => w.showingDelay);
        StarView[] stars;
        WinnableLocationsController controller => GameRoot.instance.Get<WinnableLocationsController>();
        public override string shownText => base.shownText + "\nCOMPLETED!";

        protected override void Awake()
        {
            base.Awake();
            nextLevelButton.Safe(() => nextLevelButton.onClick.AddListener(NextPressed));
            AwakeAdReward();
        }

        #region Money multiplying
        int xRewardForAd;
#if ADS
        protected virtual async void OnXRewardAdWatched()
        {
            buttonsParent.SetActive(false);
            xAdRewardForButton?.gameObject.SetActive(false);
            rewardView.PlayTween();
            await Awaiters.Seconds(0.25f);
            rewardView.SetReward(controller.levelWinMoney * xRewardForAd);
            await Awaiters.Seconds(1f);
            ShowNextWindow(xRewardForAd);
        }
        void AwakeAdReward()
        {
            xAdRewardForButton.Safe(() => xAdRewardForButton.SubscribeAdWatched(OnXRewardAdWatched));
        }
        void InitRewardForAd()
        {
            xRewardForAd = Utils.Chance(controller.levelWinX3Chance) ? 3 : 2;
            xAdRewardForButton.Safe(() => xAdRewardForButton.gameObject.SetActive(true));
            xAdRewardTitle.SetTextSafe(string.Format(xAdRewardText, xRewardForAd));
        }
#else
        void AwakeAdReward(){}
        void InitRewardForAd()
        {
            xRewardForAd = 2;
            xAdRewardForButton.Safe(() => xAdRewardForButton.gameObject.SetActive(false));
        }
#endif
        #endregion

        const int MaxStarsCount = 3;
        void InitStars()
        {
            if (stars.CountSafe() != MaxStarsCount)
                stars = starsParent.GetComponentsInChildren<StarView>();
            stars.ForEach(s => s.SetState(false));
        }
        protected virtual Task ShowingReward()
        {
            rewardView.SetReward(controller.levelWinMoney);
            return Task.CompletedTask;
        }
        protected override async void OnEnable()
        {
            InitStars();
            base.OnEnable();
            InitRewardForAd();
            buttonsParent.SetActive(false);

            await ShowingReward();

            await Awaiters.Seconds(starsAppearanceDelay);

            Haptic.Vibrate(HapticType.Light);
            for (int i = 0; i < MaxStarsCount; i++)
            {
                stars[i].SetState(controller.winStarsCount >= i + 1);
                await Awaiters.Seconds(0.5f);
                Haptic.Vibrate(HapticType.Medium);
            }
            buttonsParent.SetActive(true);
        }

        protected virtual void NextPressed()
        {
            ShowNextWindow(1);
        }

        bool moneySoakIsPlaying => MoneySoakEffect.instance.isPlaying;
        public async void ShowNextWindow(int moneyMultiplier)
        {
            buttonsParent.SetActive(false);
            levelsController.GiveWinMoney(moneyMultiplier);
            await Awaiters.Until(() => moneySoakIsPlaying);
            await Awaiters.Until(() => !moneySoakIsPlaying);
            await AsyncUtils.FramesCount(3);
            shown = false;
#if ECS_SKIN_PROGRESS
            if (GameRoot.instance.Get<ProgressSkinController>().anySkinLocked)
            {
                RewardProgressWindow.Show();
                return;
            }
#endif
            levelsController.ChangeLocation();
            MainMenuWindow.Show();
        }
#endif
    }
}