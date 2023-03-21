using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class SubscriptionDebugView : SubscriptionView
    {
        [SerializeField] TextMeshProUGUI debugSubscriptionRemaining;
#if IAP
        protected override void UpdateView()
        {
            base.UpdateView();
            if (debugSubscriptionRemaining != null)
                debugSubscriptionRemaining.text = IAPSettings.instance.debugSubscriptionRemainingOnAppLaunch.ToShownTime();
        }
#endif
    }
}
