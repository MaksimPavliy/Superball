namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class TutorialModule : ECSModule
    {
        public const string define = "TUTORIAL";
        public override string Define => define;
        public override HowToModule HowTo() => new TutorialModule_HowTo();
        protected override string debugViewPath => "ECSGame/ECS_TUTORIAL/Debug/TutorialDebugView";

#if TUTORIAL
        TutorialSettings config => SettingsInEditor<TutorialSettings>.instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            EditorGUIUtils.ToggleMadeFromToolbar("enabled by default in develop", "disabled by default in develop",
                ref config.enabledByDefaultInDevelop, ref changed);
            if (changed)
                EditorUtils.SetDirty(config);
        }
#endif
    }
}