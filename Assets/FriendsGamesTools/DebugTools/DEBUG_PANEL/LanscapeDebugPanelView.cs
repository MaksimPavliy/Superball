using System;
using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways]
    public class LanscapeDebugPanelView : MonoBehaviour {
        [SerializeField] SafeRect safeRect;
#if DEBUG_PANEL
        bool rotated;
        private void OnEnable()
        {
            if (!rotated && Utils.IsHorizontal())
                Rotate();
        }

        private void Rotate()
        {
            rotated = true;
            safeRect.enabled = false;
            var canvas = UIUtils.GetRootCanvas(transform);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var screenSize = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
            var notch = ScreenSettings.notchHeight;
            //screenSize = new Vector2(200, 100);
            //notch = 10;
            var rect = GetComponent<RectTransform>();
            rect.localRotation = Quaternion.Euler(0, 0, 90);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            var size = new Vector2(screenSize.y, screenSize.x - notch);
            var correctPos = new Vector2(notch * 0.5f, 0);
            rect.offsetMax = size * 0.5f + correctPos;
            rect.offsetMin = -size * 0.5f + correctPos;
            rect.localScale = Vector3.one;
        }
        public static void Init(Transform tr)
            => tr.GetComponentsInChildren<RectMask2D>(true).ForEach(rectMask => rectMask.enabled = false);
#endif
    }
}
