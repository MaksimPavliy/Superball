using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.Ads
{
    public class X1X3EarningView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI x1Amount;
        [SerializeField] TextMeshProUGUI x3Amount;
        [SerializeField] TextMeshProUGUI x3Multiplier;
        [SerializeField] WatchAdButtonView x3Button;
        [SerializeField] Button x1Button;
        [SerializeField] bool dollarSign = true;
#if ADS

        Action<double> onPressedMultiplier;
        Action<bool> onPressedMultiplied;
        double multiplier;
        private void Awake()
        {
            if (x3Button != null)
                x3Button.SubscribeAdWatched(() => OnPressed(true));
            if (x1Button != null)
                x1Button.onClick.AddListener(() => OnPressed(false));
        }
        void OnPressed(bool multiplied)
        {
            onPressedMultiplier?.Invoke(multiplied ? multiplier : 1);
            onPressedMultiplied?.Invoke(multiplied);
        }
        public void Show(double amount, Action<double> onPressedMultiplier, double multiplier)
            => Show(amount, onPressedMultiplier, null, multiplier);
        public void Show(double amount, Action<bool> onPressedMultiplied, double multiplier)
            => Show(amount, null, onPressedMultiplied, multiplier);
        private void Show(double amount, Action<double> onPressedMultiplier, Action<bool> onPressedMultiplied, double multiplier)
        {
            this.multiplier = multiplier;
            this.onPressedMultiplier = onPressedMultiplier;
            this.onPressedMultiplied = onPressedMultiplied;
            x1Amount.SetTextSafe(amount.ToShownMoney(dollarSign));
            x3Amount.SetTextSafe((amount * multiplier).ToShownMoney(dollarSign));
            x3Multiplier.SetTextSafe($"X{multiplier}");
        }
#endif
    }
}
