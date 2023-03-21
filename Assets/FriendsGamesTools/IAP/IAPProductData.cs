#if IAP

namespace FriendsGamesTools.IAP
{
    public abstract class IAPProductData
    {
        public string productId { get; private set; }
        public string title { get; private set; }
        public string description { get; private set; }
        public decimal price { get; private set; }
        public string currency { get; private set; }
        public AbstractProductSettings config => IAPSettings.instance.GetProduct(productId);
        public abstract ProductType type { get; }
        public virtual bool active => false;
        public IAPProductData(string productId, string title, string description, decimal price, string currency)
        {
            this.productId = productId;
            this.title = title;
            this.description = description;
            this.price = price;
            this.currency = currency;
        }
    }
}
#endif