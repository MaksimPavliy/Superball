using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class HardCurrencyModule : ECSModule
    {
        public override string parentModule => PlayerModule.define;
        public const string define = "ECS_HARD_CURRENCY";
        public override string Define => define;
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(PlayerModule.define).Adding(UIModule.define);
        public override HowToModule HowTo() => new HardCurrencyModule_HowTo();
        protected override string debugViewPath => "ECSGame/ECS_PLAYER/ECS_HARD_CURRENCY/Debug/HardCurrencyDebugView";
    }
}