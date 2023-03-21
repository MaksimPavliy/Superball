using FriendsGamesTools.UI;
using System;

namespace FriendsGamesTools
{
    public class RateAppAdvancedSettings : SettingsScriptable<RateAppAdvancedSettings>
    {
        public DoYouLikeOurGameSettings doYouLikeWindow = new DoYouLikeOurGameSettings();
        public MessageToDevelopersSettings messageToDevelopersWindow = new MessageToDevelopersSettings();
        public float startDelay = 5 * 60;
        public float nextDelay = 24 * 60 * 60;
        public int attempts = 3;
    }
    public abstract class RateAppWindowPrefabSettings<T> : WindowPrefabSettings<T> where T : Window
    {
        const string DefaultWindowsFolder = FriendsGamesManager.MainPluginFolder + "/ECSGame/RateAppAdvanced/Windows";
        public override string defaultPath => DefaultWindowsFolder + $"/{prefabName}.prefab";
        public abstract string prefabName { get; }
    }
    [Serializable]
    public class DoYouLikeOurGameSettings : RateAppWindowPrefabSettings<DoYouLikeOurGameWindow>
    {
        public override string title => "do you like our game";
        public override string prefabName => "DoYouLikeOurGameWindow";
    }
    [Serializable]
    public class MessageToDevelopersSettings : RateAppWindowPrefabSettings<MessageToDevelopersWindow>
    {
        public override string title => "message to devs";
        public override string prefabName => "MessageToDevelopersWindow";
    }
}