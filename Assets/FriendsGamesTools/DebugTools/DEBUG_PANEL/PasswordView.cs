#if DEBUG_PANEL
using System;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class PasswordView : MonoBehaviour
    {
        string password => DebugPanelSettings.instance.password.ToString();
        Action<bool> onResponse;
        string enteredPassword;
        public void Show(Action<bool> onResponse)
        {
            this.onResponse = onResponse;
            gameObject.SetActive(true);
            enteredPassword = "";
        }
        public void OnDigitPressed(int digit) {
            enteredPassword = $"{enteredPassword}{digit}";
            if (enteredPassword.Length>=password.Length)
                OnFinished(enteredPassword==password);
        }

        private void OnFinished(bool success)
        {
            gameObject.SetActive(false);
            onResponse(success);
        }
    }
}
#endif