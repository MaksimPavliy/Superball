namespace FriendsGamesTools.DebugTools
{
    public class DebugPerformanceModule : DebugToolsModule
    {
        public const string define = "DEBUG_PERFORMANCE";
        public override string Define => define;
        public override HowToModule HowTo() => new DEBUG_PERFORMANCE_HowTo();
        protected override string debugViewPath => "DebugTools/DEBUG_PERFORMANCE/PerformanceDebugView";

        // simulated/budget prefabs/cpu items editing and showing. Prefabs automatically found and put into settings here.
#if DEBUG_PERFORMANCE
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<DebugPerformanceSettings>.EnsureExists();
            SettingsInEditor<DebugPerformanceLocalSettings>.EnsureExists();
        }
#endif
    }
}