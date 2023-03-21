#if ECS_PLAYER_MONEY
using System;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public class PlayerMoneyView : PlayerCurrencyView
    {
        public static new PlayerMoneyView instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        [SerializeField] protected TextMeshProUGUI income;
        [SerializeField] bool dollarSign = true;
        public double shownIncome { get; private set; }
        double realIncome => PlayerMoneyController.GetData().income;
        public override double realValue => PlayerMoneyController.GetData().amount;
        protected override void ShowValue(double shownValue) => money.text = $"{ToShownMoney(shownValue, dollarSign)}";
        protected virtual void ShowIncome(double shownIncome)
        {
            this.shownIncome = shownIncome;
            if (income != null)
                income.text = $"+{ToShownMoney(shownIncome, dollarSign)} /min";
        }
        protected void Show(double shownValue, double shownIncome)
        {
            Show(shownValue);
            ShowIncome(shownIncome);
        }

        protected override void Update()
        {
            base.Update();
            ShowIncome(Utils.MoveTowards(shownIncome, realIncome, Math.Max(shownIncome, realIncome) * addSpeed * GameTime.deltaTime));
        }
    }
}
#endif