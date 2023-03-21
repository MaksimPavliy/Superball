using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class BonusModule : ECSModule
    {
        public const string define = "ECS_BONUS";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_BONUS_HowTo();
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(GameTimeModule.define);
        protected override string debugViewPath => "ECSGame/ECS_BONUS/Debug/BonusDebugView";
    }
}


