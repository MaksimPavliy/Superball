#if ECS_ISO
using UnityEngine;
namespace FriendsGamesTools.ECSGame.Iso
{
    public static class TrajectoryIsoPtUtils 
    {
        public static Vector3 GetIso(this TrajectoryPt pt) => IsoCoos.WorldToIso(pt.pos);
        public static void SetIso(this TrajectoryPt pt, Vector3 iso) => pt.SetPos(IsoCoos.IsoToWorld(iso));
    }
}
#endif