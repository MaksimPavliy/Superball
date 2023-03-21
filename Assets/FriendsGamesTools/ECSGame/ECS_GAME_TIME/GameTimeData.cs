#if ECS_GAME_TIME
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public struct GameTimeData : IComponentData
    {
        public long lastOnlineTimestamp;
        public float totalOnlineTime;
        public float totalOfflineTime;
    }
    public enum OfflineType { AppOutOfFocus, AppClosed, FirstLaunch }
}
#endif