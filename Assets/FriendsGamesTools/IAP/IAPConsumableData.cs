#if IAP

namespace FriendsGamesTools.IAP
{
    public class IAPConsumableData : IAPProductData
    {
        public override ProductType type => ProductType.Consumable;
        new public ConsumableProductSettings config => IAPSettings.instance.consumables.Find(p => p.productId == productId);
        public override bool active => false;
        public IAPConsumableData(string productId, string title, string description, decimal price, string currency)
        : base(productId, title, description, price, currency)
        {
        }
    }
}
#endif