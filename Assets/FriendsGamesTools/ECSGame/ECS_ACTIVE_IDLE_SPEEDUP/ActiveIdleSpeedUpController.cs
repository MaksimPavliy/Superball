#if ECS_ACTIVE_IDLE_SPEEDUP
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.ActiveIdle
{
    public struct SpeedUp : IDurableProcess {
        public float multiplier;
        public float elapsed;
        public float duration;

        public bool paused => false;
        public bool finished => elapsed >= duration;
        public float speed => 1;
        public bool ticksOffline => false;
        float IDurableProcess.elapsed { get => elapsed; set => elapsed = value; }
    }
    public static class SpeedUpUtils
    {
        public static bool HasSpeedUp(this Entity e) => e.HasComponent<SpeedUp>();
        public static float GetSpeedUp(this Entity e)
        {
            float speedup = 1;
            if (e.TryGetComponent<SpeedUp>(out var s))
                speedup = s.multiplier;
            return speedup;
        }
    }
    public abstract class ActiveIdleSpeedUpController : AbstractDurableProcessSystem<SpeedUp>
    {
        protected abstract (float multiplier, float duration) GetSpeedUpSettings(Entity e);
        public float GetSpeedUp(Entity e) => e.TryGetComponent<SpeedUp>(out var data) ? data.multiplier : 1;
        public virtual void Activate(Entity e)
        {
            if (e.HasComponent<SpeedUp>())
                OnFinished(e, e.GetComponentData<SpeedUp>());
            var (multiplier, duration) = GetSpeedUpSettings(e);
            var speedup = new SpeedUp { multiplier = multiplier, elapsed = 0, duration = duration };
            e.AddComponent(speedup);
            OnActivated(e, speedup);
        }
        protected override void OnFinished(Entity e, SpeedUp speedup)
        {
            OnDeactivated(e, speedup);
            e.RemoveComponent<SpeedUp>();
        }
        protected virtual void OnActivated(Entity tgtEntity, SpeedUp speedup)
        {
            if (tgtEntity.HasComponent<TrajectoryMover>())
                tgtEntity.ModifyComponent((ref TrajectoryMover m) => m.speed *= speedup.multiplier);
            if (tgtEntity.HasComponent<DurableProcess>())
                tgtEntity.ModifyComponent((ref DurableProcess p) => p.speed *= speedup.multiplier);
        }
        protected virtual void OnDeactivated(Entity tgtEntity, SpeedUp speedup)
        {
            if (tgtEntity.HasComponent<TrajectoryMover>())
                tgtEntity.ModifyComponent((ref TrajectoryMover m) => m.speed /= speedup.multiplier);
            if (tgtEntity.HasComponent<DurableProcess>())
                tgtEntity.ModifyComponent((ref DurableProcess p) => p.speed /= speedup.multiplier);
        }
    }
}
#endif