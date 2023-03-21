#if ECS_ISO
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    [ExecuteAlways]
    public class IsoCoos : MonoBehaviourHasInstance<IsoCoos>
    {
        #region Transforms
        public static Vector3 WorldToIsoDir(Vector3 worldDir, float z = 0)
        {
            var isoXOrt = instance.isoXOrt;
            var isoYOrt = instance.isoYOrt;
            // isoXOrt*iso.x+isoYOrt*iso.y = worldDir.xy
            // 
            // isoXOrt.x*iso.x+isoYOrt.x*iso.y = worldDir.x
            // isoXOrt.y*iso.x+isoYOrt.y*iso.y = worldDir.y
            // 
            // iso.y = (worldDir.y-isoXOrt.y*iso.x)/isoYOrt.y
            // 
            // isoXOrt.x*iso.x+isoYOrt.x*(worldDir.y-isoXOrt.y*iso.x)/isoYOrt.y = worldDir.x
            // iso.x*(isoXOrt.x-isoYOrt.x*isoXOrt.y/isoYOrt.y) + isoYOrt.x*worldDir.y/isoYOrt.y = worldDir.x
            // iso.x = (worldDir.x - isoYOrt.x*worldDir.y/isoYOrt.y)/(isoXOrt.x-isoYOrt.x*isoXOrt.y/isoYOrt.y)
            // iso.x = (worldDir.x*isoYOrt.y - isoYOrt.x*worldDir.y)/(isoXOrt.x*isoYOrt.y-isoYOrt.x*isoXOrt.y)
            var iso = Vector3.zero;
            iso.x = (worldDir.x * isoYOrt.y - isoYOrt.x * worldDir.y)
                / (isoXOrt.x * isoYOrt.y - isoYOrt.x * isoXOrt.y);
            iso.y = (worldDir.y - isoXOrt.y * iso.x) / isoYOrt.y;
            iso.z = z;
            return iso;
        }
            
        public static Vector3 WorldToIso(Vector3 world)
            => WorldToIsoDir(world - instance.transform.position, world.z);
        public static Vector3 IsoToWorldDir(Vector3 isoDir)
            => (isoDir.x * instance.isoXOrt + isoDir.y * instance.isoYOrt).ZTo(0);
        public static Vector3 IsoToWorld(Vector3 iso)
            => (IsoToWorldDir(iso) + instance.transform.position).ZTo(iso.z);
        public static float IsoDist(Vector3 iso1, Vector3 iso2) => Vector2.Distance(iso1, iso2);
        public static float IsoDistSqr(Vector3 iso1, Vector3 iso2) => (iso1 - iso2).ZTo0().sqrMagnitude;
        #endregion

        #region Orts
        [SerializeField] Transform xOrtTransform;
        [SerializeField] Transform yOrtTransform;
        public bool set => xOrtTransform != null && yOrtTransform != null;
        Vector3 isoXOrt, isoYOrt, isoZOrt;
        public Vector3 up => isoZOrt;
        //float isoXOrtLength, isoYOrtLength;
        //float isoXOrtLengthSqr, isoYOrtLengthSqr;
        float treeDRightSign;
        void UpdateIsoOrts()
        {
            if (!set)
                return;
            isoXOrt = xOrtTransform.position - transform.position;
            isoYOrt = yOrtTransform.position - transform.position;
            //isoXOrtLengthSqr = isoXOrt.ZTo(0).sqrMagnitude;
            //isoYOrtLengthSqr = isoYOrt.ZTo(0).sqrMagnitude;
            //isoXOrtLength = Mathf.Sqrt(isoXOrtLengthSqr);
            //isoYOrtLength = Mathf.Sqrt(isoYOrtLengthSqr);
            if (treeDPos != null)
                treeDRightSign = Mathf.Sign(Vector3.Dot(treeDPos.right, instance.isoYOrt));
        }
        void DrawOrts()
        {
            if (!set)
                return;
            Debug.DrawLine(transform.position, xOrtTransform.position, Color.red);
            Debug.DrawLine(transform.position, yOrtTransform.position, Color.green);
        }
        public Transform treeDPos;
        public static Quaternion LookAt3DRotation(Vector2 isoDir)
        {
            
            var forward = instance.treeDPos.forward * isoDir.x + instance.treeDRightSign * instance.treeDPos.right * isoDir.y;
            var up = instance.treeDPos.up;
            return Quaternion.LookRotation(forward, up);
        }
        #endregion

        #region Common
        protected override void Awake()
        {
            base.Awake();
            UpdateIsoOrts();
        }
        private void OnEnable()
        {
            if (!Application.isPlaying)
                Awake();
        }
        private void OnDrawGizmosSelected()
        {
            DrawOrts();
        }
        private void OnDrawGizmos()
        {
        }
        private void Update()
        {
            if (!Application.isPlaying)
                UpdateIsoOrts();
        }
        #endregion
    }
}
#endif