#if ECSGame && ECS_GAMEROOT
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public interface IDurableProcess : IComponentData
    {
        bool paused { get; }
        bool finished { get; }
        float elapsed { get; set; }
        float speed { get; }
#if ECS_GAME_TIME
        bool ticksOffline { get; }
#endif
    }
    public abstract class AbstractDurableProcessSystem<TDurableProcess> : Controller
#if ECS_GAME_TIME
        , IOfflineTimeReceiver
#endif
        where TDurableProcess : struct, IDurableProcess
    {
        protected virtual void OnFinished(Entity e, TDurableProcess p) { }
        protected override void OnUpdate() => Update(deltaTime, false);
        public void Update(float deltaTime, bool offline)
        {
            Entities.ForEach((Entity e, ref TDurableProcess p) =>
            {
#if ECS_GAME_TIME
                if (offline && !p.ticksOffline)
                    return;
#endif
                if (p.paused || p.finished)
                    return;
                DurableProcessUtils.Tick(ref p, deltaTime);
                if (p.finished)
                    OnFinished(e, p);
            });
        }

#if ECS_GAME_TIME
        public void ApplyOfflineTime(float seconds, OfflineType type)
        {
            if (type != OfflineType.FirstLaunch)
                Update(seconds, true);
        }
#endif
    }
}
#endif