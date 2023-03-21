#if ECSGame
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class EntityView : MonoBehaviour
    {
        List<IEntityView> views = new List<IEntityView>();
        List<IEntityLateUpdateView> lateViews = new List<IEntityLateUpdateView>();
        protected virtual void Awake()
        {
            views = gameObject.GetComponents<Component>().ConvertAll(c => c as IEntityView).FilterNulls();
            lateViews = gameObject.GetComponents<Component>().ConvertAll(c => c as IEntityLateUpdateView).FilterNulls();
        }
        public virtual void UpdateView(Entity e) {
            this.e = e;
            views.ForEach(view => view.UpdateView(e));
        }
        public virtual void LateUpdateView(Entity e)
        {
            lateViews.ForEach(view => view.LateUpdateView(e));
        }
        public Entity e { get; private set; }
    }
    public interface IEntityView { void UpdateView(Entity e); }
    public interface IEntityLateUpdateView { void LateUpdateView(Entity e); }
}
#endif