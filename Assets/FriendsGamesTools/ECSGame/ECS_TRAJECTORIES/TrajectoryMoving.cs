#if ECS_TRAJECTORIES
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class TrajectoryMoving : Controller
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref TrajectoryMover mover) => {
                var tr = GetTrajectory(mover.trajectoryInd);
                if (tr == null)
                    return;
                if (mover.isMoving && !mover.paused)
                {
                    Debug.Assert(mover.speed > 0, "Speed should be positive");
                    var deltaGamePos = mover.speed * deltaTime;
                    bool isMoving;
                    (mover.ptInd, mover.ptProgress, isMoving)
                        = Move(tr, mover.reverse, mover.ptInd, mover.ptProgress, deltaGamePos);
                    if (!isMoving)
                    {
                        // Trajectory finished.
                        StartMoveNextQueuedTrajectory(e, ref mover);
                    }
                }
            });
        }

        public static void StartMove(ref TrajectoryMover mover, int trajectoryInd, bool reverse = false)
        {
            mover.trajectoryInd = trajectoryInd;
            mover.ptInd = 0;
            mover.ptProgress = 0;
            mover.paused = false;
            mover.reverse = reverse;
        }
        public static void StartMove(Entity e, ref TrajectoryMover mover, params (int trInd, bool reverse)[] trajectories)
        {
            if (trajectories == null || trajectories.Length == 0)
                return;
            e.AddBuffer<QueuedTrajectory>();
            var queue = e.GetBuffer<QueuedTrajectory>();
            foreach (var item in trajectories)
                queue.Add(new QueuedTrajectory { trajectoryInd = item.trInd, reverse = item.reverse });
            StartMoveNextQueuedTrajectory(e, ref mover);
        }
        public static void StartMove(Entity e, ref TrajectoryMover mover, List<QueuedTrajectory> trajectories)
        {
            if (trajectories == null || trajectories.Count == 0)
                return;
            e.AddBuffer<QueuedTrajectory>();
            var queue = e.GetBuffer<QueuedTrajectory>();
            foreach (var item in trajectories)
                queue.Add(item);
            StartMoveNextQueuedTrajectory(e, ref mover);
        }
        public static void StartMove(Entity e, ref TrajectoryMover mover, params int[] trajectoryInds)
            => StartMove(e, ref mover, trajectoryInds.ConvertAll(ind => (ind, false)).ToArray());
        private static bool StartMoveNextQueuedTrajectory(Entity e, ref TrajectoryMover mover)
        {
            if (!e.HasBuffer<QueuedTrajectory>())
                return false;
            var queue = e.GetBuffer<QueuedTrajectory>();
            if (queue.Length == 0)
                return false;
            var next = queue[0];
            queue.RemoveAt(0);
            if (queue.Length == 0)
                e.RemoveBuffer<QueuedTrajectory>();
            StartMove(ref mover, next.trajectoryInd, next.reverse);
            return true;
        }
        public static TrajectoryView GetTrajectory(int ind)
            => TrajectoriesView.instance.trajectories.IndIsValid(ind) ? TrajectoriesView.instance.trajectories[ind] : null;
        public static float CalcCoveredDist(TrajectoryMover mover)
        {
            var tr = GetTrajectory(mover.trajectoryInd);
            if (tr == null) return 0;
            if (mover.ptInd == tr.pts.Count - 1)
                return tr.CalcTotalLength();
            float dist = 0;
            for (int i = 1; i <= mover.ptInd; i++)
            {
                var prev = tr.GetPt(i - 1, mover.reverse);
                var next = tr.GetPt(i, mover.reverse);
                dist += tr.GetGameSpaceDist(next, prev);
            }
            var prev1 = tr.GetPt(mover.ptInd, mover.reverse);
            var next1 = tr.GetPt(mover.ptInd + 1, mover.reverse);
            dist += tr.GetGameSpaceDist(next1, prev1) * mover.ptProgress;
            return dist;
        }
        public static (TrajectoryPt prev, TrajectoryPt next) GetPrevNextPt(TrajectoryView tr, int ptInd, bool reverse)
        => tr == null ? (null, null) : (tr.GetPt(ptInd, reverse), (ptInd < tr.pts.Count - 1 ? tr.GetPt(ptInd + 1, reverse) : null));
        public static (TrajectoryPt prev, TrajectoryPt next) GetPrevNextPt(TrajectoryMover mover)
            => GetPrevNextPt(GetTrajectory(mover.trajectoryInd), mover.ptInd, mover.reverse);
        public static TrajectoryPt GetClosestPt(TrajectoryMover m)
        {
            var (prev, next) = GetPrevNextPt(m);
            return (prev == null || m.ptProgress > 0.5f) ? next : prev;
        }
        public static Vector3 GetWorldPos(TrajectoryView tr, int ptInd, float ptProgress, bool reverse)
        {
            Debug.Assert(tr != null, "trajectory should exist");
            var (prev1, next1) = GetPrevNextPt(tr, ptInd, reverse);
            if (next1 != null)
                return Vector3.Lerp(prev1.pos, next1.pos, ptProgress);
            else
                return prev1.pos;
        }
        public static Vector3 GetDir(TrajectoryMover mover)
        {
            var tr = GetTrajectory(mover.trajectoryInd);
            if (tr == null)
                return Vector3.forward;
            var curr = tr.GetPt(mover.ptInd, mover.reverse);
            if (mover.ptInd + 1 < tr.pts.Count)
            {
                var next = tr.GetPt(Mathf.Min(tr.pts.Count - 1, mover.ptInd + 1), mover.reverse);
                var nextDir = tr.GetGameSpacePos(next) - tr.GetGameSpacePos(curr);
                return nextDir;
            }
            else
            {
                var prev = tr.GetPt(Mathf.Max(0, mover.ptInd - 1), mover.reverse);
                var prevDir = tr.GetGameSpacePos(curr) - tr.GetGameSpacePos(prev);
                return prevDir;
            }
        }
        public static Vector3 GetSmoothDir(TrajectoryMover mover)
        {
            var tr = GetTrajectory(mover.trajectoryInd);
            if (tr == null)
                return Vector3.forward;
            var prev = tr.GetPt(Mathf.Max(0, mover.ptInd - 1), mover.reverse);
            var curr = tr.GetPt(mover.ptInd, mover.reverse);
            var next = tr.GetPt(Mathf.Min(tr.pts.Count - 1, mover.ptInd + 1), mover.reverse);
            var prevDir = tr.GetGameSpacePos(curr) - tr.GetGameSpacePos(prev);
            var nextDir = tr.GetGameSpacePos(next) - tr.GetGameSpacePos(curr);
            if (prev == curr)
                return nextDir;
            else if (curr == next)
                return prevDir;
            else
                return Vector3.Lerp(prevDir, nextDir, mover.ptProgress);
        }
        public static (int ptIndMoved, float ptProgressMoved, bool movingInProgress)
            Move(TrajectoryView tr, bool reverse, int ptInd, float ptProgress, float deltaGameSpaceDist)
        {
            bool movingInProgress = true;
            do
            {
                var (prev, next) = GetPrevNextPt(tr, ptInd, reverse);
                var currSpeedMul = (prev.speedMul + next.speedMul) * 0.5f;
                var ptDist = tr.GetGameSpaceDist(prev, next);
                var currProgressInc = deltaGameSpaceDist * currSpeedMul / ptDist;
                if (!currProgressInc.IsSane())
                    currProgressInc = 1; // if dist is 0, move instantly.
                ptProgress += currProgressInc;
                if (ptProgress > 1)
                {
                    var extraProgress = ptProgress - 1;
                    var extraDeltaIso = extraProgress * ptDist / currSpeedMul;
                    deltaGameSpaceDist = extraDeltaIso;
                    ptInd++;
                    ptProgress = 0;
                    if (ptInd >= tr.pts.Count - 1)
                    {
                        movingInProgress = false;
                        break;
                    }
                }
                else
                    break;
            } while (true);
            return (ptInd, ptProgress, movingInProgress);
        }
        public static TrajectoryMover GetMover(TrajectoryView tr, float dist)
        {
            var mover = new TrajectoryMover { trajectoryInd = tr.ind };
            (mover.ptInd, mover.ptProgress, _) = Move(tr, false, 0, 0, dist);
            return mover;
        }
        public static Vector3 GetWorldPos(TrajectoryMover mover)
        {
            var tr = GetTrajectory(mover.trajectoryInd);
            if (tr == null)
                Debug.LogError($"trajectory {mover.trajectoryInd} should exist");
            return GetWorldPos(tr, mover.ptInd, mover.ptProgress, mover.reverse);
        }
        public static bool SetViewPosition(Entity e, Transform tr)
        {
            if (!e.HasComponent<TrajectoryMover>())
                return false;
            var mover = e.GetComponentData<TrajectoryMover>();
            if (mover.trajectoryInd == -1)
                return false;
            var pos = GetWorldPos(mover);
            if (!pos.IsSane())
                Debug.Log("hi");
            tr.position = pos;
            return true;
        }
    }
}
#endif