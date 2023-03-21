#if ECS_TRAJECTORIES
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public abstract class Queuing<TItem> : Controller
        where TItem : struct, IComponentData
    {
        public override void OnInited() => TrajectoriesNearness.InitNearbyTrajectories();
        protected abstract float itemWidth { get; } // 0.5f
        protected virtual float unpauseDelay => 1;
        protected virtual bool TrajectoryAffected(int trajectoryInd) => true;
        protected virtual bool ItemAffected(Entity e, TItem item) => true;
        protected virtual bool TrajectoryIsNext(int currTrajectoryInd, int nextTrajectoryInd, Entity e, TItem item) => true;
        protected virtual float killDist { get => -1; } // 0.1f
        protected virtual bool killClose => false;
        protected virtual bool KillingAllowed(int trajectoryInd) 
            => !TrajectoriesView.instance.trajectories[trajectoryInd].dontKillClose;
        protected virtual bool KillingAllowed(Entity e) => true;
        public float GetTrajectoryOccupation(int trajectoryInd, bool reverse = false)
        {
            var count = CountOnTrajectory(trajectoryInd, reverse);
            var lengthOccupied = count * itemWidth;
            var length = TrajectoriesView.instance.trajectories[trajectoryInd].CalcTotalLength();
            var occupation = lengthOccupied / length;
            return occupation;
        }
        public int CountOnTrajectory(int trajectoryInd, bool reverse = false)
        {
            int count = 0;
            Entities.ForEach((Entity e, ref TItem item, ref TrajectoryMover mover) =>
            {
                if (mover.trajectoryInd == trajectoryInd && mover.reverse == reverse)
                    count++;
            });
            return count;
        }
        public bool NobodyOnTrajectory(int trajectoryInd, int ignoredEntityInd = -1)
        {
            bool isFree = true;
            Entities.ForEach((Entity e, ref TItem item, ref TrajectoryMover mover) =>
            {
                if (ignoredEntityInd == e.Index)
                    return;
                if (mover.trajectoryInd == trajectoryInd)
                    isFree = false;
            });
            return isFree;
        }
        protected override void OnUpdate()
        {
            Entity entityToDieOutOfOvercrowding = Entity.Null;
            Entities.ForEach((Entity e, ref TItem item, ref TrajectoryMover mover) =>
            {
                if (mover.trajectoryInd == -1 || !TrajectoryAffected(mover.trajectoryInd) || !ItemAffected(e, item))
                    return;
                if (!mover.isMoving)
                    return;
                if (mover.paused)
                {
                    mover.queuePausedRemaining -= deltaTime;
                    if (mover.queuePausedRemaining > 0)
                        return;
                }
                var shouldPause = false;
                var coveredDist1 = -1f;
                var mover1 = mover;
                var item1 = item;
                Entities.ForEach((Entity e1, ref TItem item2, ref TrajectoryMover mover2) =>
                {
                    if (e.Index == e1.Index)
                        return;
                    bool mover2IsOnNextTrajectory = false;
                    if (mover1.trajectoryInd != mover2.trajectoryInd)
                    {
                        if (!TrajectoriesNearness.TrajectoryIsNext(mover1.trajectoryInd, mover1.reverse, mover2.trajectoryInd, mover2.reverse))
                            return;
                        if (!TrajectoryIsNext(mover1.trajectoryInd, mover2.trajectoryInd, e, item1))
                            return;
                        mover2IsOnNextTrajectory = true;
                    }
                    else if (mover1.reverse != mover2.reverse)
                        return;
                    if (mover1.ptInd > mover2.ptInd && !mover2IsOnNextTrajectory)
                        return;
                    if (coveredDist1 < 0)
                        coveredDist1 = TrajectoryMoving.CalcCoveredDist(mover1);
                    var coveredDist2 = TrajectoryMoving.CalcCoveredDist(mover2);
                    if (mover2IsOnNextTrajectory)
                        coveredDist2 += TrajectoriesView.instance.trajectories[mover1.trajectoryInd].CalcTotalLength();
                    if (coveredDist1 > coveredDist2)
                        return;
                    if (coveredDist1 == coveredDist2 && e.Index > e1.Index)
                        return; // When they are at the same spot, somebody should come first.
                    if (coveredDist2 - coveredDist1 < itemWidth)
                        shouldPause = true;
                    if (coveredDist2 - coveredDist1 < killDist && KillingAllowed(mover1.trajectoryInd)
                            && KillingAllowed(e))
                        entityToDieOutOfOvercrowding = e;
                });
                if (shouldPause)
                    mover.queuePausedRemaining = unpauseDelay;
                mover.paused = shouldPause;
            });

            if (entityToDieOutOfOvercrowding != Entity.Null && killClose)
            {
                //Debug.Log("killed smb");
                entityToDieOutOfOvercrowding.RemoveEntity();
            }
        }
    }
}
#endif