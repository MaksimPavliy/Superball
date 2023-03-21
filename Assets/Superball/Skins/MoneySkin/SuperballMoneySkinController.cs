using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using HcUtils;
using System;

namespace Superball
{
    public class SuperballMoneySkinController : MoneySkinController, ISkinController
    {
        SuperballMoneySkinConfig config => SuperballMoneySkinConfig.instance;

        public event Action<int> SkinActivated;
        public override int GetSkinPrice(int unlockedSkinsCount)
            => config.priceBase * Utils.Pow(config.priceMul, unlockedSkinsCount - 1);

        public override void ActivateSkin(int ind)
        {
            base.ActivateSkin(ind);
            InvokeSkinActivated(ind);
        }
        public int GetSkinInd() => activeSkinInd;
        public void InvokeSkinActivated(int ind) => SkinActivated?.Invoke(ind);
        public void Subscribe(Action<int> action) => SkinActivated += action;
        public void Unsubscribe(Action<int> action) => SkinActivated -= action;
    }
}