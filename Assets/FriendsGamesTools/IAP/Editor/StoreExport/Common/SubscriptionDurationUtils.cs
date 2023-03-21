#if IAP
using System;

namespace FriendsGamesTools.IAP
{
    public static partial class SubscriptionDurationUtils
    {
        public static string ToExportString(this SubscriptionDuration type, AppStoreType store)
        {
            switch (store)
            {
                default: throw new Exception();
                case AppStoreType.AppleAppStore: return type.ToAppleAppeAppStore().ToExportString();
                case AppStoreType.GooglePlayMarket: return type.ToGooglePlayMarket().ToExportString();
            }
        }
        public static AppleSubscriptionDuration ToAppleAppeAppStore(this SubscriptionDuration type)
        {
            switch (type)
            {
                default: throw new Exception();
                case SubscriptionDuration.OneWeek: return AppleSubscriptionDuration.SevenDays;
                case SubscriptionDuration.OneMonth: return AppleSubscriptionDuration.OneMonth;
                case SubscriptionDuration.ThreeMonths: return AppleSubscriptionDuration.ThreeMonths;
                case SubscriptionDuration.SixMonths: return AppleSubscriptionDuration.SixMonths;
                case SubscriptionDuration.OneYear: return AppleSubscriptionDuration.OneYear;
            }
        }
        public static GoogleSubscriptionDuration ToGooglePlayMarket(this SubscriptionDuration type)
        {
            switch (type)
            {
                default: throw new Exception();
                case SubscriptionDuration.OneWeek: return GoogleSubscriptionDuration.Weekly;
                case SubscriptionDuration.OneMonth: return GoogleSubscriptionDuration.OneMonth;
                case SubscriptionDuration.ThreeMonths: return GoogleSubscriptionDuration.ThreeMonths;
                case SubscriptionDuration.SixMonths: return GoogleSubscriptionDuration.SixMonths;
                case SubscriptionDuration.OneYear: return GoogleSubscriptionDuration.Annual;
            }
        }
    }
}
#endif