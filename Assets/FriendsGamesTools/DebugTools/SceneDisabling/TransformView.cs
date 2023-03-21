using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class TransformView : MonoBehaviour
    {
        [SerializeField] Toggle enabledCheckbox;
        [SerializeField] TextMeshProUGUI nameLabel;
        [SerializeField] TextMeshProUGUI childCountLabel;
        [SerializeField] GameObject toChildrenParent;
#if DEBUG_TOOLS
        Transform curr;
        SceneDisablingView view;
        public void Show(Transform curr, SceneDisablingView view)
        {
            this.view = view;
            this.curr = curr;
            enabledCheckbox.isOn = curr.gameObject.activeInHierarchy;
            enabledCheckbox.interactable = !view.IsProtected(curr);
            nameLabel.text = curr.name;
            childCountLabel.text = curr.childCount.ToString();
            toChildrenParent.SetActive(curr.childCount > 0);
        }
        public void OnEnabledChanged()
        {
            if (view.programChange)
                return;
            if (curr.gameObject.activeSelf && !curr.gameObject.activeInHierarchy)
                return;
            view.SetTransformEnabled(curr, enabledCheckbox.isOn);
        }
        public void OnToChildrenPressed() => view.ShowTransform(curr);
#endif
    }
}
