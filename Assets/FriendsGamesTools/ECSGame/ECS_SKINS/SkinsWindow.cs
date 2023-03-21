#if ECS_SKINS
using System.Collections.Generic;
using FriendsGamesTools;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class SkinsWindow : Window
    {
        [SerializeField] TextMeshProUGUI header;
        [SerializeField] TextMeshProUGUI hint;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] ScrollArrowsView scroll;
        [SerializeField] MoneySkinTabView moneyTab;
        [SerializeField] ProgressSkinTabView progressTab;
        List<SkinsTabView> tabs;
        public static void Show() => Show<SkinsWindow>();
        int activeTabIndex = 0;
        SkinsTabView activeTab => activeTabIndex < tabs.Count ? tabs[activeTabIndex] : null;
        protected virtual void OnEnable() => UpdateView();
        protected virtual void Awake()
        {
            tabs = new List<SkinsTabView>();
#if ECS_SKIN_MONEY
            tabs.Add(moneyTab);
            moneyTab.gameObject.SetActive(true);
#else
            moneyTab.gameObject.SetActive(false);
            moneyTab.unlockButton.gameObject.SetActive(false);
#endif
#if ECS_SKIN_PROGRESS
            tabs.Add(progressTab);
            progressTab.gameObject.SetActive(true);
#else
            progressTab.SetActiveSafe(false);
#endif

#if ECS_SKIN_MONEY && ECS_SKIN_PROGRESS
            const bool scrollEnabled = true;
#else
            const bool scrollEnabled = false;
#endif
            scroll.Safe(() => scroll.SetEnabled(scrollEnabled));
            if (scrollRect != null)
                scrollRect.onValueChanged.AddListener(scrollValue => UpdateView());
        }
        public virtual void UpdateView()
        {
            activeTabIndex = scrollRect != null ? Mathf.Clamp(Mathf.RoundToInt(scrollRect.horizontalNormalizedPosition * tabs.Count), 0, tabs.Count - 1) : 0;
            header.text = tabs.GetElementSafe(activeTabIndex)?.TabName;
            hint.SetTextSafe(activeTab?.TabHint);
            tabs.ForEachWithInd((tab, ind) => tab.SetActiveTab(ind == activeTabIndex));
        }
        public override void OnClosePressed() {
            base.OnClosePressed();
            MainMenuWindow.Show();
        }
    }
}
#endif