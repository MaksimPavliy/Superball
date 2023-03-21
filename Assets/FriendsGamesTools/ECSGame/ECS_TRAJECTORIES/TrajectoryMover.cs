#if ECS_TRAJECTORIES
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public struct TrajectoryMover : IComponentData
    {
        public int trajectoryInd;
        public int ptInd; // Last passed pt ind.
        public float ptProgress; // Progress of moving to next pt.
        public float speed; // Current speed.

        public bool reverse;
        public bool paused;
        public float queuePausedRemaining;

        public bool finished => trajectoryInd != -1 && ptInd >= TrajectoriesView.instance.trajectories[trajectoryInd].pts.Count - 1;
        public bool isMoving => trajectoryInd != -1 && !finished;
        public static TrajectoryMover Create(float speed) 
            => new TrajectoryMover { trajectoryInd = -1, speed = speed };
        public bool standingInQueue => paused && queuePausedRemaining > 0;
    }

    [InternalBufferCapacity(20)]
    public struct QueuedTrajectory : IBufferElementData
    {
        public int trajectoryInd;
        public bool reverse;

        public QueuedTrajectory(int ind, bool reverse = false)
        {
            trajectoryInd = ind;
            this.reverse = reverse;
        }
    }
}
#endif