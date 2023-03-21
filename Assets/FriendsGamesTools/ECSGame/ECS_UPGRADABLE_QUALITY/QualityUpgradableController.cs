#if ECS_UPGRADABLE_QUALITY
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class QualityUpgradableController : UpgradableController
    {
        public int GetTotalQuality(Entity upgradable)
        {
            var data = GetData(upgradable);
            return data.quality + data.level * GetQualityPerLevel(upgradable);
        }
        public abstract int GetMaxLevel(Entity upgradable);
        public int GetMaxTotalQuality(Entity upgradable) => GetMaxLevel(upgradable) * GetQualityPerLevel(upgradable);
        public virtual int GetQualityPerLevel(Entity upgradable) => 1;
        public QualityUpgradable GetData(Entity entity) => entity.GetComponentData<QualityUpgradable>();
        public int GetLevel(Entity e) => GetData(e).level;
#if ADS
        public virtual bool AdsWhenNoMoney => false;
#else
        public bool AdsWhenNoMoney => false;
#endif
        public bool EnoughMoney(Entity e) => EnoughMoney(GetPrice(e));
        public bool EnoughMoney(double price)
#if ECS_PLAYER_MONEY
            => root.Get<Player.Money.PlayerMoneyController>().amount >= price;
#else
            => true;
#endif
        public override bool GetAvailable(Entity entity)
        {
            if (entity == Entity.Null) return false;
            return GetAvailable(entity, GetData(entity), GetMaxLevel(entity), GetQualityPerLevel(entity), GetPrice(entity));
        }
        public bool GetAvailable(Entity entity, QualityUpgradable upgradable,
            int maxLevel, int qualityPerLevel, double price)
        {
            if (entity == Entity.Null) return false;
            if (maxLevel != -1 && maxLevel <= upgradable.level)
                return false;
#if ECS_UNLOCK
            if (entity.IsLocked()) return false;
#endif
#if ECS_PLAYER_MONEY
            if (HasPrice(price) && !AdsWhenNoMoney && !EnoughMoney(price))
                return false;
#endif
            return GetAvailablePrivate(entity, upgradable, maxLevel, qualityPerLevel);
        }
        protected virtual bool GetAvailablePrivate(Entity entity, QualityUpgradable upgradable,
            int maxLevel, int qualityPerLevel) => true;
        public virtual bool Upgrade(Entity entity)
        {
            if (entity == Entity.Null) return false;
            var upgradable = GetData(entity);
            var qualityPerLevel = GetQualityPerLevel(entity);
            var maxLevel = GetMaxLevel(entity);
            var price = GetPrice(entity);
            if (!GetAvailable(entity, upgradable, maxLevel, qualityPerLevel, price))
                return false;
            upgradable.quality++;
            if (upgradable.quality >= qualityPerLevel)
            {
                upgradable.quality = 0;
                upgradable.level++;
            }
            entity.SetComponentData(upgradable);
            if (HasPrice(price))
                Pay(price, entity);
            return true;
        }
        protected virtual void Pay(double price, Entity entity)
        {
#if ECS_PLAYER_MONEY
            if (!EnoughMoney(price) && AdsWhenNoMoney)
                return;
            Player.Money.PlayerMoneyController.instance.PayMoney(price);
#endif
        }
    }
}
#endif