using System;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools
{
    public class SettingsModuleSettings : SettingsScriptable<SettingsModuleSettings> {
        public SettingsWindowPrefabSettings window = new SettingsWindowPrefabSettings();

        // Default graphics.
        public TMP_FontAsset headerFont, captionFont, infoFont;
        public Sprite windowPic, crossPic, onPic, offPic, starsPic;
    }
    [Serializable]
    public class SettingsWindowPrefabSettings : WindowPrefabSettings<SettingsWindow>
    {
        public override string defaultPath => $"{FriendsGamesManager.MainPluginFolder}/Settings/SettingsWindow.prefab";
        public override string title => "Settings window prefab";
    }
}


