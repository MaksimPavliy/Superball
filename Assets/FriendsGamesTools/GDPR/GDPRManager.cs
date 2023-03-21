#if GDPR
using FriendsGamesTools.Ads;

namespace FriendsGamesTools
{
    public enum GDPRState { NotSet, Accepted, Declined }
    public static class GDPRManager
    {
        public const string prefsKey = "GDPRAccepted";
        public static GDPRState state
        {
            get => PlayerPrefsUtils.GetEnum(prefsKey, GDPRState.NotSet);
            set
            {
                if (state == value) return;
                PlayerPrefsUtils.SetEnum(prefsKey, value);
                if (value == GDPRState.Accepted)
                    OnConscentDecided(true);
                else if (value == GDPRState.Declined)
                    OnConscentDecided(false);
            }
        }

        public static bool conscentRequired
#if ADS
            => AdsManager.adsSourceSDK?.conscentRequired ?? false;
#else
            => true;
#endif
        private static void OnConscentDecided(bool accepted)
        {
#if ADS
            AdsManager.adsSourceSDK.SetUserConscent(accepted);
#endif
        }
    }
}
#endif