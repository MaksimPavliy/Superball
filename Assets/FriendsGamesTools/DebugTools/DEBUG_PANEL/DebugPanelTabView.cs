using FriendsGamesTools.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelTabView : MonoBehaviour
    {
        public DebugPanelItemHeader headerPrefab;
        [SerializeField] GameObject multipleItemsParentGameObject;
        [SerializeField] Transform multipleItemsParentTransform;
        [SerializeField] Transform singleItemParent;
#if DEBUG_PANEL
        public DebugPanelSettings settings => DebugPanelSettings.instance;
        List<DebugPanelItemView> prefabs;
        public void SetItems(List<DebugPanelItemView> prefabs) 
            => this.prefabs = prefabs.SortedBy(item => item.sortPriority);
        List<DebugPanelItemView> items;
        private void OnEnable()
        {
            if (items != null) return;
            items = new List<DebugPanelItemView>();
            var showSingleItem = prefabs[0].wholeTab;
            multipleItemsParentGameObject.SetActive(!showSingleItem);
            singleItemParent.gameObject.SetActive(showSingleItem);
            if (showSingleItem)
            {
                // Single item.
                var item = Instantiate(prefabs[0], singleItemParent);
                if (prefabs.Count != 1)
                    Debug.LogError($"Debug panel tab {prefabs[0].whereToShow.tab} should have only one tab, " +
                        $"but has {prefabs.ConvertAll(p => p.name).PrintCollection()}");
                FillParentRect.Fill(item.GetComponent<RectTransform>());
                items.Add(item);
            }
            else
            {
                // Multiple items.
                prefabs.ForEach(prefab =>
                {
                    var (_, name) = prefab.whereToShow;
                    var header = Instantiate(headerPrefab, multipleItemsParentTransform);
                    header.header.text = name;
                    var item = Instantiate(prefab, multipleItemsParentTransform);
                    items.Add(item);
                });
            }
            LanscapeDebugPanelView.Init(transform);
        }
#endif
    }
}