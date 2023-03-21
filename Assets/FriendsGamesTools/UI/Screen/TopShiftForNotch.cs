using UnityEngine;

namespace FriendsGamesTools.UI
{
    /// <summary>
    /// Enlarges rect height by notch size.
    /// </summary>
    [ExecuteAlways]
    public class TopShiftForNotch : MonoBehaviour
    {
#if UI
        Rect safeArea
#if UNITY_EDITOR
        {
            get
            {
                var rect = Screen.safeArea;
                if (ScreenSettings.instance != null)
                {
                    var margins = ScreenSettings.margins;
                    rect.position += new Vector2(margins.leftMargin, margins.bottomMargin);
                    rect.width -= margins.leftMargin + margins.rightMargin;
                    rect.height -= margins.bottomMargin + margins.topMargin;
                }
                return rect;
            }
        }
#else
        => Screen.safeArea;
#endif
        RectTransform rect;
        RectTransform canvasRect;
        Vector2 startSize;
        private void Awake()
        {
            var canvas = transform.parent.GetComponent<Canvas>();
            Debug.Assert(canvas!=null, "should be direct child of canvas");
            canvasRect = canvas.transform.GetComponent<RectTransform>();
            rect = GetComponent<RectTransform>();
            FillParentRect.Fill(rect);
        }
        void Start()
        {
            //Debug.Log(Screen.safeArea);
            startSize = rect.sizeDelta;
            ApplySafeArea();
        }

        void Update() => ApplySafeArea();

        float shownNotchHeight;
        void ApplySafeArea()
        {
            float currNotchHeight = ScreenSettings.notchHeight;
            currNotchHeight *= canvasRect.rect.height / Screen.height;
            if (shownNotchHeight == currNotchHeight)
                return;
            shownNotchHeight = currNotchHeight;
            rect.offsetMax = new Vector2(0, -shownNotchHeight);
        }
#endif
    }
}
