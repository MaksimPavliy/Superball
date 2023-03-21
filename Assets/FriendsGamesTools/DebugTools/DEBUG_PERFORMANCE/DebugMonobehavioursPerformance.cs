using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugMonobehavioursPerformance : MonoBehaviour
    {
        [SerializeField] Button enableButton, disableButton;
#if DEBUG_PERFORMANCE
        List<MonoBehaviour> disabled = new List<MonoBehaviour>();
        public void DisableAllScripts()
        {
            disabled.Clear();
            var monoBehs = Utils.FindSceneObjectsWithInactive<MonoBehaviour>();
            var debugPanel = transform.GetComponentInParent<DebugPanel>();
            var childBehs = debugPanel.transform.GetComponentsInChildren<Behaviour>(true).ToList();
            monoBehs.RemoveAll(m =>
            {
                if (childBehs.Contains(m) || m is EventSystem || m is PointerInputModule)
                    return true;
                var fullName = m.GetType().FullName;
                if (fullName.StartsWith("UnityEngine") || fullName.StartsWith("TMPro")
                || fullName.StartsWith("FriendsGamesTools.DebugTools") || fullName.StartsWith("Unity.Entities"))
                    return true;
                return false;
            });
            //Debug.Log($"disabled = \n{monoBehs.PrintCollection(toString: m => m.GetType().FullName, separator:"\n")}");
            monoBehs.ForEach(m => m.enabled = false);
            disabled = monoBehs;
            UpdateButtonsView();
        }
        public void EnableAllScripts()
        {
            disabled.ForEach(m => m.enabled = true);
            disabled.Clear();
            UpdateButtonsView();
        }
        void UpdateButtonsView()
        {
            disableButton.gameObject.SetActive(monobehsEnabled);
            enableButton.gameObject.SetActive(!monobehsEnabled);
        }
        bool monobehsEnabled => disabled.Count == 0;
        private void Awake()
        {
            UpdateButtonsView();
            if (enableButton != null)
                enableButton.onClick.AddListener(EnableAllScripts);
            if (disableButton != null)
                disableButton.onClick.AddListener(DisableAllScripts);
        }
#endif
    }
}
