using FriendsGamesTools.Ads;
using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class IncumeForVideoModule : ECSModule
    {
        public const string define = "ECS_INCOME_FOR_VIDEO";
        public override string Define => define;
        public override HowToModule HowTo() => new IncomeForVideo_HowTo();
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(AdsModule.define).Adding(PlayerMoneyModule.define).Adding(GameTimeModule.define);
    }
}