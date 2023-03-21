#if UI
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.UI
{
    public class ButtonScale : EventTrigger
    {
        public float scale = 1.2f;
        public override void OnPointerDown(PointerEventData data)
        {
            targetScale = scale;
        }
        public override void OnPointerUp(PointerEventData data)
        {
            targetScale = 1;
        }
        float targetScale = 1;
        float currScale = 1;
        float speed = 5;
        private void Update()
        {
            currScale = Mathf.MoveTowards(currScale, targetScale, speed * Time.deltaTime);
            transform.localScale = Vector3.one * currScale;
        }
    }
}
#endif