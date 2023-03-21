using System;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public abstract class DebugPanelParamEditor<T> : MonoBehaviour
#if DEBUG_CONFIG
        , IDebugPanelParamEditor
#endif
    {
        [SerializeField] TextMeshProUGUI title;
#if DEBUG_CONFIG
        public abstract void Show();
        protected Func<T> get { get; private set; }
        protected Action<T> set { get; private set; }
        public virtual IDebugPanelParamEditor Init(string name, int indent, Func<T> get, Action<T> set)
        {
            DebugPanelParamEditorLabel.Show(name, indent, title);
            this.get = get;
            this.set = set;
            Show();
            return this;
        }
#endif
    }
#if DEBUG_CONFIG
    public interface IDebugPanelParamEditor { void Show(); }
#endif
}
