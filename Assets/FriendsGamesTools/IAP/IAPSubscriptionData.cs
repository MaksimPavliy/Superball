#if IAP
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class IAPSubscriptionData : IAPProductData
    {
        public override ProductType type => ProductType.NonConsumable;
        float endTime;
        public float remainingTime => endTime - Time.realtimeSinceStartup;
        public override bool active => remainingTime > 0;
        new public SubscriptionProductSettings config => IAPSettings.instance.subscription.products.Find(p => p.productId == productId);
        public IAPSubscriptionData(string productId, string title, string description, decimal price, string currency, float remainingTime)
        : base(productId, title, description, price, currency) {
            endTime = Time.realtimeSinceStartup + remainingTime;
        }
    }
}
#endif