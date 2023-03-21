#if IAP
using System;

namespace FriendsGamesTools.IAP
{
    public enum AppleSubscriptionDuration { SevenDays, OneMonth, TwoMonths, ThreeMonths, SixMonths, OneYear }
    public static class AppleSubscriptionDurationUtils
    {
        public static string ToExportString(this AppleSubscriptionDuration type)
        {
            switch (type)
            {
                default: throw new Exception();
                case AppleSubscriptionDuration.SevenDays: return "7 Days";
                case AppleSubscriptionDuration.OneMonth: return "1 Month";
                case AppleSubscriptionDuration.TwoMonths: return "2 Months";
                case AppleSubscriptionDuration.ThreeMonths: return "3 Months";
                case AppleSubscriptionDuration.SixMonths: return "6 Months";
                case AppleSubscriptionDuration.OneYear: return "1 Year";
            }
        }
    }
}
#endif