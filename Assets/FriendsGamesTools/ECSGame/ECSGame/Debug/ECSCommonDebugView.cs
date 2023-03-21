using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    [Obsolete]
    public class ECSCommonDebugView : MonoBehaviour
    {
        [SerializeField] GameObject gameRootParent;
        [SerializeField] Button resetButton;
        [SerializeField] GameObject moneyParent;
        [SerializeField] GameObject levelParent;
        [SerializeField] GameObject locationsParent;
        [SerializeField] GameObject timeParent;
        [SerializeField] GameObject bonusParent;
        [SerializeField] Button bonusButtonPrefab;
        [SerializeField] Toggle tutorialEnabled;
        [SerializeField] GameObject tutorialParent;

#if ECSGame && DEBUG_PANEL
        void OnEnable()
        {
            InitGameRoot();
            moneyParent.SetActiveSafe(money);
            levelParent.SetActiveSafe(level);
            locationsParent.SetActiveSafe(locations);
            timeParent.SetActiveSafe(time);
            bonusParent.SetActiveSafe(bonus);
            InitBonus();
            InitTutorial();
        }

#if ECS_GAMEROOT
        void InitGameRoot()
        {
            gameRootParent.SetActiveSafe(gameRoot);
            resetButton.interactable = true;
#if TUTORIAL
            resetButton.interactable = !Tutorial.TutorialManager.instance.chapterShown;
#endif
        }
        bool gameRoot => true;
        public void OnResetPressed()
        {
#if WINDOWS
            UI.Windows.CloseAll();
#endif
            GameRoot.instance.ResetWorld();
        }
        public void OnEmailSavePressed() => Serialization.SendSaveToDeveloper("from debug panel");
#else
        InitGameRoot() {}
        bool gameRoot => false;
#endif

#if ECS_PLAYER_MONEY
        bool money => true;
        public void OnX10Pressed() => GameRoot.instance.Get<Player.Money.PlayerMoneyController>().DebugMultiply(10);
        public void OnDiv10Pressed() => GameRoot.instance.Get<Player.Money.PlayerMoneyController>().DebugMultiply(0.1d);
#else
        bool money => false;
#endif

#if ECS_PLAYER_LEVEL
        bool level => true;
        public void OnAddLevelPressed() => GameRoot.instance.Get<Player.Level.PlayerLevelController>().DebugAddLevel(1);
#else
        bool level => false;
#endif

#if ECS_LOCATIONS
        bool locations => true;
        Locations.LocationsController locationsController => GameRoot.instance.Get<Locations.LocationsController>();
        public void OnNextLocationPressed() => locationsController.DebugChangeLocation(locationsController.currLocationInd + 1);
#else
        bool locations => false;
#endif

#if ECS_GAME_TIME
        bool time => true;
        public void OnXDot25TimePressed() => SetTime(0.25f);
        public void OnXDot50TimePressed() => SetTime(0.5f);
        public void OnX1TimePressed() => SetTime(1f);
        public void OnX2TimePressed() => SetTime(2f);
        public void OnX4TimePressed() => SetTime(4f);
        void SetTime(float speed) => DebugTools.DebugSettings.instance.timeSpeed = speed;
#else
        bool time => false;
#endif

#if ECS_BONUS
        bool bonus => true;
        List<BonusEvent.BonusEventController> bonusControllers;
        List<Button> bonusButtons = new List<Button>();
        void InitBonus()
        {
            bonusButtonPrefab.gameObject.SetActive(false);
            bonusControllers = GameRoot.instance.controllers.ConvertAll(c
                => c as BonusEvent.BonusEventController).Filter(c => c != null);
            Utils.UpdatePrefabsList(bonusButtons, bonusControllers, bonusButtonPrefab, bonusParent.transform, (c, button) =>
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = c.GetType().Name.Replace("Controller", "");
                button.onClick.AddListener(c.DebugAppear);
            });
        }
#else
        bool bonus => false;
        void InitBonus() {}
#endif

#if TUTORIAL
        Tutorial.TutorialSettings tutorialConfig => Tutorial.TutorialSettings.instance;
        void InitTutorial()
        {
            tutorialEnabled.isOn = tutorialConfig.enabled;
            tutorialEnabled.onValueChanged.AddListener(val=>tutorialConfig.enabled = val);
        }
#else
        void InitTutorial() { }
#endif

#endif
    }
}
