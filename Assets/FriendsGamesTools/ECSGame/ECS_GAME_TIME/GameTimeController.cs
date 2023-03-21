#if ECS_GAME_TIME
using System;
using Unity.Entities;
using Time = FriendsGamesTools.GameTime;

namespace FriendsGamesTools.ECSGame
{
#if ECS_PLAYER_MONEY
    [UpdateAfter(typeof(Player.Money.PlayerMoneyController))]
#endif
    public abstract class GameTimeController : Controller, IOfflineTimeReceiver
    {
        // Time is continous through whole game. online and offline.
        public float totalTime { get; private set; }
        public abstract void ApplyOfflineTime(float seconds, OfflineType type);
        protected Entity e => ECSUtils.GetSingleEntity<GameTimeData>();
        private GameTimeData data => e.GetComponentData<GameTimeData>();
        public float totalOnlineTime => data.totalOnlineTime;
        public float totalOfflineTime => data.totalOfflineTime;
        public override void InitDefault()
        {
            ECSUtils.CreateEntity(new GameTimeData
            {
                lastOnlineTimestamp = -1,
                totalOfflineTime = 0,
                totalOnlineTime = 0
            });
            //UnityEngine.Debug.Log($"data.lastOnlineTimestamp = 0");
        }
        public override void OnInited()
        {
            base.OnInited();
            UpdateTime(true);
            
        }
        protected override void OnUpdate()
        {
            UpdateTime(false);
        }
        protected virtual float minOfflineSeconds => 1;
        void UpdateTime(bool initing)
        {
            var deltaTime = GameTime.deltaTime;
            //UnityEngine.Debug.Log($"UpdateTime({deltaTime})");
            e.ModifyComponent((ref GameTimeData data) =>
            {
                data.totalOnlineTime += deltaTime;
                totalTime = data.totalOfflineTime + data.totalOnlineTime;
                var now = DateTime.UtcNow.Ticks;
                if (data.lastOnlineTimestamp != -1)
                {
                    long ticksDelta = now - data.lastOnlineTimestamp;
                    float secondsDelta = ticksDelta / (float)TimeSpan.TicksPerSecond;
                    secondsDelta -= deltaTime;
                    if (secondsDelta > minOfflineSeconds || initing)
                    {
                        //UnityEngine.Debug.Log($"data.lastOnlineTimestamp = {data.lastOnlineTimestamp}");
                        data.totalOfflineTime += secondsDelta;
                        ApplyOfflineTimePrivate(secondsDelta, initing ? OfflineType.AppClosed : OfflineType.AppOutOfFocus);
                    }
                } else
                    ApplyOfflineTimePrivate(0, OfflineType.FirstLaunch);
                data.lastOnlineTimestamp = now;
            });
        }
        protected void ApplyOfflineTimePrivate(float seconds, OfflineType type)
        {
            GameRoot.instance.controllers.ForEach(c=> {
                if (c is IOfflineTimeReceiver wantsOfflineTime)
                    wantsOfflineTime.ApplyOfflineTime(seconds, type);
            });
        }
    }
    public interface IOfflineTimeReceiver
    {
        void ApplyOfflineTime(float seconds, OfflineType type);
    }
}
#endif