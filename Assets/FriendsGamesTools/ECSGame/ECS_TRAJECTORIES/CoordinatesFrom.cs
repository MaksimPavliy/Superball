#if ECS_TRAJECTORIES
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [ExecuteAlways]
    public class CoordinatesFrom : MonoBehaviour
    {
        private void Awake()
        {
            if (!Application.isPlaying) return;
            Update();
            Destroy(this);
        }
        public Transform target;
        public static Transform GetOriginal(Transform tr)
        {
            if (tr == null)
                return null;
            var cooFrom = tr.GetComponent<CoordinatesFrom>();
            if (cooFrom == null || cooFrom.target == null)
                return tr;
            return GetOriginal(cooFrom.target);
        }
        private void Update()
        {
            if (target != null)
                transform.position = target.position;
        }
    }
}
#endif