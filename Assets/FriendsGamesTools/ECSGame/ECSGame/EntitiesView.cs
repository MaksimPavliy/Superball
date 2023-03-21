#if ECSGame
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public abstract class EntitiesView<TFlag, TView, TSelf> : ItemsView<TFlag, TView, TSelf>
        where TFlag : struct, IComponentData
        where TView : EntityView
        where TSelf : ItemsView<TFlag, TView, TSelf>
    {
        [SerializeField] bool hasLateUpdate;
        bool isLateUpdate;
        private void LateUpdate()
        {
            if (!hasLateUpdate) return;
            isLateUpdate = true;
            Update();
            isLateUpdate = false;
        }
        protected override void OnCreateView(Entity newEntity, TView newView) {
            newView.UpdateView(newEntity);
            newView.LateUpdateView(newEntity);
        }
        protected override void UpdateView(Entity e, TView view)
        {
            if (!isLateUpdate)
                view.UpdateView(e);
            else
                view.LateUpdateView(e);
        }
    }
}
#endif