using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class LocationsModule : ECSModule
    {
        public const string define = "ECS_LOCATIONS";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_LOCATIONS_HowTo();
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(GameRootModule.define).Adding(WindowsModule.define);
        protected override string debugViewPath => "ECSGame/ECS_LOCATIONS/Debug/LocationsDebugView";
    }
}