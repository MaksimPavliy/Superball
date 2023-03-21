namespace FriendsGamesTools.DebugTools
{
    public class TargetFrameRateDebugView : FGTModuleDebugPanel
    {
        public override string tab => CommonTab;
        public override string module => "EDITOR_TOOLS";
#if EDITOR_TOOLS && DEBUG_PERFORMANCE
        public override void OnDebugPanelAwake() => PerformanceBudgetManager.EnsureExists();
#endif
    }
}
