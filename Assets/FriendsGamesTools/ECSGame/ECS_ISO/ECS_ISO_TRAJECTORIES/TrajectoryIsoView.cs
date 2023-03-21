#if ECS_ISO_TRAJECTORIES
using UnityEngine;
namespace FriendsGamesTools.ECSGame.Iso
{
    [ExecuteAlways]
    public class TrajectoryIsoView : TrajectoryView
    {
        public override float CalcTotalLength() => CalcIsoLength();
        public float CalcIsoLength()
        {
            if (pts.Count == 0)
                return 0;
            float length = 0;
            var iso = pts[0].GetIso();
            for (int i = 0; i < pts.Count - 1; i++)
                length += IsoCoos.IsoDist(pts[i].GetIso(), pts[i + 1].GetIso());
            return length;
        }

        public override Vector3 GetGameSpacePos(TrajectoryPt pt) => pt.GetIso();
        public override void SetGameSpacePos(TrajectoryPt pt, Vector3 gameSpacePos) => pt.SetIso(gameSpacePos);
        public override float GetGameSpaceDist(Vector3 gameSpacePos1, Vector3 gameSpacePos2)
            => IsoCoos.IsoDist(gameSpacePos1, gameSpacePos2);
        public override string gameSpaceName => "ISO";
    }
}
#endif