#if ECS_PLAYER_MONEY
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public struct IncomeMultiplier : IComponentData
    {
        public double multiplier;
    }
}
#endif