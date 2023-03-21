#if ECS_BONUS
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.BonusEvent
{
    public struct BonusEvent : IComponentData
    {
        public enum State {
            Hidden, // Waiting some time.
            PreparingToAppear, // Will appear immediately after this state. Can show animations here.
            Appeared, // Waiting to be activated.
            WatchingAd, // Time is stopped while watching ad.
            Active // Bonus gives its value.
        }

        public State state;
        public float remainingTime;
    }
}
#endif