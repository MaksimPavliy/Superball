#if OTHER
using UnityEngine;

namespace FriendsGamesTools.UI
{
    [ExecuteAlways]
    public class Billboard : MonoBehaviour
    {
        [SerializeField] Camera cam;
        void LateUpdate()
        {
            if (cam == null)
                cam = Camera.main;
            if (cam == null)
                return;
            if (cam.orthographic)
                transform.rotation = cam.transform.rotation;
            else
                transform.LookAt(cam.transform.position);
        }
    }
}
#endif