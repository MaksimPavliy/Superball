using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class TabView : MonoBehaviour
    {
        public List<HorizontalLayoutGroup> tabButtonLines;
        public List<Button> tabButtons;
        public List<GameObject> tabs;
        public int startTabInd;
#if UI
        int tabInd;
        private void Awake()
        {
            for (int i = 0; i < tabButtons.Count; i++)
            {
                var ind = i;
                tabButtons[i].onClick.AddListener(() => SetShownTab(ind));
            }
            SetShownTab(startTabInd);
        }

        public void Clear()
        {
            if (tabButtonLines.Count == 0)
                transform.DestroyChildren();
            else
                tabButtonLines.ForEach(t => t.transform.DestroyChildren());
            tabButtons.Clear();
            tabs.Clear();
        }

        public void SetShownTab(int tabInd)
        {
            this.tabInd = tabInd;
            for (int i = 0; i < tabButtons.Count; i++)
            {
                var selected = i == tabInd;
                tabs[i].SetActive(selected);
                tabButtons[i].interactable = !selected;
            }
        }

        public void AddTab(string name, GameObject tab, Button tabButtonPrefab)
        {
            tab.name = name;
            var newTabInd = tabs.Count;
            var tabButton = Instantiate(tabButtonPrefab);
            tabButton.onClick.AddListener(() => SetShownTab(newTabInd));
            tabButton.name = $"{name}TabButton";
            tabButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            tabButtons.Add(tabButton);
            tabs.Add(tab);
            UpdateButtonsParenting();
        }
        void UpdateButtonsParenting()
        {
            if (tabButtonLines.Count == 0)
            {
                tabButtons.ForEach(t =>
                {
                    t.transform.SetParent(transform);
                    t.transform.localScale = Vector3.one;
                    t.transform.localRotation = Quaternion.identity;
                    ZTo0(t.transform);
                });
                return;
            }

            var buttonsPerLine = Mathf.CeilToInt(tabButtons.Count / (float)tabButtonLines.Count);
            for (int i = 0; i < tabButtons.Count; i++)
            {
                var lineInd = i / buttonsPerLine;
                tabButtons[i].transform.SetParent(tabButtonLines[lineInd].transform);
                ZTo0(tabButtons[i].transform);
                tabButtons[i].transform.localScale = Vector3.one;
                tabButtons[i].transform.localRotation = Quaternion.identity;
            }
        }
        void ZTo0(Transform tr) => tr.localPosition = tr.localPosition.ZTo0();
#endif
    }
}
