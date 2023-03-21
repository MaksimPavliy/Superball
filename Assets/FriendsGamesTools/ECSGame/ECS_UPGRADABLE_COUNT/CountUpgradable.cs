#if ECS_UPGRADABLE_COUNT
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public struct CountUpgradable : IComponentData
    {
        public int count;
    }
}
#endif