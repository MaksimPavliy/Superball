#if UI
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.UI
{
    public class UI : MonoBehaviour
    {
        static UI instance;
        public static void EnsureInited()
        {
            if (instance == null)
                new GameObject("UI").AddComponent<UI>();
        }
        private void Awake() => instance = this;
        private void Update()
        {
            currFrame++;
            UpdateEventSystem();
        }

        static int currFrame;
        const int FramesDelay = 3;
        static int minPressingFrame;
        public static void DelayPressing(int frames = FramesDelay)
        {
            EnsureInited();
            minPressingFrame = currFrame + frames;
        }

        static EventSystem eventSystem;
        static void UpdateEventSystem()
        {
            if (eventSystem == null)
                eventSystem = EventSystem.current;
            eventSystem.enabled = pressEnabled;
        }
        static bool _pressEnabled = true;
        public static bool pressEnabled
        {
            get
            {
                if (minPressingFrame > currFrame)
                    return false;
#if CAMERA
                var cam = CameraMover.instance;
                if (cam != null && cam.movedRecently)
                    return false;
#endif
                return _pressEnabled;
            }
            set {
                if (value == _pressEnabled)
                    return;
                EnsureInited();
                _pressEnabled = value;
            }
        }
    }
}
#endif