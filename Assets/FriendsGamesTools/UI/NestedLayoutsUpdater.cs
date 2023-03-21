using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class NestedLayoutsUpdater : MonoBehaviour
    {
        private void OnEnable() {
            Do(gameObject);  
        }
        public static void Do(GameObject gameObject) {
            var layoutGroups = gameObject.GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>();
            layoutGroups.ForEach(layoutGroup => {
                layoutGroup.gameObject.SetActive(false);
                layoutGroup.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.gameObject.GetComponent<RectTransform>());
            });
        }
    }
}