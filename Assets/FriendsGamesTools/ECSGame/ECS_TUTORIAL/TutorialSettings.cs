
using FriendsGamesTools.EditorTools.BuildModes;
using System;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class TutorialSettings : SettingsScriptable<TutorialSettings>
    {
        public bool enabledByDefaultInDevelop = true;

#if TUTORIAL
        [NonSerialized] bool _enabled;
        [NonSerialized] bool _enabledInited;
        public bool enabled
        {
            get
            {
                if (!_enabledInited)
                {
                    _enabledInited = true;
                    _enabled = BuildModeSettings.develop ? enabledByDefaultInDevelop : true;
                }
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
#endif
    }
}
