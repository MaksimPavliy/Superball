using System.Linq;

namespace FriendsGamesTools.IAP
{
#if IAP
    public static class AbstractProductSettingsUtils
    {
        static IAPSettings config => IAPSettings.instance;
        public static ProductType GetProductType(AbstractProductSettings product, IAPSettings config)
        {
            if (config.consumables.Contains(product))
                return ProductType.Consumable;
            else if (config.nonConsumables.Contains(product))
                return ProductType.NonConsumable;
            else if (config.subscription.products.Contains(product))
                return ProductType.Subscription;
            throw new System.Exception();
        }
        public static ProductType GetProductType(this AbstractProductSettings product)
            => GetProductType(product, config);
    }
#endif
}
