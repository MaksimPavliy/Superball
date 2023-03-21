namespace FriendsGamesTools
{
    public class AudioModule : RootModule
    {
        public const string define = "AUDIO";
        public override string Define => define;
        public override HowToModule HowTo() => new AudioModule_HowTo();
    }
}


