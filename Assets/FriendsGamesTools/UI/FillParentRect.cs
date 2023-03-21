using UnityEngine;

namespace FriendsGamesTools.UI
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class FillParentRect : MonoBehaviour
    {
        private void Awake() => Fill(GetComponent<RectTransform>());
        public static void Fill(RectTransform rect)
        {
            rect.pivot = Vector2.one * 0.5f;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}