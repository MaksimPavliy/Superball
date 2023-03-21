#if RATE_APP_ADVANCED
using System;
using FriendsGamesTools.ECSGame;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools
{
    public struct RateAppAdvanced : IComponentData {
        public float remainingTime;
        public int remainingAttempts;
    }
    public abstract class RateAppAdvancedController : Controller, IOfflineTimeReceiver
    {
        RateAppAdvancedSettings config => RateAppAdvancedSettings.instance;
        public static RateAppAdvancedController instance => GameRoot.instance.Get<RateAppAdvancedController>();
        protected Entity e => ECSUtils.GetSingleEntity<RateAppAdvanced>();
        public RateAppAdvanced data => e.Get<RateAppAdvanced>();
        public override void InitDefault() {
            base.InitDefault();
            ECSUtils.CreateEntity(new RateAppAdvanced { remainingAttempts = config.attempts, remainingTime = config.startDelay });
        }

        protected virtual bool noTutorial
#if !TUTORIAL
            => true;
#else
            => ECSGame.Tutorial.TutorialManager.instance?.completed ?? true;
#endif
        public bool ShowRateAppIfPossible() {
            var data = this.data;
            if (data.remainingAttempts <= 0 || data.remainingTime > 0 || !noTutorial)
                return false;
            data.remainingAttempts--;
            data.remainingTime = config.nextDelay;
            e.Set(data);
            if (!GameRoot.instance.ViewEnabled)
                return false;
            DoYouLikeOurGameWindow.Show();
            return true;
        }
        void Tick(float deltaTime) => Entities.ForEach((Entity e, ref RateAppAdvanced data) => data.remainingTime -= deltaTime);
        public void DebugRemoveDelay() => e.Modify((ref RateAppAdvanced data) => data.remainingTime = 0);
        protected override void OnUpdate() {
            base.OnUpdate();
            Tick(Time.unscaledDeltaTime);
        }
        public void ApplyOfflineTime(float deltaTime, OfflineType type) => Tick(deltaTime);
        void RemoveAttempts() => e.Modify((ref RateAppAdvanced data) => data.remainingAttempts = 0);
        public void DontRateUs() {
            MessageToDevelopersWindow.Show();
            RemoveAttempts();
        }
        public void RateUs() {
            RateApp.Open();
            RemoveAttempts();
        }
    }
}
#endif