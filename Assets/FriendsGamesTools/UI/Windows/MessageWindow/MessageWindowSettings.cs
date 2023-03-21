using TMPro;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class MessageWindowSettings : SettingsScriptable<MessageWindowSettings>
    {
        public TMP_FontAsset headerFont, messageFont, okFont;
        public Sprite windowPic, buttonPic;
    }
}
