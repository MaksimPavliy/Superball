using FriendsGamesTools.IAP;

namespace Superball
{
    public class NoAdsView : PurchasableProductView
    {
#if IAP
        private void Update()
        {
            this.gameObject.SetActive(!IAPManager.instance.interstitialsRemoved);
        }
#endif
    }
}