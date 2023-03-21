using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class RateAppButton : MonoBehaviour
    {
        [SerializeField] Button button;
#if RATE_APP_BASIC
        private void Awake() {
            if (button != null)
                button.onClick.AddListener(OnRatePressed);
        }
        protected virtual void OnRatePressed()
        {
            RateApp.Open();
            onPressed?.Invoke();
        }
        public event Action onPressed;
#endif
    }
}