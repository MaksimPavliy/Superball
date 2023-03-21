using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public abstract class TweenInTime : MonoBehaviour
    {
        public float duration = 1;
        [SerializeField] protected bool startsEnabled = false;
        [SerializeField] protected float loopsCount = -1;
        [SerializeField] bool realtime;
#if UI
        protected new bool enabled;
        protected float elapsed { private set;get; }
        public void SetEnabled(bool enabled)
        {
            if (this.enabled == enabled)
                return;
            this.enabled = enabled;
            elapsed = 0;
            OnProgress();
            OnEnabledChanged();
        }
        protected virtual void OnEnabledChanged() { }
        protected abstract void OnProgress(float progress);
        protected void OnProgress() => OnProgress(GetProgress());
        protected virtual void Awake() => enabled = startsEnabled;
        protected virtual float GetProgress()
        {
            var halfDuration = duration * 0.5f;
            return Mathf.PingPong(elapsed, halfDuration) / halfDuration;
        }
        protected virtual void Update()
        {
            if (!enabled)
                return;
            elapsed += realtime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (loopsCount > 0 && elapsed > loopsCount * duration)
            {
                SetEnabled(false);
                return;
            }
            OnProgress();
        }
        public virtual void Play(int loopsCount)
        {
            this.loopsCount = loopsCount;
            SetEnabled(true);
        }
        public virtual void PlayOnce() => Play(1);
        public async Task PlayingOnce() => await Playing(1);
        public async Task Playing(int loopsCount)
        {
            Play(loopsCount);
            while (enabled)
                await Awaiters.EndOfFrame;
        }
        public void PlayLooped() => Play(-1);
#endif
    }
}
