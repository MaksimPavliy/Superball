namespace FriendsGamesTools.DebugTools
{
    public class DebugPerformanceLocalSettings : SettingsScriptable<DebugPerformanceLocalSettings>
    {
        protected override bool inRepository => false;
        public double cpuSpeed = -1;
        public static bool CPUSpeedKnown => instance.cpuSpeed > 0;
        public static double CPUSpeed => instance.cpuSpeed;
    }
}
