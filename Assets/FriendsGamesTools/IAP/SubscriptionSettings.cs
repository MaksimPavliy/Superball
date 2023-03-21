using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    [Serializable]
    public class SubscriptionSettings
    {
        public bool exists;
        public List<SubscriptionProductSettings> products = new List<SubscriptionProductSettings>();
        public string title = "", description = "";

        public bool freeTrialExists; // So far free trial period is just smallest subscription duration.
#if IAP
        public SubscriptionDuration GetFreeTrialDuration()
        {
            if (products.Count == 0) return SubscriptionDuration.OneWeek;
            var durationInt = (int)SubscriptionDuration.OneYear;
            products.ForEach(p => durationInt = Mathf.Min(durationInt, (int)p.duration));
            return (SubscriptionDuration)durationInt;
        }
        public SubscriptionDuration gracePeriod => SubscriptionDuration.OneWeek;

        public void OnBeforeExport()
        {
            if (!exists)
                products.Clear();
        }
#endif
    }

}
