using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    [RequireComponent(typeof(Image))]
    public class Joystick : MonoBehaviourHasInstance<Joystick>, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public bool isFixed;
        public float maxDistance;
        [SerializeField] RectTransform originView, pointerView;
        public bool isMoving {
            get => touchId != noTouchId;
            set => touchId = noTouchId;
        }
        private bool isDragging => isMoving && touchId != keyboardTouchId;
        const int noTouchId = -10;
        int _touchId = noTouchId;
        bool shown = true;

        public event Action<Vector2> Dragged;
        public event Action DragEnded;

        private Vector2 lastDragPosition;
        private Vector2 dragDelta;

        private Camera _lastEventCamera;
        private int touchId {
            get => _touchId;
            set {
                _touchId = value;
                pointerView.gameObject.SetActive(shown && isMoving);
                originView.gameObject.SetActive(shown && (isMoving || isFixed));
                if (!isMoving)
                    dragDir = Vector2.zero;
            }
        }

        public void Show()
        {
            originView.gameObject.SetActive(true);
            pointerView.gameObject.SetActive(true);
            shown = true;
        }

        public void Hide()
        {
            originView.gameObject.SetActive(false);
            pointerView.gameObject.SetActive(false);
            shown = false;
        }

        RectTransform rect;
        void Start() {
            isMoving = false;
            //originView.anchoredPosition = fixedPosition;
            rect = GetComponent<RectTransform>();
        }

        Vector2 GetTouchLocalPos(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, eventData.position, eventData.enterEventCamera, out Vector2 localPos);
            return localPos;
        }

        Vector2 GetTouchLocalPos(Vector2 screenPos, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, screenPos, eventCamera, out Vector2 localPos);
            return localPos;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isMoving) return;
            touchId = eventData.pointerId;
            if (!isFixed)
                originView.anchoredPosition = GetTouchLocalPos(eventData);
            lastDragPosition = eventData.position;
            _lastEventCamera = eventData.enterEventCamera;
        }

        public void ResetDrag()
        {
            if (dragDir.magnitude > 0)
            {
                originView.anchoredPosition = GetTouchLocalPos(lastDragPosition,_lastEventCamera);
                ApplyDragDir(lastDragPosition);             
                dragDir= Vector2.zero;
                pointerView.anchoredPosition = originView.anchoredPosition;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            var dragDir = (GetTouchLocalPos(eventData) - originView.anchoredPosition);
            ApplyDragDir(dragDir);
            
            dragDelta = eventData.position - lastDragPosition;
            Dragged?.Invoke(dragDelta);
            lastDragPosition = eventData.position;
            _lastEventCamera = eventData.enterEventCamera;
        }
        void ApplyDragDir(Vector2 dragDirInScreen)
        {
            var distSqr = dragDirInScreen.sqrMagnitude;
            if (distSqr > maxDistance * maxDistance)
                dragDirInScreen *= maxDistance / Mathf.Sqrt(distSqr);
            dragDir = dragDirInScreen / maxDistance;
            if (distSqr > 0)
                angle = Mathf.Atan2(dragDir.y, dragDir.x) * Mathf.Rad2Deg - 90;
            pointerView.anchoredPosition = originView.anchoredPosition + dragDir*maxDistance;
            originView.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (touchId == eventData.pointerId)
                EndDrag();
        }
        private void EndDrag()
        {
            isMoving = false;
            dragDelta = Vector2.zero;
            DragEnded?.Invoke();
        }
        public float angle { get; private set; }
        public Vector2 dragDir { get; private set; }
        void OnDisable() => EndDrag();

        void Update()
        {
            UpdateKeyboard();
        }

        [SerializeField] bool keyboard = true;
        const int keyboardTouchId = -3;
        void UpdateKeyboard()
        {
            if (!keyboard || isDragging) return;
            var up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            var down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            var left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            var right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            var pressed = up || down || left || right;
            var dragDir = Vector2.zero;
            if (up) dragDir += new Vector2(0, 1);
            if (down) dragDir += new Vector2(0, -1);
            if (left) dragDir += new Vector2(-1, 0);
            if (right) dragDir += new Vector2(1, 0);
            dragDir *= maxDistance;
            ApplyDragDir(dragDir);
            touchId = pressed ? keyboardTouchId : noTouchId;
        }

    }
}