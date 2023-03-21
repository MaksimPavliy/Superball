using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public abstract class DebugPanelNumberEditor<T> : DebugPanelParamEditor<T>
    {
        [SerializeField] TMP_InputField input;
#if DEBUG_CONFIG
        public override void Show() => input.text = get().ToString();
        public override IDebugPanelParamEditor Init(string name, int indent, Func<T> get, Action<T> set)
        {
            base.Init(name, indent, get, set);
            input.onEndEdit.AddListener(onValChanged);
            return this;
        }
        protected abstract (bool success, T value) TryParse(string text);
        private void onValChanged(string text)
        {
            var (success, value) = TryParse(text);
            if (success)
            {
                set(value);
                Show();
            }
        }
#endif
    }
}
