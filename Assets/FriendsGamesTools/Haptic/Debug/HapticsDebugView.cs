using FriendsGamesTools.DebugTools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class HapticsDebugView : ECSModuleDebugPanel
    {
        public override string tab => CommonTab;
        public override string module => "HAPTIC";
        [SerializeField] Toggle available, on, logs;
        [SerializeField] Button playDefault, playLight, playMedium, playHeavy;
#if HAPTIC
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            available.interactable = false;
            available.isOn = Haptic.available;
            on.interactable = Haptic.available;
            AddToggle(on, () => Haptic.on, on => Haptic.on = on);
            AddToggle(logs, () => HapticSettings.instance.log, log => HapticSettings.instance.log = log);
            playDefault.onClick.AddListener(()=>Haptic.Vibrate());
            playLight.onClick.AddListener(() => Haptic.Vibrate(HapticType.Light));
            playMedium.onClick.AddListener(() => Haptic.Vibrate(HapticType.Medium));
            playHeavy.onClick.AddListener(() => Haptic.Vibrate(HapticType.Heavy));
        }
#endif
    }
}
