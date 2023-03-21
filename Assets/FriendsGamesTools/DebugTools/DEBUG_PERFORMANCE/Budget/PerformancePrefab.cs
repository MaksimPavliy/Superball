using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways]
    public class PerformancePrefab : MonoBehaviour
    {
        public string prefabName;
#if DEBUG_PERFORMANCE
        public static Dictionary<string, int> shownCounts = new Dictionary<string, int>();
        public static int GetCount(string prefabName)
        {
            shownCounts.TryGetValue(prefabName, out var count);
            return count;
        }
        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                if (string.IsNullOrEmpty(prefabName))
                {
                    prefabName = transform.name;
                    transform.SetChanged();
                }
            }
            else
                ChangeCount(1);
        }
        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            ChangeCount(-1);
        }
        void ChangeCount(int delta) => shownCounts[prefabName] = GetCount(prefabName) + delta;
#endif
    }
}
