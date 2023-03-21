using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class HumanModule : ECSISOModule
    {
        public const string define = "ECS_ISO_HUMAN";
        public override string Define => define;
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(TrajectoriesIsoModule.define);
        public override HowToModule HowTo() => new ECS_ISO_HUMAN_HowTo();
    }
}


