using UnityEngine;

namespace FriendsGamesTools.UI
{
    [ExecuteAlways]
    public class OrientationFromRect : MonoBehaviour
    {
        [SerializeField] RectTransform vertical, horizontal;
        RectTransform _curr;
        RectTransform curr => _curr ?? (_curr = GetComponent<RectTransform>());
        private void Awake() => Update();
        private void Update()
        {
            if (Utils.IsVertical())
                Set(curr, vertical);
            else
                Set(curr, horizontal);
        }

        private void Set(RectTransform curr, RectTransform value)
        {
            if (value == null) return;
            curr.position = value.position;
            curr.anchorMin = value.anchorMin;
            curr.anchorMax = value.anchorMax;
            curr.anchoredPosition = value.anchoredPosition;
            curr.sizeDelta = value.sizeDelta;
        }
    }
}