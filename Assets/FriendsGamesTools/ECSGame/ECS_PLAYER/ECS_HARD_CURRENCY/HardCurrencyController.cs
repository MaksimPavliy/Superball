#if ECS_HARD_CURRENCY
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player;
using FriendsGamesTools.ECSGame.Player.Money;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public struct HardCurrency : IComponentData { public int amount; }
    [UpdateAfter(typeof(PlayerController))]
    public abstract class HardCurrencyController : Controller
    {
        public override void InitDefault()
        {
            base.InitDefault();
            PlayerController.entity.AddComponent(new HardCurrency());
        }
        public Entity e => PlayerController.entity;
        public double amount => e.GetComponentData<HardCurrency>().amount;
        public virtual void Add(int income) => e.ModifyComponent((ref HardCurrency h) => h.amount += income);
        public virtual void Pay(int price) => e.ModifyComponent((ref HardCurrency h) => h.amount -= price);

        public void DebugMultiply(float multiplier)
        {
            PlayerController.entity.ModifyComponent((ref HardCurrency h) => {
                if (h.amount > 0)
                    h.amount = Mathf.RoundToInt(h.amount * multiplier);
                else
                    h.amount = 1;
            });
        }
        public void AddSoaked(int income)
        {
            GameRoot.instance.Get<PlayerMoneyController>().AddMoneySoaked(income, 1, CurrencyType.Hard);
        }
    }
}
#endif