using System;

namespace FriendsGamesTools
{
    public class FrameRateSettings : SettingsScriptable<FrameRateSettings>
    {
        public bool defaultFPSEnabled = true;
        public int defaultFPS = FrameRate.HighFPS;
    }
}
