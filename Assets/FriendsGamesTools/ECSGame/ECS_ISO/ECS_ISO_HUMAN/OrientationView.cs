
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    [ExecuteAlways]
    public class OrientationView : MonoBehaviour
    {
        public Orientation orientation;
        [SerializeField] protected float length = 1;
        [SerializeField] protected float width = 10;
        [SerializeField] protected Color color = new Color(1, 0, 1, 1);
#if ECS_ISO
        protected virtual void OnDrawGizmos()
        {
            var isoDir = orientation.GetIsoDir();
            var worldDir = IsoCoos.IsoToWorldDir(isoDir);
            GizmosUtils.DrawLine(transform.position, transform.position + worldDir * length, width, color);
        }
#endif
    }
}
