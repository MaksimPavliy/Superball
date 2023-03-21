using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class RadioButtons : MonoBehaviour
    {
        public List<Toggle> toggles = new List<Toggle>();
        [SerializeField] int startInd = -1;
#if UI
        int _onToggleInd;
        bool programChange;
        public int onToggleInd {
            get => _onToggleInd;
            set {
                InitIfNeeded();
                if (_onToggleInd == value && !initing) return;
                _onToggleInd = value;
                programChange = true;
                toggles.ForEachWithInd((t, ind) => t.isOn = ind == _onToggleInd);
                programChange = false;
            }
        }
        bool inited, initing;
        void InitIfNeeded()
        {
            if (inited) return;
            inited = true;
            initing = true;
            onToggleInd = startInd;
            toggles.ForEachWithInd((t, ind) => t.onValueChanged.AddListener(isOn => OnToggleChanged(ind, isOn)));
            initing = false;
        }
        private void Awake() => InitIfNeeded();
        public event Action<int> onToggleIndChanged;
        private void OnToggleChanged(int ind, bool isOn)
        {
            if (programChange) return;
            if (onToggleInd == ind)
            {
                if (isOn) return;
                onToggleInd = -1;
            }
            else
            {
                if (!isOn) return;
                onToggleInd = ind;
            }
            onToggleIndChanged?.Invoke(onToggleInd);
        }
#endif
    }
}