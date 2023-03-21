#if ECS_LOCATIONS
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Locations
{
    public struct LocationsData : IComponentData
    {
        public int currLocationInd;
    }
}
#endif