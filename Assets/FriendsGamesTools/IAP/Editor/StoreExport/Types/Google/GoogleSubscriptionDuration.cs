#if IAP
using System;

namespace FriendsGamesTools.IAP
{
    public enum GoogleSubscriptionDuration { Weekly, OneMonth, ThreeMonths, SixMonths, Annual }
    public static class GoogleSubscriptionDurationUtils
    {
        public static string ToExportString(this GoogleSubscriptionDuration type)
        {
            switch (type)
            {
                default: throw new Exception();
                case GoogleSubscriptionDuration.Weekly: return "Weekly";
                case GoogleSubscriptionDuration.OneMonth: return "1 month";
                case GoogleSubscriptionDuration.ThreeMonths: return "3 months";
                case GoogleSubscriptionDuration.SixMonths: return "6 months";
                case GoogleSubscriptionDuration.Annual: return "Annual";
            }
        }
    }
}
#endif