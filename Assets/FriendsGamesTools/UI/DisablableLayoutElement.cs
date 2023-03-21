#if UI
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{ 
    public class DisablableLayoutElement : MonoBehaviour
    {
        public GameObject shownParent;
        LayoutElement layoutElement;
        public virtual void SetShown(bool shown) => SetShown(shown, gameObject, shownParent, ref layoutElement);
        public static void SetShown(bool shown, GameObject layoutParent, GameObject shownParent, ref LayoutElement layoutElement)
        {
            if (shownParent != null)
                shownParent.SetActive(shown);
            SetShown(shown, layoutParent, ref layoutElement);
        }
        public static void SetShown(bool shown, GameObject layoutParent, ref LayoutElement layoutElement)
        {
            if (layoutElement == null)
                layoutElement = layoutParent.GetOrAddComponent<LayoutElement>();
            layoutElement.ignoreLayout = !shown;
        }
    }
}
#endif