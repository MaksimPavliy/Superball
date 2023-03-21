using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public abstract class ToggleView : MonoBehaviour
    {
        protected abstract bool value { get; set; }
        public event Action<bool> onValueChanged;
        [SerializeField] Button button;
        [SerializeField] GameObject onParent, offParent;
        protected virtual void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnTogglePressed);
        }
        protected virtual void OnEnable() => UpdateView();
        protected virtual void OnTogglePressed()
        {
            value = !value;
            UpdateView();
            onValueChanged?.Invoke(value);
        }
        void UpdateView()
        {
            onParent.SetActiveSafe(value);
            offParent.SetActiveSafe(!value);
        }
    }
}