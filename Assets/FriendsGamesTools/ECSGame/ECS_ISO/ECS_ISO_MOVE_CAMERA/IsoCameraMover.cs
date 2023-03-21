#if ECS_ISO_MOVE_CAMERA

namespace FriendsGamesTools.ECSGame.Iso
{
    public abstract class IsoCameraMover<TSelf> : CameraMover<TSelf>
        where TSelf : IsoCameraMover<TSelf>
    {
        
    }
}
#endif