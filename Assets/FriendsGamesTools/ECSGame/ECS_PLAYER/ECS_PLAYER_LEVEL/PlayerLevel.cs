#if ECS_PLAYER_LEVEL
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Player.Level
{
    public struct PlayerLevel : IComponentData
    {
        public int level;
        public double exp;
    }
}
#endif