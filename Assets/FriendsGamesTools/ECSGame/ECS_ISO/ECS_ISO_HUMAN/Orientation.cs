using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    public enum Orientation { RT, LB, RB, LT }
#if ECS_ISO
    public static class OrientationUtils
    {
        public static Orientation GetMoveOrientation(TrajectoryMover m, Orientation prevOrientation)
        {
            var (prev, next) = TrajectoryMoving.GetPrevNextPt(m);
            if (next != null)
                return GetMoveOrientation(prev, next);
            else
                return prevOrientation;
        }
        private static Orientation GetMoveOrientation(TrajectoryPt prevPt, TrajectoryPt nextPt)
        {
            var isoDir = nextPt.GetIso() - prevPt.GetIso();
            if (Mathf.Abs(isoDir.y) > Mathf.Abs(isoDir.x))
                return isoDir.y > 0 ? Orientation.LT : Orientation.RB;
            else
                return isoDir.x > 0 ? Orientation.RT : Orientation.LB;
        }
        public static Vector3 GetIsoDir(this Orientation orient)
        {
            switch (orient)
            {
                default:
                case Orientation.RT: return new Vector3(1, 0, 0);
                case Orientation.LB: return new Vector3(-1, 0, 0);
                case Orientation.LT: return new Vector3(0, 1, 0);
                case Orientation.RB: return new Vector3(0, -1, 0);
            }
        }
        public static Vector3 GetWorldDir(this Orientation orient) => IsoCoos.IsoToWorldDir(orient.GetIsoDir());
    }
#endif
}
