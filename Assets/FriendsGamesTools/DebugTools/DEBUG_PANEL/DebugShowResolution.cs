#if DEBUG_PANEL
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugShowResolution : MonoBehaviour
    {
        public Text text;
        public Canvas canvas;
        private void OnEnable()
        {
            var width = canvas != null ? canvas.pixelRect.width : Screen.width;
            var height = canvas != null ? canvas.pixelRect.height : Screen.height;
            text.text = $"{width}x{height}";
        }
    }
}
#endif