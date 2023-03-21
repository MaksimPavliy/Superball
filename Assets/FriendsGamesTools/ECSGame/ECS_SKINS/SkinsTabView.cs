using System.Collections.Generic;
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using UnityEngine;
#if ECS_SKINS
using Unity.Entities;
#endif

namespace FriendsGamesTools.ECSGame
{
    public abstract class SkinsTabView : MonoBehaviour
    {
        [SerializeField] Transform slotsParent;
        [SerializeField] SkinItemView slotPrefab;
#if ECS_SKINS
        public abstract string TabName { get; }
        public abstract string TabHint { get; }
        protected abstract SkinsController controller { get; }
        protected abstract IReadOnlyList<SkinViewConfig> viewConfigs { get; }
        protected virtual void Awake() { }
        List<SkinItemView> shownItems = new List<SkinItemView>();
        public virtual void UpdateView()
            => Utils.UpdatePrefabsList(shownItems, viewConfigs, slotPrefab, slotsParent, (viewConfig, view) => view.Show(viewConfig), true);
        public bool isActiveTab { get; private set; }
        public void SetActiveTab(bool isActiveTab) {
            this.isActiveTab = isActiveTab;
            UpdateView();
        }
        protected virtual void Update() { }
#endif
    }
#if ECS_SKINS
    public abstract class SkinsTabView<TData> : SkinsTabView
        where TData : struct, IComponentData
    {
        protected override SkinsController controller => GameRoot.instance.Get<SkinsController<TData>>();
        protected override IReadOnlyList<SkinViewConfig> viewConfigs => controller.viewConfigs;
    }
#endif
}
