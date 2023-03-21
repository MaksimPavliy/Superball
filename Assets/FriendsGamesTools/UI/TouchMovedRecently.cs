#if UI
using UnityEngine;

namespace FriendsGamesTools
{
    public class TouchMovedRecently : MonoBehaviourHasInstance<TouchMovedRecently>
    {
        Vector3 tapStartPos;
        bool inTap;

        [SerializeField] float dragThreshold = 0.03f; // screen width part (or height, whats smaller).
        float dragThresholdPxSqr;
        bool inDraggingTap;
        float draggingTapFinishTime;
        private void Start()
        {
            dragThresholdPxSqr = dragThreshold * Mathf.Min(Screen.width, Screen.height);
            dragThresholdPxSqr *= dragThresholdPxSqr;
        }
        [SerializeField] float delayAfterDragging = 0.2f;
        public bool movedRecently
        {
            get
            {
                var tappedRecently = draggingTapFinishTime + delayAfterDragging > Time.time;
                var movedRecently = inDraggingTap || tappedRecently;
                return movedRecently;
            }
        }
        private void Update()
        {
            var pressed = Input.touchCount > 0 || Input.GetMouseButton(0);
            if (pressed && !inTap)
            {
                // Start tap.
                inTap = true;
                tapStartPos = Input.mousePosition;
            }
            else if (inTap && pressed)
            {
                // Continue tap.
                if (!inDraggingTap)
                {
                    if ((tapStartPos - Input.mousePosition).sqrMagnitude > dragThresholdPxSqr)
                        inDraggingTap = true;
                }
            }
            else if (inTap)
            {
                // End tap.
                if (inDraggingTap)
                    draggingTapFinishTime = Time.time;
                inTap = false;
                inDraggingTap = false;
            }
        }
    }
}
#endif