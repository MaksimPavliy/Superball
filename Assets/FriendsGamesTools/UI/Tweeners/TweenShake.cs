#if UI
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class TweenShake : TweenInTime
    {
        [SerializeField] protected Transform tgt;
        [SerializeField] protected float tgtActionScale = 1.2f;
        [SerializeField] float shakeAngle = 5;
        [SerializeField] float shakeSpeed = 10f;
        [SerializeField] float idleTimeFraction = 0.09f;
        [SerializeField] float tgtIdleScale = 1.05f;
        [SerializeField] int idleScaleLoops = 2;

        protected Transform tr => tgt ? tgt : transform;
        protected override float GetProgress() => elapsed / duration - (int)(elapsed / duration);
        protected override void OnProgress(float progress)
        {
            var shakeTimeFraction = 1 - idleTimeFraction;
            if (progress > idleTimeFraction)
                OnIdleProgress((progress - idleTimeFraction) / shakeTimeFraction);
            else
                OnShakeProgress(progress / idleTimeFraction);
        }
        void OnShakeProgress(float progress)
        {
            var pingedProgress = Mathf.PingPong(progress * 2, 1);
            tr.localScale = Vector3.one * Mathf.Lerp(1, tgtActionScale, pingedProgress);
            var rot = shakeAngle * Mathf.Cos(progress * shakeSpeed);
            tr.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, rot, pingedProgress));
        }
        void OnIdleProgress(float progress)
        {
            tr.localScale = Vector3.one * Mathf.Lerp(1, tgtIdleScale, Mathf.PingPong(progress * idleScaleLoops, 1));
            tr.localRotation = Quaternion.Euler(0, 0, 0);
        }
        public override void PlayOnce() {
            loopsCount = 1 - idleTimeFraction;
            SetEnabled(true);
        }
    }
}
#endif