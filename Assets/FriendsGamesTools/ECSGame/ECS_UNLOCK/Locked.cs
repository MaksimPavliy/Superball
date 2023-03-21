#if ECS_UNLOCK
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public struct Locked : IComponentData
    {
        public bool ignored;
    }
}
#endif