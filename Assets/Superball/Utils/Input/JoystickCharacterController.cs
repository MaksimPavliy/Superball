using FriendsGamesTools;
using UnityEngine;

namespace HcUtils
{
    public class JoystickCharacterController: MonoBehaviour
    {
        bool _canControl = false;
        [SerializeField] 
        private MonoBehaviour target;
        protected IControllable controllable;
        [SerializeField] private bool relativeOffset;
        public bool CanControl
        {
            set
            {
                if (value == _canControl) return;
                _canControl = value;
                if (value)
                {
                    Joystick.instance.Dragged += OnDragged;
                    Joystick.instance.DragEnded += OnDragEnded;
                }
                else
                {
                    Joystick.instance.Dragged -= OnDragged;
                    Joystick.instance.DragEnded -= OnDragEnded;
                }
            }
            get { return _canControl; }
        }

        private void Awake()
        {
            controllable = target as IControllable;
            if (controllable==null) return;
            controllable.CanControlChanged += OnCanControlChanged;
        }
        private void OnCanControlChanged(bool canControl)
        {
            CanControl = canControl;
        }
        private void OnDragged(Vector2 dir)
        {
            if (controllable == null) return;
            controllable.OnDragged(relativeOffset?dir:Joystick.instance.dragDir);
        }

        private void OnDragEnded()
        {
            if (controllable == null) return;
            controllable.OnDragEnded();
        }

        private void OnDestroy()
        {
            if (Joystick.instance == null) return;

            Joystick.instance.Dragged -= OnDragged;
            Joystick.instance.DragEnded -= OnDragEnded;
        }
        
  
    }
}