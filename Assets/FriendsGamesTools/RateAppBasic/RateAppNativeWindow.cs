using FriendsGamesTools.UI;

namespace FriendsGamesTools
{
    public class RateAppNativeWindow  : NativeWindow
    {
#if RATE_APP_BASIC
        public new static void Show() => Show("RateAppNativeWindow");
#endif
    }
}
