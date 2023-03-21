using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame
{
    public class ECSISOModuleFolder : ModulesFolder
    {
        public override string parentModule => ECSModuleFolder.define;
        public const string define = "ECS_ISO";
        public override string Define => define;
        public override bool canBeEnabled => true;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(ECSModuleFolder.define);
        public override List<string> dependFromPackages 
            => base.dependFromPackages.Adding("com.unity.entities");
        public override HowToModule HowTo() => new ECS_ISO_HowTo();
    }
    public abstract class ECSISOModule : ModuleManager {
        public override string parentModule => ECSISOModuleFolder.define;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(ECSISOModuleFolder.define);
        public override HowToModule HowTo() => new ECS_ISO_HowTo();
    }
}


