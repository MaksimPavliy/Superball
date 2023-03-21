#if CAMERA
using FriendsGamesTools.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools
{
    [RequireComponent(typeof(Camera))]
    public abstract class CameraMover<TSelf> : CameraMover
        where TSelf : CameraMover<TSelf>
    {
        new public static TSelf instance { get; private set; }
        protected override void Awake()
        {
            instance = (TSelf)this;
            base.Awake();
        }
    }
    [RequireComponent(typeof(Camera))]
    public abstract class CameraMover : MonoBehaviour
    {
        #region Common
        public static CameraMover instance { get; private set; }
        protected virtual void Awake()
        {
            instance = this;
            SetBounds(bounds);
            InitMovements();
        }
        protected virtual void Update()
        {
            UpdateMovements();
        }
        protected virtual void LateUpdate()
        {
            UpdateFollowing();
        }
        #endregion

        #region Drag scale
        public enum MoveType { None, Dragging, DragScale, ProgramaticalMove }
        public abstract MoveType allowedMoveType { get; }
        public bool dontMoveAboveUI = true;
        public MoveType actualMoveType
        {
            get
            {
                if (dontMoveAboveUI && UIUnderPos.instance != null && UIUnderPos.instance.FingersAboveUI())
                    return MoveType.None;
                if (isMovingProgrammatically)
                    return MoveType.ProgramaticalMove;
                return allowedMoveType;
            }
        }
        public bool draggingEnabled => actualMoveType == MoveType.Dragging || actualMoveType == MoveType.DragScale;
        public bool scalingEnabled => actualMoveType == MoveType.DragScale;
        public bool inputEnabled => draggingEnabled || scalingEnabled;
        
        void UpdateDraggingPinch()
        {
            int consideredTouchesCount = touches.Count;
            switch (actualMoveType)
            {
                case MoveType.Dragging: consideredTouchesCount = Mathf.Min(1, consideredTouchesCount); break;
                case MoveType.DragScale: consideredTouchesCount = Mathf.Min(2, consideredTouchesCount); break;
                default: consideredTouchesCount = 0; break;
            }
            switch (consideredTouchesCount)
            {
                case 0: break;
                case 1: UpdateDraggingOneTouch(); break;
                default: UpdateDraggingPinchTwoTouches(); break;
            }
        }
        void UpdateDraggingOneTouch()
        {
            // Idea: World coordinate of the touch remains the same.
            var touch = touches[0];
            var touchStartWorldPos = touch.startWorldCoo;
            var touchCurrScreenPos = touch.currScreenCoo;

            var screenRelativePos = new Vector2(touchCurrScreenPos.x / cam.pixelWidth, touchCurrScreenPos.y / cam.pixelHeight) - Vector2.one * 0.5f;
            // touchStartWorldPos = screen2World(orthographicSize)*touchCurrScreenPos;
            // touchStartWorldPos = cameraTransform.position  + cam.orthographicSize * 2 * cameraTransform.up.normalized * screenRelativePos.y +cam.orthographicSize * 2 * cam.aspect * cameraTransform.right.normalized * screenRelativePos.x;
            cameraTransform.position = GetCameraPosLookingAt(touchStartWorldPos - cam.orthographicSize * 2 * (cameraTransform.up.normalized * screenRelativePos.y + cam.aspect * cameraTransform.right.normalized * screenRelativePos.x));
        }
        void UpdateDraggingPinchTwoTouches()
        {
            var touch1 = touches[0];
            var touch2 = touches[1];
            // Idea: distance between touches in world coordinates remain the same.
            //  (touch1.startWorldCoo-touch2.startWorldCoo).length
            //  = 
            //  (touch1.currWorldCoo-touch2.currWorldCoo).length

            var startDist = (touch1.startWorldCoo - touch2.startWorldCoo).magnitude;
            var screenShift = touch1.currScreenCoo - touch2.currScreenCoo;
            var screenRelativeShift = new Vector2(screenShift.x / cam.pixelWidth, screenShift.y / cam.pixelHeight);

            var currWorldShiftNoOrthoSize = 2 *
             (cameraTransform.up.normalized * screenRelativeShift.y
            + cam.aspect * cameraTransform.right.normalized * screenRelativeShift.x);
            var currWorldDistNoOrthoSize = currWorldShiftNoOrthoSize.magnitude;
            // startDist = currWorldDistNoOrthoSize*cam.orthographicSize
            if (bounds != null)
                cam.orthographicSize = bounds.ClampSize(startDist / currWorldDistNoOrthoSize);

            // Commented this to make scaling only from the center of the screen.
            //UpdateDraggingOneTouch();
        }
        [SerializeField] float scrollScaleSpeed = 0.1f;
        void UpdateMouseWheelScaling()
        {
            if (actualMoveType != MoveType.DragScale)
                return;
            var size = cam.orthographicSize * (1 + Input.mouseScrollDelta.y * scrollScaleSpeed);
            if (bounds != null)
                size = bounds.ClampSize(size);
            cam.orthographicSize = size;
        }
        #endregion

        #region Movements general
        List<Touch> touches => TouchesManager.instance.touches;
        bool inDraggingTap;
        float dragThreshold = 0.03f; // screen width part.
        float dragThresholdPx => dragThreshold * Screen.width;
        float draggingTapFinishTime;
        public bool movedRecently
        {
            get
            {
                var tappedRecently = draggingTapFinishTime + 0.2f > Time.realtimeSinceStartup;
                var movedRecently = inDraggingTap || tappedRecently;
                return movedRecently;
            }
        }
        void InitMovements()
        {
#if UI
            UI.UI.EnsureInited();
#endif
        }
        void UpdateMovements()
        {
            // Start dragging?
            if (!inDraggingTap && inputEnabled && touches.Count > 0)
            {
                for (int i = 0; i < touches.Count; i++)
                {
                    // Dragging starts from some threshold.
                    if ((touches[i].startScreenCoo - touches[i].currScreenCoo).sqrMagnitude
                        > dragThresholdPx * dragThresholdPx)
                        inDraggingTap = true;
                }
            }
            // Dragging and pinch.
            if (inputEnabled && inDraggingTap)
                UpdateDraggingPinch();
            if (inputEnabled)
                UpdateMouseWheelScaling();
            // Stop dragging?
            if (inDraggingTap && touches.Count == 0)
            {
                inDraggingTap = false;
                draggingTapFinishTime = Time.realtimeSinceStartup;
            }

            UpdateInertia();
            bounds?.CameraPosRestrictions();
        }
        #endregion

        #region Inertia
        Vector2 prevWorldPos;
        Vector2 inertia;
        public float inertiaDecreaseCoef = 7f;
        public bool enableInertia = true;
        void UpdateInertia()
        {
            if (!enableInertia || !inputEnabled)
            {
                inertia = Vector2.zero;
                return;
            }
            if (inDraggingTap)
            {
                // Build up inertia.
                var delta = (Vector2)cameraTransform.position - prevWorldPos;
                inertia = delta / Time.deltaTime;
                if (!inertia.IsSane())
                    inertia = Vector2.zero;
            }
            else
            {
                // Decrease inertia.
                var length = inertia.magnitude;
                var lengthDecreased = length * (1 - Time.deltaTime * inertiaDecreaseCoef);
                if (lengthDecreased <= 0)
                    inertia = Vector2.zero;
                else
                    inertia = inertia / length * lengthDecreased;
                var delta = inertia * Time.deltaTime;
                // Apply inertia.
                cameraTransform.position += (Vector3)delta;
            }
            prevWorldPos = cameraTransform.position;
        }
        #endregion

        #region Restrictions
        [SerializeField] protected CameraMoverBounds bounds;
        public float startScale => bounds.startOrthographicSize;
        public virtual void SetBounds(CameraMoverBounds bounds)
        {
            this.bounds = bounds;
            bounds?.Init(this);
        }
        #endregion

        #region Programatical move
        bool isMovingProgrammatically;
        public async Task MoveTo(Transform target, float moveDuration = 1)
            => await MoveScaleTo(target, cam.orthographicSize, moveDuration);
        public async Task ScaleTo(float tgtScale, float moveDuration = 1)
            => await MoveScaleTo(cameraTransform, tgtScale, moveDuration);
        public async Task MoveScaleTo(Transform target, float tgtScale, float moveDuration)
        {
            isMovingProgrammatically = true;
            inertia = Vector2.zero;
            var startWorld = GetCameraPosLookingAt(cameraTransform.position);
            var startScale = cam.orthographicSize;
            float elapsed = 0;
            while (elapsed < moveDuration)
            {
                await Awaiters.EndOfFrame;
                if (target == null)
                    break;
                elapsed += Time.unscaledDeltaTime;
                var progress = Mathf.Clamp01(elapsed / moveDuration);
                var targetWorld = GetCameraPosLookingAt(target.position);
                var currWorld = Vector3.Lerp(startWorld, targetWorld, Mathf.SmoothStep(0, 1, progress));
                cameraTransform.position = currWorld;
                var currSize = Mathf.Lerp(startScale, tgtScale, progress);
                cam.orthographicSize = currSize;
            }
            // Wait 2 frames to allow UI to rebuild after movement.
            await Awaiters.EndOfFrame;
            await Awaiters.EndOfFrame;
            isMovingProgrammatically = false;
        }
        protected virtual Vector3 GetCameraPosLookingAt(Vector3 target) => target.ZTo(cameraTransform.position.z);
        public void MoveToInstantly(Transform target)
            => cameraTransform.position = GetCameraPosLookingAt(target.transform.position);
        Transform followTarget;
        public void CancelFollow() => Follow(null);
        public void Follow(Transform followTarget)
        {
            this.followTarget = followTarget;
        }
        void UpdateFollowing()
        {
            if (followTarget == null)
                return;
            MoveToInstantly(followTarget);
        }
        #endregion

        #region Camera 
        Camera _cam;
        public Camera cam => this == null ? null : (_cam == null ? (_cam = GetComponent<Camera>()) : _cam);
        public Transform _cameraHolder;
        public Transform cameraTransform => _cameraHolder != null ? _cameraHolder : cam.transform;
        public bool InsideScreen(Vector3 pos) => Utils.IsInsideScreen(pos, cam);
        #endregion
    }
}
#endif