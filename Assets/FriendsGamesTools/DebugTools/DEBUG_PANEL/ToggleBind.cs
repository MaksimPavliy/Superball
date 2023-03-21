#if DEBUG_PANEL
using System;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class ToggleBind
    {
        Toggle toggle;
        Action<bool> action;
        public ToggleBind(Toggle toggle, Action<bool> action)
        {
            this.toggle = toggle;
            this.action = action;
            action(toggle.isOn);
            toggle.onValueChanged.AddListener(val => action(val));
        }
        public bool isOn => toggle.isOn;
    }
}
#endif