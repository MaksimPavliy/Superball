using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelBoolEditor : DebugPanelParamEditor<bool>
    {
        [SerializeField] Toggle toggle;
#if DEBUG_CONFIG
        public override void Show() => toggle.isOn = get();
        public override IDebugPanelParamEditor Init(string name, int indent, Func<bool> get, Action<bool> set)
        {
            base.Init(name, indent, get, set);
            toggle.onValueChanged.AddListener(onValChanged);
            return this;
        }
        private void onValChanged(bool val)
        {
            set(val);
            Show();
        }
#endif
    }
}
