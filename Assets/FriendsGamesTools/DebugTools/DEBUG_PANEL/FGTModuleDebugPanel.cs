namespace FriendsGamesTools.DebugTools
{
    public abstract class FGTModuleDebugPanel : DebugPanelItemView
    {
        public abstract string module { get; }
        public abstract string tab { get; }
        public override (string tab, string name) whereToShow => (tab, module);
        public override bool showInDebugPanel => DefinesSettings.Exists(module);
        public override float sortPriority => -1;
        public const string CommonTab = "common";
    }
    public abstract class ECSModuleDebugPanel : FGTModuleDebugPanel
    {
        public override float sortPriority => -0.5f;
    }
}