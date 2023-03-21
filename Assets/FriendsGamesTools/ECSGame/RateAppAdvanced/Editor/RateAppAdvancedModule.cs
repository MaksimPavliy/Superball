using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using System.Collections.Generic;
using System.Text;

namespace FriendsGamesTools
{
    public class RateAppAdvancedModule : ECSModule
    {
        public const string define = "RATE_APP_ADVANCED";
        public override string Define => define;
        public override HowToModule HowTo() => new RateAppAdvancedModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules
            .Adding(RateAppBasicModule.define)
            .Adding(UIModule.define)
            .Adding(WindowsModule.define)
            .Adding(GameTimeModule.define);

#if RATE_APP_ADVANCED
        RateAppAdvancedSettings config => SettingsInEditor<RateAppAdvancedSettings>.instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            var changed = false;
            EditorGUIUtils.FloatField("rate app start delay", ref config.startDelay, ref changed);
            EditorGUIUtils.FloatField("rate app next delay", ref config.nextDelay, ref changed);
            EditorGUIUtils.IntField("rate app attempts", ref config.attempts, ref changed);
            WindowEditorUtils.ShowWindow<DoYouLikeOurGameWindow, DoYouLikeOurGameSettings>(config.doYouLikeWindow, ref changed);
            WindowEditorUtils.ShowWindow<MessageToDevelopersWindow, MessageToDevelopersSettings>(config.messageToDevelopersWindow,  ref changed);
            sb.Clear();
            WindowsValid(sb);
            if (sb.Length > 0)
                EditorGUIUtils.Error(sb.ToString());
            if (changed)
                EditorUtils.SetDirty(config);
        }
        StringBuilder sb = new StringBuilder();
        bool WindowsValid(StringBuilder sb = null)
        {
            var valid = true;
            if (!WindowEditorUtils.WindowValid(config.doYouLikeWindow, sb))
                valid = false;
            if (!WindowEditorUtils.WindowValid(config.messageToDevelopersWindow, sb))
                valid = false;
            return valid;
        }
        public override string DoReleaseChecks()
        {
            sb.Clear();
            WindowsValid(sb);
            return sb.ToString();
        }
#endif
    }
}