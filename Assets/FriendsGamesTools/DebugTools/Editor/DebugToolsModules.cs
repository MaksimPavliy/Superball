using System.Collections.Generic;

namespace FriendsGamesTools.DebugTools
{
    public class DebugToolsModuleFolder : ModulesFolder
    {
        public const string define = "DEBUG_TOOLS";
        public override string Define => define;
        public override bool canBeEnabled => true;
        public override HowToModule HowTo() => new DEBUG_TOOLS_HowTo();
    }
    public abstract class DebugToolsModule : ModuleManager {
        public override List<string> dependFromPackages 
            => base.dependFromPackages.Adding("com.unity.textmeshpro");
        public override string parentModule => DebugToolsModuleFolder.define;
    }
}


