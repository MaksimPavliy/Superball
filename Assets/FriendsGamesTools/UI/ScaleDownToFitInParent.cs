using UnityEngine;

namespace FriendsGamesTools.UI
{
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public class ScaleDownToFitInParent : MonoBehaviour
    {
        private void OnEnable()
        {
            if (Utils.IsPrefabOpened()) return;
            var rect = GetComponent<RectTransform>();
            var parentRect = rect.parent.GetComponent<RectTransform>();
            var topScale = (parentRect.rect.height * 0.5f + rect.anchoredPosition.y) / (rect.rect.height * 0.5f);
            var bottomScale = (parentRect.rect.height * 0.5f - rect.anchoredPosition.y) / (rect.rect.height * 0.5f);
            var rightScale = (parentRect.rect.width * 0.5f + rect.anchoredPosition.x) / (rect.rect.width * 0.5f);
            var leftScale = (parentRect.rect.width * 0.5f - rect.anchoredPosition.x) / (rect.rect.width * 0.5f);
            var minScale = Mathf.Min(1, topScale, bottomScale, rightScale, leftScale);
            rect.localScale = Vector3.one * minScale;
        }
    }
}