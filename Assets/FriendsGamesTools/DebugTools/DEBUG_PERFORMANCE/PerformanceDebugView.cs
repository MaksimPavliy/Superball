namespace FriendsGamesTools.DebugTools
{
    public class PerformanceDebugView : FGTModuleDebugPanel
    {
        public override string tab => "Performance";
        public override string module => "DEBUG_PERFORMANCE";
        public override bool wholeTab => true;
#if DEBUG_PERFORMANCE
        public override void OnDebugPanelAwake() => PerformanceBudgetManager.EnsureExists();
#endif
    }
}
