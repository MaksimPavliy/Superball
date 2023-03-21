#if UI
using UnityEngine;

namespace FriendsGamesTools.UI
{
    /// <summary>
    /// Makes rect occupy notch line at the top of the screen.
    /// </summary>
    [ExecuteAlways]
    public class NotchRect : MonoBehaviour
    {
        public float additionalHeight;
        public float notchHeightCoef = 1f;
        float notchHeight => additionalHeight + ScreenSettings.notchHeight * notchHeightCoef;
        float shownNotchHeight = -1;
        RectTransform canvasRect, rect;
        private void Awake()
        {
            var canvas = transform.parent.GetComponent<Canvas>();
            Debug.Assert(canvas!=null, "should be direct child of canvas");
            canvasRect = canvas.transform.GetComponent<RectTransform>();
            rect = GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = Vector2.one;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            //Debug.Log($"ScreenSettings.notchHeight={ScreenSettings.notchHeight}");
            ApplySafeArea();
        }
        void Update()
        {
            if (shownNotchHeight!= notchHeight)
                ApplySafeArea();
        }
        void ApplySafeArea()
        {
            shownNotchHeight = notchHeight;
            var currNotchHeight = notchHeight * canvasRect.rect.height / Screen.height;
            rect.offsetMin = new Vector2(0, -notchHeight);
            rect.offsetMax = Vector2.zero;
        }
    }
}
#endif