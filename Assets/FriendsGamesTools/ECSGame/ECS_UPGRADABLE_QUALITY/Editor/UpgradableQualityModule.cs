using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class UpgradableQualityModule : ECSModule
    {
        public const string define = "ECS_UPGRADABLE_QUALITY";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_UPGRADABLE_QUALITY_HowTo();
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(UnlockModule.define).Adding(PlayerMoneyModule.define);
    }
}


