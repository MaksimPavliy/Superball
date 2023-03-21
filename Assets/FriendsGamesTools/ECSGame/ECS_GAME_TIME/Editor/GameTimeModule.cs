namespace FriendsGamesTools.ECSGame
{
    public class GameTimeModule : ECSModule
    {
        public const string define = "ECS_GAME_TIME";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_GAME_TIME_HowTo();
        protected override string debugViewPath => "ECSGame/ECS_GAME_TIME/Debug/GameTimeDebugView";
    }
}