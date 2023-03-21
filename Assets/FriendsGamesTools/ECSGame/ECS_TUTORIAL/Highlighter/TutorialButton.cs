using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.ECSGame.Tutorial
{
#if TUTORIAL || QUESTS
    public class TutorialButton : TutorialButtonAbstarct
#else
    public class TutorialButton : MonoBehaviour
#endif
    {
        [SerializeField] bool showTouch;
        [SerializeField] Vector3 touchShift;
#if TUTORIAL || QUESTS
        RectTransform _rect;
        public RectTransform rect => _rect ?? (_rect = GetComponent<RectTransform>());
        public Action onPressed;
        public override void OnPointerClick(PointerEventData eventData)
            => onPressed?.Invoke();
        [SerializeField] Highlighter highlighterPrefab;
        [SerializeField] bool dynamic;
        Highlighter highlighter;
        public override async Task PressingButton(string text)
        {
            //Debug.Log($"show PressingButton with text {text}, time = {UnityEngine.Time.time}");
            ShowTouch();
            highlighter = highlighterPrefab.CreateInstance();
            await highlighter.PressingButton(this, text, dynamic);
            Destroy(highlighter.gameObject);
            highlighter = null;
            HideTouch();
        }
        private void Update() => UpdateTouch();

    #region Touch view
        
        TouchView touch;
        void ShowTouch()
        {
            if (!showTouch) return;
#if TOUCHES
            touch = TouchesView.instance.CreateTouch();
            touch.isTapping = true;
#endif
        }
        void HideTouch()
        {
            if (!showTouch) return;
#if TOUCHES
            touch.Disappear();
#endif
        }
        void UpdateTouch()
        {
            if (!showTouch || highlighter == null) return;
#if TOUCHES
            var corner = new Vector2(highlighter.dimMaskRect.rect.xMax, highlighter.dimMaskRect.rect.yMin);
            var worldPos = highlighter.dimMaskRect.localToWorldMatrix.MultiplyPoint3x4(corner);
            var screenPos = highlighter.highlighterCamera.WorldToScreenPoint(worldPos);
            touch.screenPos = screenPos + touchShift;
#endif
        }
        #endregion
#endif
    }
}