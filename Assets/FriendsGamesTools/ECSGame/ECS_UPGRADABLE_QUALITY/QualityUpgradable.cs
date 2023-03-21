#if ECS_UPGRADABLE_QUALITY
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public struct QualityUpgradable : IComponentData
    {
        public int quality;
        public int level;
    }
}
#endif