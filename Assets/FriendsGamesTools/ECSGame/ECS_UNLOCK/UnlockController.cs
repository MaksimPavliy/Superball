#if ECS_UNLOCK
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class UnlockController : Controller
    {
        public bool GetAvailable(Entity entity)
        {
#if ECS_PLAYER_MONEY
            var price = GetPrice(entity);
            if (HasPrice(price) && money < price)
                return false;
#endif
            return entity.IsLocked() && GetAvailablePrivate(entity);
        }
        protected abstract bool GetAvailablePrivate(Entity entity);
        public virtual double GetPrice(Entity entity) => -1;
        public bool HasPrice(Entity e) => HasPrice(GetPrice(e));
        public bool HasPrice(double price) => price >= 0;
        protected virtual void Pay(double price, Entity entity)
        {
#if ECS_PLAYER_MONEY
            Player.Money.PlayerMoneyController.instance.PayMoney(price);
#endif
        }
        public virtual bool Unlock(Entity entity)
        {
            if (!GetAvailable(entity))
                return false;
            var price = GetPrice(entity);
            if (HasPrice(price))
                Pay(price, entity);
            entity.RemoveComponent<Locked>();
            return true;
        }
    }
    public static class LockedEntityUtils
    {
        public static bool IsLocked(this Entity entity) => entity.HasComponent<Locked>();
    }
}
#endif