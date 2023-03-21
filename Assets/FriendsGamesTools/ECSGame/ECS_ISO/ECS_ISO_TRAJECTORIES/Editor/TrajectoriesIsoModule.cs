using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class TrajectoriesIsoModule : ECSISOModule
    {
        public const string define = "ECS_ISO_TRAJECTORIES";
        public override string Define => define;
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(TrajectoriesModule.define);
        public override HowToModule HowTo() => new ECS_ISO_TRAJECTORIES_HowTo();
    }
}


