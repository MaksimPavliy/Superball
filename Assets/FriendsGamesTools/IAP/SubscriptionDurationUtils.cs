using System;

namespace FriendsGamesTools.IAP
{
#if IAP
    public static partial class SubscriptionDurationUtils
    {
        public static string ToProductIdSuffix(this SubscriptionDuration type) => type.ToString().ToLower();
        public static float ToSeconds(this SubscriptionDuration type)
        {
            const float SecondsInDay = 60 * 60 * 24;
            switch (type)
            {
                default: throw new Exception();
                case SubscriptionDuration.OneWeek: return SecondsInDay * 7;
                case SubscriptionDuration.OneMonth: return SecondsInDay * 30;
                case SubscriptionDuration.ThreeMonths: return SecondsInDay * 30*3;
                case SubscriptionDuration.SixMonths: return SecondsInDay * 30*6;
                case SubscriptionDuration.OneYear: return SecondsInDay * 365;
            }
        }
    }
#endif
}
