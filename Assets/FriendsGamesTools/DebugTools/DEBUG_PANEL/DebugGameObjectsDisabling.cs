#if DEBUG_PANEL
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugGameObjectsDisabling : MonoBehaviour
    {
        [SerializeField] DebugDisablerToggleView prefab;
        [SerializeField] HorizontalOrVerticalLayoutGroup layout;
        List<DebugDisablerToggleView> shown = new List<DebugDisablerToggleView>();
        private void Update()
        {
            Utils.UpdatePrefabsList(shown, DebugDisablerToggleView.goToDisable.Keys.ToList(),
                prefab, transform, (name, toggleView) => toggleView.Show(name));
            layout.enabled = !layout.enabled;
        }
    }
}
#endif