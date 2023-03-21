#if DEBUG_PANEL
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DisablableGameObject : MonoBehaviour // Should be after DebugGameObjectDisabler in execution order.
    {
        [SerializeField] string nameOverride;
        public new string name => !string.IsNullOrEmpty(nameOverride) ? nameOverride : transform.name;
        private void Awake() => DebugDisablerToggleView.Add(this);
        private void OnDestroy() => DebugDisablerToggleView.Remove(this);
    }
}
#endif