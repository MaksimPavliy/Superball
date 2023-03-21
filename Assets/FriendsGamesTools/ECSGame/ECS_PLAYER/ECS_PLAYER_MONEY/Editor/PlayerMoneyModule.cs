using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class PlayerMoneyModule : ECSModule
    {
        public override string parentModule => PlayerModule.define;
        public const string define = "ECS_PLAYER_MONEY";
        public override string Define => define;
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(PlayerModule.define).Adding(UIModule.define);
        public override HowToModule HowTo() => new ECS_PLAYER_MONEY_HowTo();
        protected override string debugViewPath => "ECSGame/ECS_PLAYER/ECS_PLAYER_MONEY/Debug/PlayerMoneyDebugView";
    }
}