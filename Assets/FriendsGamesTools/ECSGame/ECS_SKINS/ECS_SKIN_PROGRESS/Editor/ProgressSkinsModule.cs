using System.Collections.Generic;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.Analytics;

namespace FriendsGamesTools
{
    public class ProgressSkinsModule : ECSModule
    {
        public const string define = "ECS_SKIN_PROGRESS";
        public override string Define => define;
        public override HowToModule HowTo() => new ProgressSkinsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(SkinsModule.define);
        public override string parentModule => SkinsModule.define;

#if ECS_SKIN_PROGRESS
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
        }
#endif
    }
}