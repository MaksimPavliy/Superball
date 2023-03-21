namespace FriendsGamesTools.ECSGame.ActiveIdle
{
    public class ActiveIdleSpeeUpModule : ECSModule
    {
        public const string define = "ECS_ACTIVE_IDLE_SPEEDUP";
        public override string Define => define;
        public override HowToModule HowTo() => new ActiveIdleSpeeUp_HowTo();
    }
}