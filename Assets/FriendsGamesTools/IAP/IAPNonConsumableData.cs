#if IAP

namespace FriendsGamesTools.IAP
{
    public class IAPNonConsumableData : IAPProductData
    {
        public override ProductType type => ProductType.NonConsumable;
        public bool owned;
        public override bool active => owned;
        new public NonConsumableProductSettings config => IAPSettings.instance.nonConsumables.Find(p => p.productId == productId);
        public IAPNonConsumableData(string productId, string title, string description, decimal price, string currency, bool owned)
        : base(productId, title, description, price, currency)
        {
            this.owned = owned;
        }
    }
}
#endif