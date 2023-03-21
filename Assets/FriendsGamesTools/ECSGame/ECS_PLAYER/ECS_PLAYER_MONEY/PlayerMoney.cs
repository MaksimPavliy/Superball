#if ECS_PLAYER_MONEY
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public struct PlayerMoney : IComponentData
    {
        public double amount;
        /// <summary>
        /// Per minute.
        /// </summary>
        public double income;
        public float remainingToUpdate;
    }
}
#endif