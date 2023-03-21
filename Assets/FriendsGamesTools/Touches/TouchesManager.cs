#if TOUCHES
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FriendsGamesTools
{
    public class TouchesManager : MonoBehaviourSingleton<TouchesManager>
    {
        public Camera worldCamera;
        public UnityEvent onInputDown;

        protected override void Awake() 
        {
            onInputDown = new UnityEvent();

            base.Awake(); 
        }

        void Start()
        {
#if CAMERA
            if (worldCamera == null)
                worldCamera = CameraMover.instance?.cam;
#endif
            if (worldCamera == null)
                worldCamera = Camera.main;
        }
        public List<Touch> touches = new List<Touch>();
        public Touch GetTouch(int id) => touches.Find(t => t.id == id);
        protected Touch AddTouch(int id, Vector2 screenPos)
        {
            onInputDown.Invoke();

            var touch = new Touch(id, screenPos, worldCamera.ScreenToWorldPoint(screenPos));
            touches.Add(touch);
            return touch;
        }
        public bool inputEnabled = true;
        void UpdateTouches()
        {
            if (!inputEnabled)
                touches.Clear();
            else
            {
                foreach (var t in touches)
                    t.exists = false;
                if (Input.touchCount == 0 && Input.GetMouseButton(0))
                {
                    // Touch from mouse.
                    var touch = GetTouch(-1);
                    if (touch == null)
                        touch = AddTouch(-1, Input.mousePosition);
                    touch.ChangeCurrScreenCoo(Input.mousePosition);
                    touch.exists = true;
                }
                else if (Input.touchCount > 0)
                {
                    // Touches from touchscreen.
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        var unityTouch = Input.GetTouch(i);
                        var touch = GetTouch(unityTouch.fingerId);
                        if (touch == null)
                            touch = AddTouch(unityTouch.fingerId, unityTouch.position);
                        touch.ChangeCurrScreenCoo(unityTouch.position);
                        touch.exists = true;
                    }
                }
                if (touches.Any(t => !t.exists)) // Any touch ended -> recreate each of them. Useful when 2-nd touch in drag wants to become 1-st touch used for dragging.
                    touches.Clear();
                //touches.RemoveAll(t => !t.exists);
            }
        }
        private void Update() => UpdateTouches();
    }
}
#endif