namespace FriendsGamesTools.ECSGame
{
    public class MoveCameraModule : ECSISOModule
    {
        public const string define = "ECS_ISO_MOVE_CAMERA";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_ISO_MOVE_CAMERA_HowTo();
    }
}


