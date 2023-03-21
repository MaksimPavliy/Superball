using FriendsGamesTools;
using FriendsGamesTools.Ads;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ECS_LEVEL_BASED
using FriendsGamesTools.ECSGame.Locations;
#endif

namespace FriendsGamesTools.ECSGame
{
    public abstract class EndLevelWindow : Window
    {
        [SerializeField] protected GameObject buttonsParent;
        [SerializeField] protected TextMeshProUGUI shownTextLabel;
#if ECS_LEVEL_BASED
        protected bool proposeAds => AdsManager.AdsEnabled;
        public virtual string shownText => LocationsView.instance.LocationName;
        protected WinnableLocationsController levelsController => GameRoot.instance.Get<WinnableLocationsController>();
        protected virtual void OnEnable()
        {
            LevelBasedView.SetLevelText(string.Empty);
            shownTextLabel.Safe(() => shownTextLabel.text = shownText);
            Haptic.Vibrate(HapticType.Medium);
        }
        protected virtual void Awake() { }
        protected virtual void OnRestartLevelPressed()
        {
            shown = false;
            levelsController.RestartLocation();
            MainMenuWindow.Show();
        }
#endif
    }
}