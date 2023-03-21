using UnityEngine;

namespace FriendsGamesTools.UI
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class FillParentRectAlways : MonoBehaviour
    {
        RectTransform rect;
        private void Awake() => rect = GetComponent<RectTransform>();
        private void Update() => FillParentRect.Fill(rect);
    }
}