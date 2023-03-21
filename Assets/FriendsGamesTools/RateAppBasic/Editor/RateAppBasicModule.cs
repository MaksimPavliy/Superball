namespace FriendsGamesTools
{
    public class RateAppBasicModule : RootModule
    {
        public const string define = "RATE_APP_BASIC";
        public override string Define => define;
        public override HowToModule HowTo() => new RateAppBasicModule_HowTo();
    }
}