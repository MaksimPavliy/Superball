#if ECS_SKINS
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class ScrollArrowsView : MonoBehaviour
    {
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] Button arrowLeft;
        [SerializeField] Button arrowRight;
        private void Update()
        {
            arrowLeft.interactable = scrollRect.horizontalNormalizedPosition >= 0.5f;
            arrowRight.interactable = scrollRect.horizontalNormalizedPosition <= 0.5f;
        }
        public void SetEnabled(bool enabled)
        {
            scrollRect.enabled = enabled;
            arrowLeft.gameObject.SetActive(enabled);
            arrowRight.gameObject.SetActive(enabled);
        }
    }
}
#endif