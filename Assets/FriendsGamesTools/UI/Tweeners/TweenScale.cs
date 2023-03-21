#if UI
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class TweenScale : TweenInTime
    {
        [SerializeField] protected Transform tgt;
        [SerializeField] protected float tgtScale;
        protected override void OnProgress(float progress)
        {
            tgt.localScale = Vector3.one * Mathf.Lerp(1, tgtScale, progress);
        }
    }
}
#endif