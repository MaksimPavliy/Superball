#if ECS_PLAYER
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Player
{
    public struct IsPlayer : IComponentData { public bool ignored; }
    public class PlayerController : Controller
    {
        public static Entity entity => ECSUtils.GetSingleEntity<IsPlayer>();
        public override void InitDefault() => ECSUtils.CreateEntity(new IsPlayer());
    }
}
#endif
