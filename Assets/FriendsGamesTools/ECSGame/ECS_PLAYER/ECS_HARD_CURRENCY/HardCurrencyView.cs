using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player.Money;
using FriendsGamesTools.UI;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class HardCurrencyView : PlayerCurrencyView
    {
        [SerializeField] TweenInTime notEnoughTween;
#if ECS_HARD_CURRENCY
        public static new HardCurrencyView instance { get; private set; }
        public override double realValue => GameRoot.instance.Get<HardCurrencyController>().amount;
        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        public void ShowNotEnough() => notEnoughTween.Play(2);
#elif ECS_PLAYER_MONEY
        public override double realValue => -1;
#endif
    }
}