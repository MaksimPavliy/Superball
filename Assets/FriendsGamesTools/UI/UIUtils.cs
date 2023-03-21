using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public static class UIUtils 
    {
        public static Canvas GetRootCanvas(this Transform tr)
            => tr.GetComponentInParent<Canvas>().rootCanvas;
        public static Camera GetUICamera(this Transform tr)
            => tr.GetRootCanvas().worldCamera ?? Camera.main;
        public static float GetAlpha(this MaskableGraphic item) => item.color.a;
        public static void SetAlpha(this MaskableGraphic item, float alpha)
        {
            var col = item.color;
            col.a = alpha;
            item.color = col;
        }
    }
}