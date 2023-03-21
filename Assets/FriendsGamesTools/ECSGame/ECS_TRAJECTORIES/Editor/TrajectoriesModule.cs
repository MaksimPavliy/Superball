namespace FriendsGamesTools.ECSGame
{
    public class TrajectoriesModule : ECSModule
    {
        public const string define = "ECS_TRAJECTORIES";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_TRAJECTORIES_HowTo();
    }
}


