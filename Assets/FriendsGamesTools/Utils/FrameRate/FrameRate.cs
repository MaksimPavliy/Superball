using UnityEngine;

namespace FriendsGamesTools
{
    public static class FrameRate
    {
        public const int HighFPS = 60;
        public const int LowFPS = 30;
        static FrameRateSettings config => FrameRateSettings.instance;
        public static void Init() {
            if (!config.defaultFPSEnabled) return;
            var fps = new IntInPrefs("defaultFPSSet", config.defaultFPS);
            Application.targetFrameRate = fps;
        }
    }
}
