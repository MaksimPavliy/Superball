#if DEBUG_PERFORMANCE
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class CPUTimeMeasuringFirstMonobehaviour : MonoBehaviour
    {
        private void Update() => PerformanceBudgetManager.instance.OnFirstScriptStartedFrame();
    }
}
#endif