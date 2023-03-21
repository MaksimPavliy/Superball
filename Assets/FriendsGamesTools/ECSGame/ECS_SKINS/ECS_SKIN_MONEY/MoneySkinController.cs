#if ECS_SKIN_MONEY || ECS_SKINS
using System;
using System.Collections.Generic;
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player.Money;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public struct MoneySkin : IComponentData { bool _; }
    public abstract class MoneySkinController : SkinsController<MoneySkin>
    {
        public override IReadOnlyList<SkinViewConfig> viewConfigs => MoneySkinsViewConfig.instance.items;
        public int nextSkinPrice => GetSkinPrice(unlockedSkinsCount);
        public abstract int GetSkinPrice(int unlockedSkinsCount);
        PlayerMoneyController moneyController => root.Get<PlayerMoneyController>();
        public bool enoughMoney => nextSkinPrice <= moneyController.amount;
        public bool buySkinAvailable => anySkinLocked && enoughMoney;
        public void BuyRandomSkin()
        {
            if (!buySkinAvailable) return;
            moneyController.PayMoney(nextSkinPrice);
            var skinInd = Utils.GetIndsList(skins.Length).Filter(ind => skins[ind].locked).RandomElement();
            UnlockSkin(skinInd);
        }
    }
}
#endif