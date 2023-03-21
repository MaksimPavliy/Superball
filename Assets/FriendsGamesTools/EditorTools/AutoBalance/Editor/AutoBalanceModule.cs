namespace FriendsGamesTools.EditorTools.AutoBalance
{
    public class AutoBalanceModule : ModuleManager
    {
        public const string define = "AUTO_BALANCE";
        public override string Define => define;
        public override string parentModule => EditorToolsModule.define;
        public override HowToModule HowTo() => new AutoBalanceModule_HowTo();

#if AUTO_BALANCE
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<SimulationSettings>.EnsureExists();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

        }
#endif
    }
}