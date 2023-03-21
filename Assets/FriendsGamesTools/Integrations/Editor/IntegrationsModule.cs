using FriendsGamesTools.Integrations;

namespace FriendsGamesTools
{
    public class IntegrationsModule : ModulesFolder
    {
        public const string define = "SDKs";
        public override string Define => define;
        public override bool canBeEnabled => true;
        public override HowToModule HowTo() => new IntegrationsModule_HowTo();
        protected override void OnNotCompiledGUI()
        {
            base.OnNotCompiledGUI();
            ApplicationIdValidator.UpdateBundleIdValidationOnGUI();
        }
    }
}


