#if CAMERA

using UnityEngine;

namespace FriendsGamesTools
{
    [RequireComponent(typeof(CameraMover))]
    public class CameraScreenBounds : MonoBehaviourHasInstance<CameraScreenBounds>
    {
        public float topPadding = 0;
        public float bottomPadding = 0;
        public float leftPadding = 0;
        public float rightPadding = 0;

        Camera _cam;
        Camera cam => _cam ?? (_cam = GetComponent<Camera>());
        protected virtual void OnDrawGizmos() => DrawRestrictions();

        [SerializeField] Color col = Color.green;
        public virtual void DrawRestrictions()
        {
            if (!enabled)
                return;

            var min = new Vector3(leftPadding, bottomPadding, 0);
            var max = new Vector3(cam.pixelWidth - rightPadding, cam.pixelHeight - topPadding, 0);
            min = cam.ScreenToWorldPoint(min);
            max = cam.ScreenToWorldPoint(max);
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), col);
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), col);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, min.y, min.z), col);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z), col);
        }

    }
}
#endif