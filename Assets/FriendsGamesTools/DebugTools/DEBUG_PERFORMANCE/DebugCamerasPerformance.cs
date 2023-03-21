using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugCamerasPerformance : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI status;
        [SerializeField] TMP_Dropdown camerasSelector;
        [SerializeField] Toggle selectedOn;
#if DEBUG_PERFORMANCE
        List<Camera> cams;
        void UpdateView()
        {
            var prevInd = camerasSelector.value;
            cams = Utils.FindSceneObjectsWithInactive<Camera>();
            camerasSelector.ClearOptions();
            camerasSelector.AddOptions(cams.ConvertAll(c => $"{c.name} {(c.isActiveAndEnabled ? "on" : "off")}"));
            if (!cams.IndIsValid(prevInd))
                prevInd = 0;
            camerasSelector.value = prevInd;
            status.text = $"{cams.Count} cameras, {cams.Count(c => c.isActiveAndEnabled)} enabled";
        }
        private void Awake()
        {
            camerasSelector.onValueChanged.AddListener(OnCameraIndChanged);
            selectedOn.onValueChanged.AddListener(OnEnabledChanged);
        }
        private void OnEnable() => UpdateView();
        bool programChange;
        private void OnCameraIndChanged(int ind)
        {
            if (programChange)
                return;
            programChange = true;
            if (!cams.IndIsValid(camerasSelector.value))
                return;
            var cam = cams[camerasSelector.value];
            selectedOn.isOn = cam.isActiveAndEnabled;
            UpdateView();
            programChange = false;
        }
        private void OnEnabledChanged(bool enabled)
        {
            if (programChange)
                return;
            programChange = true;
            if (!cams.IndIsValid(camerasSelector.value))
                return;
            var cam = cams[camerasSelector.value];
            cam.enabled = enabled;
            cam.gameObject.SetActive(enabled);
            UpdateView();
            programChange = false;
        }
#endif
    }
}
