using FriendsGamesTools.DebugTools;
using FriendsGamesTools.ECSGame.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class TutorialDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "TUTORIAL";
        [SerializeField] Toggle tutorialEnabled;
        [SerializeField] TextMeshProUGUI shownChaprter;
#if TUTORIAL
        Tutorial.TutorialSettings tutorialConfig => Tutorial.TutorialSettings.instance;
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            tutorialEnabled.onValueChanged.AddListener(val => tutorialConfig.enabled = val);
        }
        protected override void OnEnablePlaying() => tutorialEnabled.isOn = tutorialConfig.enabled;
        protected override void UpdatePlaying()
        {
            base.UpdatePlaying();
            shownChaprter.text = TutorialManager.instance?.shownChapter?.GetType().Name ?? "no chapter shown";
        }
#endif
    }
}
