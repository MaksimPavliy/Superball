#if DEBUG_TOOLS && ECS_GAMEROOT
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways]
    public class DebugShowDeviceModel : MonoBehaviour
    {
        public string deviceModel;
        private void Update()
        {
            deviceModel = SystemInfo.deviceModel;
        }
    }
}
#endif

