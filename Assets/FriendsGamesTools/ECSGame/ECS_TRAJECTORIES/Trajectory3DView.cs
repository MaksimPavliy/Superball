#if ECS_TRAJECTORIES
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [ExecuteAlways]
    public class Trajectory3DView : TrajectoryView
    {
        public override Vector3 GetGameSpacePos(TrajectoryPt pt) => pt.pos;
        public override void SetGameSpacePos(TrajectoryPt pt, Vector3 gameSpacePos) 
            => pt.SetPos(gameSpacePos);
        public override float GetGameSpaceDist(Vector3 gameSpacePos1, Vector3 gameSpacePos2)
            => (gameSpacePos1 - gameSpacePos2).magnitude;
        public override string gameSpaceName => "3D";
    }
}
#endif