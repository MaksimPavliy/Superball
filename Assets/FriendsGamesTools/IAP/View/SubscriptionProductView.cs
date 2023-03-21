using TMPro;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class SubscriptionProductView : PurchasableProductView
    {
        [SerializeField] TextMeshProUGUI duration;
#if IAP
        public override void Show(AbstractProductSettings product)
        {
            base.Show(product);

            var subscription = product as SubscriptionProductSettings;
            if (duration != null)
                duration.text = subscription.duration.ToSeconds().ToShownTime();
        }
#endif
    }
}
