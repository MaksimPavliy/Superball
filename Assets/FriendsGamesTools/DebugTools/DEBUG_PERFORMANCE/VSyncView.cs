using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class VSyncView : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown view;
#if DEBUG_PERFORMANCE
        private void Awake()
        {
            view.ClearOptions();
            view.AddOptions(new List<string> { "Don't VSync", "VSync 1", "VSync 2", "VSync 3", "VSync 4" });
            view.value = QualitySettings.vSyncCount;
            view.onValueChanged.AddListener(OnChanged);
        }
        private void OnChanged(int val)
        {
            Debug.Log($"change vSyncCount from {QualitySettings.vSyncCount} to {val}");
            QualitySettings.vSyncCount = val;
        }
#endif
    }
}
