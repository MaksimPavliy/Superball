using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class OpenSettings : MonoBehaviour
    {
        [SerializeField] Button button;
#if SETTINGS
        private void Awake() {
            if (button != null)
                button.onClick.AddListener(OnOpenSettingsPressed);
        }
        public virtual void OnOpenSettingsPressed() => SettingsWindow.Show();
#endif
    }
}