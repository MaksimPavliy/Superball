#if ECS_ISO
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    [ExecuteAlways]
    public class IsoRect : MonoBehaviour
    {
        public Transform corner1;
        public Transform corner2;
        public (Vector3 isoMin, Vector3 isoMax) GetCorners()
        {
            var iso1 = IsoCoos.WorldToIso(corner1.position);
            var iso2 = IsoCoos.WorldToIso(corner2.position);
            var isoMin = new Vector3(Mathf.Min(iso1.x, iso2.x), Mathf.Min(iso1.y, iso2.y), Mathf.Min(iso1.z, iso2.z));
            var isoMax = new Vector3(Mathf.Max(iso1.x, iso2.x), Mathf.Max(iso1.y, iso2.y), Mathf.Max(iso1.z, iso2.z));
            return (isoMin, isoMax);
        }

        public bool draw;
        public Color color = Color.red;
        public Vector3 shownShift;
        private void Update()
        {
            if (!draw || Utils.IsPrefabOpened())
                return;
            var (isoMin, isoMax) = GetCorners();
            var isoMid = Vector3.Lerp(isoMin, isoMax, 0.5f);
            Debug.DrawLine(IsoCoos.IsoToWorld(isoMin), IsoCoos.IsoToWorld(new Vector3(isoMin.x, isoMax.y, isoMid.z)), color);
            Debug.DrawLine(IsoCoos.IsoToWorld(isoMin), IsoCoos.IsoToWorld(new Vector3(isoMax.x, isoMin.y, isoMid.z)), color);
            Debug.DrawLine(IsoCoos.IsoToWorld(isoMax), IsoCoos.IsoToWorld(new Vector3(isoMin.x, isoMax.y, isoMid.z)), color);
            Debug.DrawLine(IsoCoos.IsoToWorld(isoMax), IsoCoos.IsoToWorld(new Vector3(isoMax.x, isoMin.y, isoMid.z)), color);
        }

        public bool Contains(Vector3 iso)
        {
            var (isoMin, isoMax) = GetCorners();
            return isoMin.x < iso.x && iso.x < isoMax.x && isoMin.y < iso.y && iso.y < isoMax.y;
        }

        public Vector3 ToRect(Vector3 iso)
        {
            var (isoMin, isoMax) = GetCorners();
            Vector3 toRect = Vector3.zero;
            if (isoMin.x > iso.x)
                toRect.x = isoMin.x - iso.x;
            if (iso.x > isoMax.x)
                toRect.x = isoMax.x - iso.x;
            if (isoMin.y > iso.y)
                toRect.y = isoMin.y - iso.y;
            if (iso.y > isoMax.y)
                toRect.y = isoMax.y - iso.y;
            return toRect;
        }
    }
}
#endif