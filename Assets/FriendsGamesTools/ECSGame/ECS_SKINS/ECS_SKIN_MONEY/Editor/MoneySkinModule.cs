using System.Collections.Generic;
using FriendsGamesTools.ECSGame;

namespace FriendsGamesTools
{
    public class MoneySkinsModule : ECSModule
    {
        public const string define = "ECS_SKIN_MONEY";
        public override string Define => define;
        public override HowToModule HowTo() => new MoneySkinModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(SkinsModule.define);
        public override string parentModule => SkinsModule.define;

#if ECS_SKIN_MONEY
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
        }
#endif
    }
}