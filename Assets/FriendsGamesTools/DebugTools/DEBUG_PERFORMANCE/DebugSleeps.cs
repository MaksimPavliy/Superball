#if DEBUG_PERFORMANCE
using System.Threading;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugSleeps : MonoBehaviourHasInstance<DebugSleeps> {
        static int _sleepMS;
        public static int sleepMS {
            get => _sleepMS;
            set
            {
                _sleepMS = value;
                if (value > 0 && instance == null)
                    new GameObject("DebugSleeps").AddComponent<DebugSleeps>();
            }
        }
        void Update()
        {
            if (sleepMS > 0)
                Thread.Sleep(sleepMS);
        }
    }
}
#endif
