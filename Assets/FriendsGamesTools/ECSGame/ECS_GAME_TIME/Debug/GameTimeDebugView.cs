using System;
using FriendsGamesTools.DebugTools;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class GameTimeDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_GAME_TIME";
        [SerializeField] Button xDot25, xDot50, x1, x2, x4, x8;
        [SerializeField] GameObject disabledByReleaseParent;
        [SerializeField] Toggle showPause;
#if ECS_GAME_TIME
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            xDot25.onClick.AddListener(OnXDot25TimePressed);
            xDot50.onClick.AddListener(OnXDot50TimePressed);
            x1.onClick.AddListener(OnX1TimePressed);
            x2.onClick.AddListener(OnX2TimePressed);
            x4.onClick.AddListener(OnX4TimePressed);
            x8.onClick.AddListener(OnX8TimePressed);
            disabledByReleaseParent.SetActive(false);// BuildModeSettings.release);
            showPause.isOn = PlayPauseView.shouldBeShown;
            showPause.interactable = !BuildModeSettings.release;
            showPause.onValueChanged.AddListener(OnShowPauseToggleChanged);
        }
        private void OnShowPauseToggleChanged(bool on) => PlayPauseView.ShownInDebug = on;
        void OnXDot25TimePressed() => SetTime(0.25f);
        void OnXDot50TimePressed() => SetTime(0.5f);
        void OnX1TimePressed() => SetTime(1f);
        void OnX2TimePressed() => SetTime(2f);
        void OnX4TimePressed() => SetTime(4f);
        void OnX8TimePressed() => SetTime(8f);

        void SetTime(float speed) => DebugTools.DebugSettings.instance.timeSpeed = speed;
#endif
    }
}
