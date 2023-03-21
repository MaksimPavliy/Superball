using FriendsGamesTools.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class Highlighter : MonoBehaviourHasInstance<Highlighter>
#if TUTORIAL || QUESTS
        , IHasScreenSizeChangeCallback
#endif
    {
        [SerializeField] TextMeshProUGUI helpText;
        public RectTransform dimMaskRect;
        [SerializeField] Vector2 margin;
#if TUTORIAL || QUESTS
        public Highlighter CreateInstance()
        {
#if TUTORIAL
            var parent = TutorialManager.instance.transform;
#else
            var parent = QuestsView.instance.transform;
#endif
            return Instantiate(this, parent);
        }
        public static bool shown => instance != null && instance.dimMaskRect.gameObject.activeInHierarchy;
        protected override void Awake()
        {
            base.Awake();
            helpText.gameObject.SetActive(false);
            dimMaskRect.gameObject.SetActive(false);
            highlighterCamera = transform.GetUICamera();
        }
        public void UpdatePosition()
        {
            if (button == null) return;
            var worldLeftBottom = button.rect.localToWorldMatrix.MultiplyPoint3x4(button.rect.rect.min);
            var worldRightTop = button.rect.localToWorldMatrix.MultiplyPoint3x4(button.rect.rect.max);
            SavingScreenCooMoveWorldCooToHighlighterCamera(ref worldLeftBottom);
            SavingScreenCooMoveWorldCooToHighlighterCamera(ref worldRightTop);
            var localLeftBottom = dimMaskRect.transform.parent.worldToLocalMatrix.MultiplyPoint3x4(worldLeftBottom);
            var localRightTop = dimMaskRect.transform.parent.worldToLocalMatrix.MultiplyPoint3x4(worldRightTop);
            dimMaskRect.anchoredPosition = (localLeftBottom + localRightTop) * 0.5f;
            dimMaskRect.sizeDelta = localRightTop - localLeftBottom + (Vector3)margin;
        }
        TutorialButton button;
        Camera buttonCamera;
        public Camera highlighterCamera { get; private set; }
        void SavingScreenCooMoveWorldCooToHighlighterCamera(ref Vector3 worldPos)
        {
            if (buttonCamera == highlighterCamera)
                return;
            var screenPos = buttonCamera.WorldToScreenPoint(worldPos);
            worldPos = highlighterCamera.ScreenToWorldPoint(screenPos);
        }
        bool withAssistant => TutorialAssistantView.instance != null;
        public static void Cancel()
        {
            if (instance == null)
                return;
            instance.cancelled = true;
        }
        public enum Side { top, bottom, right, left }
        private float GetAngle(Side side)
        {
            switch (side)
            {
                default:
                case Side.bottom: return 0; 
                case Side.top: return 180; 
                case Side.right: return 90; 
                case Side.left: return -90; 
            }
        }
        bool cancelled;
        public async Task PressingButton(TutorialButton button, string text, bool dynamic = false, Side side = Side.bottom)
        {
            dimMaskRect.localEulerAngles = new Vector3(0, 0, GetAngle(side));
            //Debug.Log($"inside PressingButton with text {text}, time = {UnityEngine.Time.time}");
            // Show text.
            this.button = button;
            this.buttonCamera = button.transform.GetUICamera();
            if (withAssistant)
                TutorialAssistantView.Show(text);
            else
            {
                helpText.text = text;
                helpText.gameObject.SetActive(true);
            }
            //show arrow at default pos and anim. if needed, customize.
            //show dim. mask it with rect from button.Or use button's overriden mask.
            UpdatePosition();
            dimMaskRect.gameObject.SetActive(true);
            UI.UI.DelayPressing();
            // And helpText should be in the center.
            var screenWidth = dimMaskRect.transform.parent.GetComponent<RectTransform>().rect.width;
            if (!withAssistant)
                helpText.rectTransform.anchoredPosition = new Vector2(screenWidth/2 - dimMaskRect.anchoredPosition.x, helpText.rectTransform.anchoredPosition.y); 
            // Wait button's callback received.
            bool pressed = false;
            button.onPressed = () => pressed = true;
            //Debug.Log($"onpressed subscribed, time = {UnityEngine.Time.time}");
            while (!pressed && !cancelled)
            {
                await Awaiters.EndOfFrame;
                if (this == null)
                    return;
                if (dynamic)
                    UpdatePosition();
            }
            //Debug.Log($"onpressed reached, time = {UnityEngine.Time.time}");
            button.onPressed = null;
            // Hide text.
            if (withAssistant)
                TutorialAssistantView.Hide();
            else
                helpText.gameObject.SetActive(false);
            //hide dim.
            dimMaskRect.gameObject.SetActive(false);
            this.button = null;
        }
        public void OnScreenSizeChanged() => UpdatePosition();
#endif
        }
    }
