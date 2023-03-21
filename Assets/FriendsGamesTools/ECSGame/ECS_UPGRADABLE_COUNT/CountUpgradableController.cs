#if ECS_UPGRADABLE_COUNT
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class CountUpgradableController : UpgradableController
    {
        public virtual Entity CreateCountedItem(Entity upgradable) => Entity.Null;
        public abstract int GetMaxCount(Entity upgradable);
        public CountUpgradable GetData(Entity entity) => entity.GetComponentData<CountUpgradable>();
        public int GetCount(Entity entity) => GetData(entity).count;
        public override bool GetAvailable(Entity entity)
        {
            if (entity == Entity.Null) return false;
            return GetAvailable(entity, GetData(entity), GetMaxCount(entity), GetPrice(entity));
        }
        public bool GetAvailable(Entity entity, CountUpgradable upgradable, int maxCount, double price)
        {
            if (entity == Entity.Null) return false;
            if (maxCount != -1 && maxCount <= upgradable.count)
                return false;
#if ECS_UNLOCK
            if (entity.IsLocked()) return false;
#endif
#if ECS_PLAYER_MONEY
            if (HasPrice(price) && money < price)
                return false;
#endif
            return GetAvailablePrivate(entity, upgradable, maxCount);
        }
        protected virtual bool GetAvailablePrivate(Entity entity, CountUpgradable upgradable, int maxCount)
            => true;
        public virtual bool Upgrade(Entity entity)
        {
            if (entity == Entity.Null) return false;
            var upgradable = GetData(entity);
            var price = GetPrice(entity);
            if (!GetAvailable(entity, upgradable, GetMaxCount(entity), price))
                return false;
            upgradable.count++;
            entity.SetComponentData(upgradable);
            CreateCountedItem(entity);
            if (HasPrice(price))
                Pay(price, entity);
            return true;
        }
        public void InitCountedItems(Entity entity, int startCount)
        {
            entity.ModifyComponent((ref CountUpgradable upgradable) => upgradable.count = startCount);
            for (int i = 0; i < startCount; i++)
                CreateCountedItem(entity);
        }
        protected virtual void Pay(double price, Entity entity)
        {
#if ECS_PLAYER_MONEY
            Player.Money.PlayerMoneyController.instance.PayMoney(price);
#endif
        }
    }
}
#endif