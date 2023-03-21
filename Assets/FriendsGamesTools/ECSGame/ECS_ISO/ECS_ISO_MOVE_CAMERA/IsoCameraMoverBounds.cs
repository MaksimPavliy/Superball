#if ECS_ISO_MOVE_CAMERA

using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    public class IsoCameraMoverBounds : CameraMoverBounds
    {
        #region Iso borders
        public IsoRect borders;
        public override void CameraPosRestrictions()
        {
            base.CameraPosRestrictions();

            if (borders != null)
            {
                // Iso pos clamp.
                var sc = halfScreenInWorld;
                BoundPt(cam.transform.position + new Vector3(sc.x, sc.y, 0));
                BoundPt(cam.transform.position + new Vector3(sc.x, -sc.y, 0));
                BoundPt(cam.transform.position + new Vector3(-sc.x, sc.y, 0));
                BoundPt(cam.transform.position + new Vector3(-sc.x, -sc.y, 0));
            }
        }
        void BoundPt(Vector3 worldPos)
        {
            var isoPos = IsoCoos.WorldToIso(worldPos);
            if (!borders.Contains(isoPos))
            {
                var toBorders = borders.ToRect(isoPos);
                var shift = IsoCoos.IsoToWorldDir(toBorders);
                cam.transform.position += shift;
            }
        }
        #endregion
    }
}
#endif