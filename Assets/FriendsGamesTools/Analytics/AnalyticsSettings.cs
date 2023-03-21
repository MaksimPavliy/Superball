#if ANALYTICS
using System.Collections.Generic;

namespace FriendsGamesTools.Analytics
{
    public class AnalyticsSettings : SettingsScriptable<AnalyticsSettings>
    {
        public List<string> disabledAnalytics = new List<string>();
    }
}
#endif