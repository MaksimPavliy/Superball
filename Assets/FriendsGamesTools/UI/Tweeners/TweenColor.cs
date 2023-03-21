#if UI
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class TweenColor : TweenInTime
    {
        [SerializeField] protected Graphic tgt;
        [SerializeField] protected Color tgtColor;
        Color startCol;
        protected override void Awake()
        {
            base.Awake();
            startCol = tgt.color;
        }
        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();
            if (enabled)
                startCol = tgt.color;
            else
                tgt.color = startCol;
        }
        protected override void OnProgress(float progress)
        {
            tgt.color = Color.Lerp(startCol, tgtColor, progress);
        }
    }
}
#endif