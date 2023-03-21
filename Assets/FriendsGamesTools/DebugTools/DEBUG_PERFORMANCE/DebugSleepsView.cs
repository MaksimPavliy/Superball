
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugSleepsView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI curr;
        [SerializeField] TMP_InputField input;
        [SerializeField] Button set;
#if DEBUG_PERFORMANCE
        void Show() => curr.text = $"sleep = {DebugSleeps.sleepMS}";
        private void Awake()
        {
            Show();
            set.onClick.AddListener(OnSetPressed);
        }
        private void OnSetPressed()
        {
            if (!int.TryParse(input.text, out var value))
                return;
            Debug.Log($"change sleep from {DebugSleeps.sleepMS} to {value}");
            DebugSleeps.sleepMS = value;
            Show();
        }
#endif
    }
}