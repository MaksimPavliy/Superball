namespace FriendsGamesTools
{
    public class PluginsModule : RootModule
    {
        public const string define = "PLUGINS";
        public override string Define => define;
        public override bool enabledWithoutDefine => true;
        public override HowToModule HowTo() => new PluginsModule_HowTo();
    }
}