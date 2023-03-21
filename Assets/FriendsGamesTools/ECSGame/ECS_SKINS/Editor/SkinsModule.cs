using System.Collections.Generic;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.Analytics;

namespace FriendsGamesTools
{
    public class SkinsModule : ECSModule
    {
        public const string define = "ECS_SKINS";
        public override string Define => define;
        public override HowToModule HowTo() => new SkinsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(LevelBasedModule.define);

#if ECS_SKINS
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
        }
#endif
    }
}