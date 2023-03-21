using System;

namespace FriendsGamesTools.IAP
{
    [Serializable]
    public class SubscriptionProductSettings : AbstractProductSettings
    {
        public SubscriptionDuration duration = SubscriptionDuration.OneWeek;
        public override string GetProductIdSuffix()
#if IAP
            => $"{productIdSuffix}_{duration.ToProductIdSuffix()}";
#else
            => null;
#endif
    }
}