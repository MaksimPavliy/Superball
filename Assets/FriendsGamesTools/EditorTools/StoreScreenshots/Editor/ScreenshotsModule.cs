using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.Screenshots
{
    public class ScreenshotsModule : ModuleManager
    {
        public const string define = "SCREENSHOTS";
        public override string Define => define;
        public override string parentModule => EditorToolsModule.define;
        public override List<string> dependFromModules => base.dependFromModules.Adding(UIModule.define);
        public override HowToModule HowTo() => new ScreenshotsModule_HowTo();
#if SCREENSHOTS
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            Screenshots.OnEnable();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            Screenshots.OnGUI();
        }
        public override void Update()
        {
            if (Application.isPlaying)
                Screenshots.Update();
            base.Update();
        }
#endif
    }
}