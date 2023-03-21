using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugControllersPerformance : MonoBehaviour
    {
        [SerializeField] Button enableButton, disableButton;
#if DEBUG_PERFORMANCE && ECS_GAMEROOT
        bool ecsEnabled;
        private void Awake()
        {
            ShowEnabled(true);
            ecsEnabled = ECSGame.GameRoot.instance?.runningType == ECSGame.GameRunningType.Auto;
            if (enableButton != null)
                enableButton.onClick.AddListener(() => SetControllersEnabled(true));
            if (disableButton != null)
                disableButton.onClick.AddListener(() => SetControllersEnabled(false));
        }
        ECSGame.GameRoot root => ECSGame.GameRoot.instance;
        void SetControllersEnabled(bool enabled)
        {
            if (root == null) return;
            root.runningType = enabled ? ECSGame.GameRunningType.Auto : ECSGame.GameRunningType.Manual;
            ShowEnabled(enabled);
        }
        void ShowEnabled(bool enabled)
        {
            enableButton.gameObject.SetActive(!enabled);
            disableButton.gameObject.SetActive(enabled);
        }
#endif
    }
}