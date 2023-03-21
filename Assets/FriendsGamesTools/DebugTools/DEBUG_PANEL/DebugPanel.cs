using FriendsGamesTools.EditorTools.BuildModes;
using FriendsGamesTools.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public sealed class DebugPanel : MonoBehaviourHasInstance<DebugPanel>
    {
#if DEBUG_PANEL
        #region Open/close
        DebugPanelSettings settings => DebugPanelSettings.instance;
        int presses => settings.openPanelPresses;
        float pressesDuration => settings.openPanelPressDuration;
        List<float> pressTimes = new List<float>();
        public GameObject debugMenuParent;
        public PasswordView passwordView;
        bool passwordPassed;
        public bool shown {
            get => debugMenuParent.activeSelf;
            set {
                debugMenuParent.SetActive(value);
                if (value)
                    Pause();
                else
                    UnPause();
                if (!value)
                    lastClosingTime = Time.realtimeSinceStartup;
                else if (!BuildModeSettings.develop && settings.passwordIfNotDevelop && !passwordPassed)
                    passwordView.Show(OnPasswordEntered);
            }
        }
        void OnPasswordEntered(bool passwordCorrect) {
            if (!passwordCorrect)
                shown = false;
            else
                passwordPassed = true;
        }
        private void OnApplicationQuit() {
            if (shown)
                UnPause();
        }
        #region Time
        bool pausedWhenOpened;
        void Pause() {
            pausedWhenOpened = !GameTime.IsChangingSoft && !GameTime.paused;
            if (pausedWhenOpened)
                GameTime.SetPause(true);
        }
        void UnPause() {
            if (pausedWhenOpened && !GameTime.IsChangingSoft && GameTime.paused)
                GameTime.Unpause();
        }
        #endregion
        float lastClosingTime;
        const float SmallTime = 0.1f;
        public bool shownRecently => shown || Time.realtimeSinceStartup < lastClosingTime + SmallTime;
        void UpdateOpening() {
            if (!Input.GetMouseButtonUp(0))
                return;
            if (!(Input.mousePosition.x < Screen.width * 0.15f && Input.mousePosition.y > Screen.height * (1 - 0.15f)))
                return;
            pressTimes.Add(Time.realtimeSinceStartup);
            while (pressTimes.Count > presses)
                pressTimes.RemoveAt(0);
            if (pressTimes.Count >= presses && pressTimes[0] > Time.realtimeSinceStartup - pressesDuration)
            {
                pressTimes.Clear();
                shown = !shown;
            }
        }
        public void OnOpenPressed()
        {
            shown = true;
        }
        public void OnClosePressed()
        {
            shown = false;
        }
        #endregion

        #region Common
        bool startsShown => settings.startsShown;
        [SerializeField] Button closeButton;
        protected override void Awake()
        {
            base.Awake();
            shown = startsShown;
            if (closeButton != null)
                closeButton.onClick.AddListener(OnClosePressed);
            InitTabs();
        }
        void Update() => UpdateOpening();
        #endregion

        #region Tabs
        [SerializeField] TabView tabView;
        [SerializeField] Button tabButtonPrefab;
        [SerializeField] DebugPanelTabView tabPrefab;
        [SerializeField] Transform tabsParent;
        List<DebugPanelTabView> shownTabs = new List<DebugPanelTabView>();
        void InitTabs()
        {
            // Sort items by tabs.
            var itemsByTabs = new Dictionary<string, List<DebugPanelItemView>>();
            settings.itemViews.ForEach(item =>
            {
                if (item == null) return;
                var (tab, name) = item.whereToShow;
                if (settings.disabledModules.Contains(name))
                    return;
                if (!itemsByTabs.TryGetValue(tab, out var itemsInTab))
                {
                    itemsInTab = new List<DebugPanelItemView>();
                    itemsByTabs.Add(tab, itemsInTab);
                }
                itemsInTab.Add(item);
                item.OnDebugPanelAwake();
            });
            // Sort tabs.
            var itemsByTabsList = itemsByTabs.ConvertAll(i => i.Value);
            itemsByTabsList.SortBy(list => list.Max(item => -item.sortPriority));
            // Show tabs, but activate only one.
            tabsParent.gameObject.SetActive(false);
            Utils.UpdatePrefabsList(shownTabs, itemsByTabsList, tabPrefab, tabsParent, 
                (itemsInTab, tab) => tab.SetItems(itemsInTab));
            shownTabs.ForEach(tab => tab.gameObject.SetActive(false));
            tabsParent.gameObject.SetActive(true);
            for (int i = 0; i < itemsByTabsList.Count; i++)
            {
                var (tabName, _) = itemsByTabsList[i][0].whereToShow;
                tabView.AddTab(tabName, shownTabs[i].gameObject, tabButtonPrefab);
            }
            tabView.SetShownTab(tabView.startTabInd);
        }
        #endregion
#endif
    }
}
