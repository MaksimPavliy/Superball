using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestsModule : ECSModule
    {
        public const string define = "QUESTS";
        public override string Define => define;
        public override HowToModule HowTo() => new QuestsModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules
            .Adding(Analytics.AnalyticsModule.define)
            .Adding(WindowsModule.define)
            .Adding(PlayerMoneyModule.define);

#if QUESTS
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
        }
#endif
    }
}