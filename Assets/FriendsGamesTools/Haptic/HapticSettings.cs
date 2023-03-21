#if HAPTIC
using TapticPlugin;

namespace FriendsGamesTools
{
    public class HapticSettings : SettingsScriptable<HapticSettings>
    {
        public bool availableInEditor = true;
        public bool log = false;
        public HapticType defaultType = HapticType.Medium;
    }
    public enum HapticType { Light, Medium, Heavy }
    public static class HapticTypeUtils
    {
        public static ImpactFeedback ToIOSType(this HapticType type)
        {
            switch (type)
            {
                default:
                case HapticType.Heavy: return ImpactFeedback.Heavy;
                case HapticType.Medium: return ImpactFeedback.Medium;
                case HapticType.Light: return ImpactFeedback.Light;
            }
        }
    } 
}
#endif