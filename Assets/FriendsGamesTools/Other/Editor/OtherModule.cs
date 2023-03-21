namespace FriendsGamesTools
{
    public class OtherModule : RootModule
    {
        public const string define = "OTHER";
        public override string Define => define;
        public override HowToModule HowTo() => new OtherModule_HowTo();
    }
}