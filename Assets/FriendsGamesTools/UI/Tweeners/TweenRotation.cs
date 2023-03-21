#if UI
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class TweenRotation : TweenInTime
    {
        [SerializeField] protected Transform tgt;
        [SerializeField] protected Vector3 angleStart, angleEnd;
        protected override void OnProgress(float progress) {
            tgt.localEulerAngles = Vector3.Lerp(angleStart, angleEnd, Mathf.SmoothStep(0, 1, progress));
        }
    }
}
#endif