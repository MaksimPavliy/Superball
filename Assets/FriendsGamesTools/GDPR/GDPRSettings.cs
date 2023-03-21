using System;
using FriendsGamesTools.UI;

namespace FriendsGamesTools
{
    public class GDPRSettings : SettingsScriptable<GDPRSettings>
    {
        public string iOSPrivacyPolicyURL;
        public string iOSTermsOfUseURL;
        public string androidPrivacyPolicyURL;
        public string androidTermsOfUseURL;
        public GDPRWindowPrefabSettings window;
        public bool openMainMenuOnClose = true;
    }
    [Serializable]
    public class GDPRWindowPrefabSettings : WindowPrefabSettings<GDPRWindow>
    {
        public override string title => "GDPR window";
        public override string defaultPath => $"{FriendsGamesManager.MainPluginFolder}/GDPR/GDPRWindow.prefab";
    }
}