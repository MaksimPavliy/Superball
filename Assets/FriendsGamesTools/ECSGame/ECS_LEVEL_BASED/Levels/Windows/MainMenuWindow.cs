using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;
#if ECS_LEVEL_BASED
using FriendsGamesTools.ECSGame.Locations;
#endif

namespace FriendsGamesTools.ECSGame
{
    public class MainMenuWindow : Window
    {
        [SerializeField] private Button _buttonPlay;
        [SerializeField] bool playOnButtonDown;
        [FormerlySerializedAs("_bottonCustomization")]
        [SerializeField] private Button _buttonCustomization;
#if ECS_LEVEL_BASED
        public static void Show() {
            GameRoot.instance.Get<WinnableLocationsController>().GoToMenu();
            Show<MainMenuWindow>();
            LevelBasedView.SetLevelText(LocationsView.instance.LocationName);
        }
        private void Awake() {
            if (!playOnButtonDown)
                _buttonPlay.onClick.AddListener(OnPlayPressed);
            else {
                var trigger = _buttonPlay.gameObject.AddComponent<EventTrigger>();
                var pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((e) => OnPlayPressed());
                trigger.triggers.Add(pointerDown);
            }
#if ECS_SKINS
            _buttonCustomization.Safe(() => _buttonCustomization.onClick.AddListener(SkinsWindow.Show));
#endif
        }
        private void OnPlayPressed() {
            shown = false;
            GameRoot.instance.Get<WinnableLocationsController>().Play();
        }
#endif
    }
}