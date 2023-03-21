using System.Threading.Tasks;
using FriendsGamesTools.Ads;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class UnlockProgressSkinWindow : Window
    {
        [SerializeField] Image ico, icoFilled;
        [SerializeField] WatchAdButtonView unlockButton;
        [SerializeField] Button unlockFreeButton;
#if ECS_SKIN_PROGRESS || ECS_SKINS
        public static void Show() => Show<UnlockProgressSkinWindow>();
        private void Awake()
        {
#if ADS
            unlockButton.SubscribeAdWatched(OnAdWatched);
            unlockFreeButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(true);
#else
            unlockFreeButton.onClick.AddListener(() => ReceiveAnswer(true));
            unlockFreeButton.gameObject.SetActive(true);
            unlockButton.gameObject.SetActive(false);
#endif
        }
        public override void OnClosePressed() => ReceiveAnswer(false);
        void OnAdWatched() => ReceiveAnswer(true);
        void ReceiveAnswer(bool unlock)
        {
            this.unlock = unlock;
            answerReceived = true;
        }
        bool answerReceived;
        bool unlock;
        ProgressSkinController progressSkin => GameRoot.instance.Get<ProgressSkinController>();
        WinnableLocationsController levels => GameRoot.instance.Get<WinnableLocationsController>();
        async void OnEnable()
        {
            answerReceived = false;
            ProgressSkinItemView.Show(ico, icoFilled, GameRoot.instance.Get<ProgressSkinController>().skinIndToUnlock, 1f);
            await Awaiters.Until(() => answerReceived);
            shown = false;
            progressSkin.UnlockOrLooseSkin(unlock);
            levels.ChangeLocation();
            MainMenuWindow.Show();
        }
#endif
    }
}