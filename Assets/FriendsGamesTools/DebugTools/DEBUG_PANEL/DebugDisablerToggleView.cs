using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugDisablerToggleView : MonoBehaviour
    {
        [SerializeField] Toggle toggle;
        [SerializeField] TextMeshProUGUI nameLabel;
        [SerializeField] TextMeshProUGUI countLabel;

#if DEBUG_PANEL

        #region Disablable
        public static Dictionary<string, List<GameObject>> goToDisable = new Dictionary<string, List<GameObject>>();
        public static void Add(DisablableGameObject d)
        {
            if (!goToDisable.ContainsKey(d.name))
                goToDisable.Add(d.name, new List<GameObject>());
            goToDisable[d.name].Add(d.gameObject);
        }
        public static void Remove(DisablableGameObject d)
        {
            if (goToDisable.ContainsKey(d.name))
                goToDisable[d.name].Remove(d.gameObject);
        }
        #endregion
        
        new string name;
        private void Awake()
        {
            toggle.onValueChanged.AddListener(SetEnabled);
        }
        public void Show(string name)
        {
            this.name = name;
            nameLabel.text = name;
        }
        void SetEnabled(bool enable)
        {
            if (goToDisable.TryGetValue(name, out var gos))
                gos.ForEach(go => go.SetActive(enable));
        }
        void Update()
        {
            int enabledCount = 0, disabledCount = 0;
            if (goToDisable.TryGetValue(name, out var gos))
            {
                gos.ForEach(go => {
                    if (go.activeSelf)
                        enabledCount++;
                    else
                        disabledCount++;
                });
            }
            countLabel.text = $"{enabledCount + disabledCount} total {enabledCount} enabled {disabledCount} disabled";
        }
#endif
    }
}
