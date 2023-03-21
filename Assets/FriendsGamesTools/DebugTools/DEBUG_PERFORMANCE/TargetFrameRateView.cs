using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class TargetFrameRateView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI curr;
        [SerializeField] TMP_InputField input;
        [SerializeField] Button set;
#if DEBUG_PERFORMANCE
        void Show() => curr.text = $"TgtFrameRate = {Application.targetFrameRate}";
        private void Awake() => set.onClick.AddListener(OnSetPressed);
        private void OnEnable() => Show();
        private void OnSetPressed()
        {
            if (!int.TryParse(input.text, out var value))
                return;
            Debug.Log($"change targetFrameRate from {Application.targetFrameRate} to {value}");
            Application.targetFrameRate = value;
            Show();
        }
#endif
    }
}
