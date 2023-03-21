using FriendsGamesTools.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class ConfigDebugView : FGTModuleDebugPanel
    {
        public static ConfigDebugView instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = this;
            if (!Application.isPlaying) return;
            UpdateView();
        }
        public override string tab => "Config";
        public override string module => "DEBUG_CONFIG";
        public override bool wholeTab => true;
        [SerializeField] Button tabButtonPrefab;
        [SerializeField] ConfigEditorFromMonoBeh tabPrefab;
        [SerializeField] TabView tabsView;
        [SerializeField] Transform tabsParent;
#if DEBUG_CONFIG
        List<ConfigEditorFromMonoBeh> configEditors = new List<ConfigEditorFromMonoBeh>();
        void UpdateView()
        {
            tabsParent.DestroyChildren();
            tabsView.Clear();
            configEditors.Clear();
            BalanceSettings.instances.SortBy(i => i.ToString());
            BalanceSettings.instances.ForEach(inst =>
            {
                var tab = Instantiate(tabPrefab, tabsParent);
                FillParentRect.Fill(tab.transform.GetComponent<RectTransform>());
                var name = ReflectionUtils.GetProperty<string>(inst, "tabName");
                tabsView.AddTab(name, tab.gameObject, tabButtonPrefab);
                tab.SetConfig(inst);
                configEditors.Add(tab);
            });
            tabsView.SetShownTab(tabsView.startTabInd);
            LanscapeDebugPanelView.Init(transform);
        }
        public void UpdateConfigValuesView() => configEditors.ForEach(c => c.Show());
#else
        void UpdateView() { }
#endif
    }
}
