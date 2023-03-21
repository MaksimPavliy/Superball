using UnityEngine;

namespace FriendsGamesTools.UI
{
    /// <summary>
    /// Makes rect all parent, but top shift for notch included
    /// </summary>
    [ExecuteAlways]
    public class SafeRect : MonoBehaviour
    {
        public float additionalNotchHeight;
        public float notchHeightCoef = 1f;
#if UI
        float notchHeight => additionalNotchHeight + ScreenSettings.notchHeight * notchHeightCoef;
        float shownNotchHeight = -1;
        float shownScreenHeight = -1;
        float shownCanvasHeight = -1, shownCanvasWidth = -1;
        ScreenOrientation shownOrientation;
        RectTransform canvasRect, rect;
        private void Awake()
        {
            canvasRect = transform.GetComponentInParent<Canvas>().rootCanvas.transform.GetComponent<RectTransform>();
            rect = GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            ApplySafeArea();
        }
        void Update()
        {
            if (shownNotchHeight != notchHeight || shownScreenHeight != Screen.height
                || shownCanvasWidth!= canvasRect.rect.size.x || shownCanvasHeight != canvasRect.rect.size.y
                || shownOrientation != Screen.orientation)
                ApplySafeArea();
        }
        void ApplySafeArea()
        {
            shownNotchHeight = notchHeight;
            shownScreenHeight = Screen.height;
            shownCanvasWidth = canvasRect.rect.size.x;
            shownCanvasHeight = canvasRect.rect.size.y;
            shownOrientation = Screen.orientation;
            var currNotchHeight = notchHeight * canvasRect.rect.height / Screen.height;
            if (!currNotchHeight.IsSane())
                currNotchHeight = 0;

            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = new Vector2(0, -currNotchHeight);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    rect.offsetMin = new Vector2(0, -currNotchHeight);
                    rect.offsetMax = Vector2.zero;
                    break;
                case ScreenOrientation.LandscapeLeft:
                    rect.offsetMin = new Vector2(currNotchHeight, 0);
                    rect.offsetMax = Vector2.zero;
                    break;
                case ScreenOrientation.LandscapeRight:
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = new Vector2(-currNotchHeight, 0);
                    break;
            }

            //Debug.Log($"ApplySafeArea called, notchHeight = {notchHeight}, screen = {Screen.width}x{Screen.height}, canvas = {canvasRect.rect.size.x}x{canvasRect.rect.size.y}," +
            //    $"\n safe area ({Screen.safeArea.xMin},{Screen.safeArea.yMin})-({Screen.safeArea.xMax},{Screen.safeArea.yMax})");
        }
#endif
    }
}
