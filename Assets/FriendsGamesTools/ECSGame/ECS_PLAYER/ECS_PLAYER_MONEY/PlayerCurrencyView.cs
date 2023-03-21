using System;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public enum CurrencyType { Soft, Hard }
    public abstract class PlayerCurrencyView : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI money;
        [SerializeField] protected float addSpeed = 10;
#if ECS_PLAYER_MONEY
        public static PlayerCurrencyView instance { get; private set; }
        protected virtual void Awake() => instance = this;
        
        public double shownValue { get; private set; }
        public abstract double realValue { get; }
        protected virtual void ShowValue(double shownValue) => money.text = $"{ToShownMoney(shownValue, false)}";
        public static string ToShownMoney(double amount, bool dollarSign = true)
            => Utils.ToShownMoney(amount, dollarSign);
        protected void Show(double shownValue)
        {
            this.shownValue = shownValue;
            ShowValue(shownValue);
        }

        public TweenInTime moneyIcoTween;
        public virtual void Bump()
        {
#if HAPTIC
            Haptic.Vibrate();
#endif
            moneyIcoTween.PlayOnce();
        }

        protected virtual void Update()
        {
            Show(Utils.MoveTowards(shownValue, realValue, Math.Max(shownValue, realValue) * addSpeed * GameTime.deltaTime));
        }
#endif
    }
}
