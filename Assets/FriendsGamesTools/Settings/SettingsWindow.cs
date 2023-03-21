using FriendsGamesTools.UI;

namespace FriendsGamesTools
{
    public class SettingsWindow : Window {
#if SETTINGS
        static SettingsModuleSettings settings => SettingsModuleSettings.instance;
        public static void Show() => Windows.Get(settings.window.prefab, true).shown = true;
#endif
    }
}