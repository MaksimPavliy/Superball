#if DEBUG_PERFORMANCE
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class CPUTimeMeasuringLastMonobehaviour : MonoBehaviour
    {
        private void LateUpdate() => PerformanceBudgetManager.instance.OnLastScriptFinishedFrame();
    }
}
#endif