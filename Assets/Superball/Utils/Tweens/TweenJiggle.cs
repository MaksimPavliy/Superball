using UnityEngine;

namespace HcUtils
{
    //Jiggling transform in desired direction
    public class TweenJiggle : FriendsGamesTools.UI.TweenInTime
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 jiggle = new Vector3(0, 0, 0);

        private Vector3 startPosition;

        [SerializeField] bool easeOut;

        protected override void Awake()
        {
            base.Awake();
            startPosition = target.position;
        }
        protected override void OnProgress(float progress)
        {
            var finalProgress = easeOut? Mathf.Sin(progress * Mathf.PI * 0.5f):progress;
            var value = Mathf.PingPong(finalProgress, 1f) * jiggle;

            target.position = startPosition + value;
        }

    }
}