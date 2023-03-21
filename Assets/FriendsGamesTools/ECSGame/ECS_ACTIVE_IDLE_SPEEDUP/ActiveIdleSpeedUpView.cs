#if ECS_ACTIVE_IDLE_SPEEDUP
using FriendsGamesTools.ECSGame.Upgradable;
using FriendsGamesTools.UI;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.ActiveIdle
{
    public abstract class ActiveIdleSpeedUpView : MonoBehaviour
    {
        protected abstract Entity e { get; }
        protected abstract ActiveIdleSpeedUpController controller { get; }
        protected virtual void Awake() { }
        [SerializeField] MultiplierButtonView multiplierView;
        protected virtual void Update()
        {
            var e = this.e;
            var active = e.HasSpeedUp();
            const double multiplier = 2;
            var process = active ? e.GetComponentData<SpeedUp>() : new SpeedUp { duration = 1, elapsed = 0, multiplier = 1 };
            multiplierView.UpdateView(active, multiplier, process.elapsed, process.duration);
        }
        public virtual void OnPressed()
        {
#if ECS_UNLOCK
            if (e.IsLocked()) return;
#endif
            controller.Activate(e);
        }
    }
}
#endif