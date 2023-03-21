#if DEBUG_TOOLS && ECS_GAMEROOT
using FriendsGamesTools.ECSGame;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugStopTime : MonoBehaviour
    {
        [SerializeField] KeyCode button = KeyCode.Space;
        void Update()
        {
            if (Input.GetKeyUp(button))
                GameTime.SetPause(!GameTime.paused);
        }
    }
}
#endif

