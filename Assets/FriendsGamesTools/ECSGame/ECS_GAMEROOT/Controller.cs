#if ECS_GAMEROOT
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public abstract class Controller : ComponentSystem
    {
        protected static GameRoot root => GameRoot.instance;
        public static T Get<T>() where T : Controller => root.Get<T>();
        protected static EntityManager manager => World.Active.EntityManager;
        public virtual void InitDefault() { } // Initing new default player data.
        public virtual void OnInited() { } // Afer default created or existing deserialized.
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false; // Update only in gameroot. DisableAutoCreation should have done this for me, but its not inherited.
        }
        protected override void OnUpdate() { }
        public new void Update()
        {
            if (time < 0)
                deltaTime = 0;
            else
                deltaTime = Mathf.Max(0, GameTime.time - time);
            time = GameTime.time;
            OnUpdate();
        }
        public virtual int updateEvery => 1;
        public float deltaTime { get; private set; }
        public float time { get; private set; } = -1;

        public virtual void OnFixedUpdate(float fixedDeltaTime) { }

#if ECS_PLAYER_MONEY
        public double money => root.Get<Player.Money.PlayerMoneyController>().amount;
#endif
    }
}
#endif