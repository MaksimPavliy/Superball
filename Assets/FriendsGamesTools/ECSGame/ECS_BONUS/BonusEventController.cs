#if ECS_BONUS
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.BonusEvent
{
    public enum BonusEventTag { RT, Fly }
    public abstract class BonusEventController : Controller
    {
        public virtual BonusEventTag tag => BonusEventTag.RT;
        public BonusEvent.State state => data.state;
        public float currStateRemainingDuration => data.remainingTime;
        public abstract Entity e { get; }
        protected abstract void AddTBonusComponent(Entity e);
        public abstract float appearInterval { get; }
        public virtual float preparingToAppearDuration => 0.1f;
        public abstract float activationWaiting { get; }
        public abstract float activatedDuration { get; }
        protected void SetState(BonusEvent.State state, float overridenDuration = -1)
        {
            var prevState = this.state;
            e.ModifyComponent((ref BonusEvent data) =>
            {
                var prevTimeStopped = data.state == BonusEvent.State.WatchingAd;
                var currTimeStopped = state == BonusEvent.State.WatchingAd;
                if (prevTimeStopped != currTimeStopped)
                    GameTime.SetPause(currTimeStopped);
                data.remainingTime = overridenDuration > 0 ? overridenDuration : GetStateDuration(state);
                data.state = state;
            });
            if (prevState != state)
                OnStateChanged(state, prevState);
        }
        protected virtual void OnStateChanged(BonusEvent.State currState, BonusEvent.State prevState) { }
        public float GetStateDuration(BonusEvent.State state)
        {
            switch (state)
            {
                default:
                case BonusEvent.State.Hidden: return appearInterval;
                case BonusEvent.State.PreparingToAppear: return preparingToAppearDuration;
                case BonusEvent.State.Appeared: return activationWaiting;
                case BonusEvent.State.WatchingAd: return float.MaxValue;
                case BonusEvent.State.Active: return activatedDuration;
            }
        }
        public BonusEvent data => e.GetComponentData<BonusEvent>();
        public float GetStateProgress()
        {
            var data = this.data;
            return Mathf.Clamp01(1 - data.remainingTime / GetStateDuration(data.state));
        }
        public float GetStateElapsed()
        {
            var data = this.data;
            return GetStateDuration(data.state) - data.remainingTime;
        }
        protected virtual void OnUpdate(bool active) { }
        protected virtual void OnActivated(bool multiplied) { }
        void UpdateState() => OnUpdate(state == BonusEvent.State.Active);
        protected virtual float startDelay => GetStateDuration(BonusEvent.State.Hidden) * Utils.Random(0.2f, 1f);
        public override void InitDefault()
        {
            AddTBonusComponent(ECSUtils.CreateEntity(new BonusEvent { state = BonusEvent.State.Hidden, remainingTime = startDelay }));
        }

        public override void OnInited() => UpdateState();
        protected abstract void TickRemainingTime();
        public virtual bool canAppear => true;
        protected override void OnUpdate()
        {
            TickRemainingTime();
            var data = this.data;
            if (data.remainingTime > 0)
                return;
            switch (data.state)
            {
                case BonusEvent.State.Hidden:
                    if (!canAppear)
                        return;
                    if (IsSimultaneousBonusEventsCountMax())
                        return;
                    SetState(BonusEvent.State.PreparingToAppear);
                    break;
                case BonusEvent.State.PreparingToAppear:
                    SetState(BonusEvent.State.Appeared);
                    break;
                case BonusEvent.State.Appeared:
                case BonusEvent.State.Active:
                    SetState(BonusEvent.State.Hidden);
                    break;
            }
            UpdateState();
        }
        public bool available => state == BonusEvent.State.Appeared;
        public virtual bool Activate(bool multiplied)
        {
            var data = this.data;
            //var canActivate = data.state == BonusEvent.State.Appeared;
            //Debug.Assert(canActivate, "Activate only when appeared");
            //if (!canActivate)
            //    return;
            SetState(BonusEvent.State.Active);
            UpdateState();
            OnActivated(multiplied);
            return true;
        }
        public virtual void StartWatchingAd()
        {
            SetState(BonusEvent.State.WatchingAd);
            UpdateState();
        }
        public virtual void OnAdFailed()
        {
            SetState(BonusEvent.State.Hidden);
            UpdateState();
        }

        public virtual int maxSimultaneousBonusEvents => -1;
        bool IsSimultaneousBonusEventsCountMax()
        {
            var maxCount = maxSimultaneousBonusEvents;
            if (maxCount == -1)
                return false;
            var currCount = 0;
            GameRoot.instance.controllers.ForEach(c=> {
                var controller = c as BonusEventController;
                if (controller == null) return;
                if (controller.tag == tag && controller.state != BonusEvent.State.Hidden)
                    currCount++;
            });
            return currCount >= maxCount;
        }

        public void DebugAppear() => SetState(BonusEvent.State.PreparingToAppear);
    }
    public abstract class BonusEventController<TBonus> : BonusEventController
        where TBonus : struct, IComponentData
    {
        protected virtual string nameInAnalytics => typeof(TBonus).Name;
        public override Entity e => ECSUtils.GetSingleEntity<TBonus>();
        protected override void AddTBonusComponent(Entity e) => e.AddComponent(new TBonus { });
        protected override void TickRemainingTime()
        {
            Entities.ForEach((ref TBonus b, ref BonusEvent bonus) =>
            {
                bonus.remainingTime -= deltaTime;
            });
        }
    }
}
#endif