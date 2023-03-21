using System;
using System.Collections.Generic;

namespace FriendsGamesTools
{
    [Serializable]
    public class CrossMarketingShowFlag
    {
        public int id = -1;
        public bool show;
    }
    public class CrossMarketingShowSettings : SettingsScriptable<CrossMarketingShowSettings>
    {
        public List<CrossMarketingShowFlag> data;
#if CROSS_MARKETING
        public bool GetShown(int id) => data.Find(d => d.id == id)?.show ?? false;
#endif
    }
}