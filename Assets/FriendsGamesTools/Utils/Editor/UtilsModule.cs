namespace FriendsGamesTools
{
    public class UtilsModule : RootModule
    {
        public const string define = "UTILS";
        public override string Define => define;
        public override bool enabledWithoutDefine => true;
        public override HowToModule HowTo() => new UtilsModule_HowTo();

        protected override void OnCompiledGUI() {
            base.OnCompiledGUI();
            FrameRateModuleUI.Show();
        }
    }
}