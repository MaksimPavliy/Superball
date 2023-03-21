using FriendsGamesTools.DebugTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class RateAppAdvancedDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "RATE_APP_ADVANCED";
        [SerializeField] Button timeoutButton;
        [SerializeField] TextMeshProUGUI label;
#if RATE_APP_ADVANCED
        protected override void AwakePlaying() {
            base.AwakePlaying();
            timeoutButton.onClick.AddListener(OnX10Pressed);
        }
        RateAppAdvancedController controller => RateAppAdvancedController.instance;
        void OnX10Pressed() => controller.DebugRemoveDelay();
        protected override void UpdatePlaying()
        {
            base.UpdatePlaying();
            label.text = $"tries={controller.data.remainingAttempts}, time={controller.data.remainingTime.ToShownTime()}";
        }
#endif
    }
}
