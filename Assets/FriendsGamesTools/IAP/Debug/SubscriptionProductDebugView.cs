using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class SubscriptionProductDebugView : SubscriptionProductView
    {
        [SerializeField] Toggle enabledToggle;
        [SerializeField] Toggle debugPurchasedToggle;
#if IAP
        protected override void Awake()
        {
            base.Awake();
            PurchasableProductDebugView.InitDebug(enabledToggle, debugPurchasedToggle);
        }
        public override void Show(AbstractProductSettings product)
        {
            base.Show(product);
            PurchasableProductDebugView.ShowDebug(product, enabledToggle, debugPurchasedToggle);
        }
#endif
    }
}
