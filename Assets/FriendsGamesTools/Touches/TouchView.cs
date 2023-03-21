using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class TouchView : MonoBehaviour
    {
        [SerializeField] SimpleAnimation anim;
        [SerializeField] AnimationClip appear;
        [SerializeField] AnimationClip disappear;
        [SerializeField] AnimationClip idle;
        [SerializeField] Image picPressed, picNotPressed;
#if TOUCHES
        Vector2 _screenPos;
        RectTransform parentRect;
        public Vector2 screenPos
        {
            get => _screenPos;
            set
            {
                _screenPos = value;
                if (parentRect == null)
                    parentRect = transform.parent.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, _screenPos,
                    TouchesView.instance.uiCamera, out var localPos);
                transform.localPosition = localPos;
            }
        }
        public Vector3 targetWorldPos
        {
            set => screenPos = TouchesView.instance.uiCamera.WorldToScreenPoint(value);
        }
        public void Appear()
        {
            gameObject.SetActive(true);
            anim.Play(appear);
        }
        public async void Disappear()
        {
            anim.Play(disappear);
            await Awaiters.SecondsRealtime(disappear.length);
            gameObject.SetActive(false);
            TouchesView.instance.ReturnToPool(this);
        }
        TouchSettings settings => TouchSettings.instance;
        private void OnEnable()
        {
            picPressed.SetSpriteSafe(settings.picPressed);
            picNotPressed.SetSpriteSafe(settings.picNormal);
        }
        private void Update() => UpdateTapping();
        public bool isTapping;
        void UpdateTapping()
        {
            if (!isTapping || anim.isPlaying) return;
            anim.Stop();
            anim.Play(idle);
        }
#endif
    }
}