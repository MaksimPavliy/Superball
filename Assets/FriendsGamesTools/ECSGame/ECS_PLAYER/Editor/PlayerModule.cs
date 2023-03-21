namespace FriendsGamesTools.ECSGame
{
    public class PlayerModule : ECSModule
    {
        public const string define = "ECS_PLAYER";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_PLAYER_HowTo();
    }
}


