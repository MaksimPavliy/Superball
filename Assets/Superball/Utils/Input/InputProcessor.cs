using FriendsGamesTools;
using System.Threading.Tasks;
using UnityEngine;

namespace HcUtils
{
    public class InputProcessor: MonoBehaviourHasInstance<InputProcessor>
    {
        private FriendsGamesTools.Touch usedTouch;
        public FriendsGamesTools.Touch ActiveTouch { private set; get; }

        private SwipeType lastSwipe = SwipeType.None;
        float minDistance =10f;

        public enum SwipeType
        {
            None,
            Up,
            Down,
            Right,
            Left
        }
        
        protected FriendsGamesTools.Touch GetActiveTouch()
        {
            
            if (TouchesManager.instance.touches.Count > 0)
            {
                FriendsGamesTools.Touch touch = null;
                if (TouchesManager.instance.GetTouch(-1) != null) touch = TouchesManager.instance.GetTouch(-1);
                else if ((TouchesManager.instance.GetTouch(0) != null)) touch = TouchesManager.instance.GetTouch(0);
                return touch;
            }

            return null;
        }

        public Vector2 GetScreenCenterOffset()
        {
            var touch = ActiveTouch;
            if (ActiveTouch==null) return Vector3.zero;
            Vector2 delta = Vector2.zero;
            delta.x = touch.currScreenCoo.x / Screen.width - 0.5f;
            delta.y = touch.currScreenCoo.y / Screen.height - 0.5f;
            return delta;
        }
        public async Task AwaitSwipe(SwipeType swipeType)
        {
            await Awaiters.Until(() => lastSwipe == swipeType);
            lastSwipe = SwipeType.None;
            return;
        }

        public async Task AwaitNoTouch()
        {
            await Awaiters.Until(() => ActiveTouch==null);
            lastSwipe = SwipeType.None;
            return;
        }

        private SwipeType SwipeProcessing(Vector2 delta)
        {
            var ratio = Mathf.Abs(delta.y / (delta.x==0?0.001f:delta.x));

            bool ySwipe = ratio > 0.7f;
            bool xSwipe = ratio < 0.3f;
            SwipeType dir;
            if (delta.y>0 && ySwipe)
            {
                return SwipeType.Up;
            }

            if (delta.y < 0 && ySwipe)
            {
                return SwipeType.Down;
            }

            if (delta.x > 0 && xSwipe)
            {
                return SwipeType.Right;
            }

            if (delta.x < 0 && xSwipe)
            {
                return SwipeType.Left;
            }

            return SwipeType.None;
        }
        private void Update()
        {
            ActiveTouch = GetActiveTouch();
            if (ActiveTouch == null)
            {
                usedTouch = null;
                return;
            }

            if (ActiveTouch == usedTouch) return;

            var delta = ActiveTouch.deltaScreenCoo;
            if (delta.magnitude > minDistance)
            {
                lastSwipe=SwipeProcessing(delta);
                usedTouch = ActiveTouch;
            }
        }
    }
}