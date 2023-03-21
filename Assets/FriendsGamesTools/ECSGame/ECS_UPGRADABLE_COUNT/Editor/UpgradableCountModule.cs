using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class UpgradableCountModule : ECSModule
    {
        public const string define = "ECS_UPGRADABLE_COUNT";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_UPGRADABLE_COUNT_HowTo();
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(UnlockModule.define);
    }
}


