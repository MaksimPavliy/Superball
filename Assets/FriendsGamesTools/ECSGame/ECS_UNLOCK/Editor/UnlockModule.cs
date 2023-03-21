namespace FriendsGamesTools.ECSGame
{
    public class UnlockModule : ECSModule
    {
        public const string define = "ECS_UNLOCK";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_UNLOCK_HowTo();
    }
}


