#if UI
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class TransformRotation : TweenInTime
    {
        [SerializeField] protected Transform tgt;
        [SerializeField] protected Vector3 angleSpeed;
        protected override void OnProgress(float progress) {
            tgt.localEulerAngles += angleSpeed * Time.deltaTime;
        }
    }
}
#endif