using FriendsGamesTools.ECSGame.Player.Money;
using UnityEngine;

namespace Superball
{
    public class SuperballMoneyController : PlayerMoneyController
    {
        public new int amount => Mathf.RoundToInt((float)base.amount);
        public void AddWinMoney(int amount) => AddMoneySoaked(amount);
        public override void InitDefault()
        {
            base.InitDefault();
            SetStartMoney(0);
        }
    }
}