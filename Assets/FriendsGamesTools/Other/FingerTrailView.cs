#if OTHER
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class FingerTrailView : MonoBehaviour
    {
        public Camera cam;
        [SerializeField] Transform trailParent;
        [SerializeField] TrailRenderer trail;
        [SerializeField] float trailZ = 10;
        public virtual bool isEnabled => Input.GetMouseButton(0);
        bool prevEnabled;
        protected virtual void Update()
        {
            var isEnabled = this.isEnabled;
            if (isEnabled)
            {
                trailParent.transform.position = cam.ScreenToWorldPoint(Input.mousePosition.ZTo(trailZ));
                if (!prevEnabled)
                    trail.Clear();
            }
            trailParent.gameObject.SetActive(isEnabled);
            prevEnabled = isEnabled;
        }
    }
}
#endif