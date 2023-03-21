using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugCanvasPerformance : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI status;
        [SerializeField] TMP_Dropdown selector;
        [SerializeField] Toggle selectedOn;
#if DEBUG_PERFORMANCE
        List<Canvas> canvases;
        void UpdateView()
        {
            var prevInd = selector.value;
            canvases = Utils.FindSceneObjectsWithInactive<Canvas>().Filter(c => c.isRootCanvas && !c.name.Contains("Template"));
            selector.ClearOptions();
            selector.AddOptions(canvases.ConvertAll(c => $"{c.name} {(c.isActiveAndEnabled ? "on" : "off")}"));
            if (!canvases.IndIsValid(prevInd))
                prevInd = 0;
            selector.value = prevInd;
            status.text = $"{canvases.Count} canvases, {canvases.Count(c => c.isActiveAndEnabled)} enabled";
        }
        private void Awake()
        {
            selector.onValueChanged.AddListener(OnIndChanged);
            selectedOn.onValueChanged.AddListener(OnEnabledChanged);
        }
        private void OnEnable() => UpdateView();
        bool programChange;
        private void OnIndChanged(int ind)
        {
            if (programChange)
                return;
            programChange = true;
            if (!canvases.IndIsValid(selector.value))
                return;
            var can = canvases[selector.value];
            selectedOn.isOn = can.isActiveAndEnabled;
            UpdateView();
            programChange = false;
        }
        private void OnEnabledChanged(bool enabled)
        {
            if (programChange)
                return;
            programChange = true;
            if (!canvases.IndIsValid(selector.value))
                return;
            var cam = canvases[selector.value];
            cam.enabled = enabled;
            cam.gameObject.SetActive(enabled);
            UpdateView();
            programChange = false;
        }
#endif
    }
}
