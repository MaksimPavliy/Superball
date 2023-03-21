using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class PurchasableProductDebugView : PurchasableProductView
    {
        [SerializeField] Toggle enabledToggle;
        [SerializeField] Toggle debugPurchasedToggle;
#if IAP
        protected override void Awake()
        {
            base.Awake();
            InitDebug(enabledToggle, debugPurchasedToggle);
        }
        public override void Show(AbstractProductSettings product)
        {
            base.Show(product);
            ShowDebug(product, enabledToggle, debugPurchasedToggle);
        }
        public static void InitDebug(Toggle enabledToggle, Toggle debugPurchasedToggle)
        {
            if (enabledToggle != null)
                enabledToggle.interactable = false;
            if (debugPurchasedToggle != null)
                debugPurchasedToggle.interactable = false;
        }
        public static void ShowDebug(AbstractProductSettings product, Toggle enabledToggle, Toggle debugPurchasedToggle)
        {
            if (enabledToggle != null)
                enabledToggle.isOn = product.active;
            if (debugPurchasedToggle != null)
                debugPurchasedToggle.isOn = product.debugModePurchased;
        }
#endif
    }
}
