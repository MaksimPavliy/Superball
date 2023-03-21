using System.Collections.Generic;

namespace FriendsGamesTools
{
    public class CameraModule : RootModule
    {
        public const string define = "CAMERA";
        public override string Define => define;
        public override List<string> dependFromModules => base.dependFromModules.Adding(TouchesModule.define);
        public override HowToModule HowTo() => new CameraModule_HowTo();
    }
}