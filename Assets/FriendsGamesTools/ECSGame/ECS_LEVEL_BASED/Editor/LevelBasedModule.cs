using System.Collections.Generic;
using FriendsGamesTools.ECSGame;

namespace FriendsGamesTools
{
    public class LevelBasedModule : ECSModule
    {
        public const string define = "ECS_LEVEL_BASED";
        public override string Define => define;
        public override HowToModule HowTo() => new LevelBasedModule_HowTo();
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(WindowsModule.define).Adding(LocationsModule.define)
            .Adding(PlayerMoneyModule.define).Adding(HapticModule.define);
        public const string RunCodegeneration = "Run codegeneration";
    }
}