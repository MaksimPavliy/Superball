#if ECSGame
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

namespace FriendsGamesTools.ECSGame
{
    public abstract class ItemsView<TFlag, TView, TSelf> : MonoBehaviourHasInstance<TSelf>
        where TFlag : struct, IComponentData 
        where TView : Component
        where TSelf : ItemsView<TFlag, TView, TSelf>
    {
        public List<TView> prefabs;
        protected abstract void UpdateView(Entity e, TView view);
        protected virtual int GetPrefabInd(Entity e) => 0;
        protected virtual long GetVisualKey(Entity e) => e.Index.ToHash().ToHash(e.Version);
        Dictionary<long, TView> itemViews = new Dictionary<long, TView>();
        Dictionary<long, Entity> itemsData = new Dictionary<long, Entity>();
        Dictionary<int, Queue<TView>> poolByPrefabInd = new Dictionary<int, Queue<TView>>();
        protected virtual bool usePool => true;
        Dictionary<TView, int> prefabIndByView = new Dictionary<TView, int>();
        public TView GetView(Entity e) => itemViews.TryGetValue(GetVisualKey(e), out var view) ? view : null;
        public IEnumerable<TView> views => itemViews.Values;
        public void ForEachView(Action<TView> action) => itemViews.Values.ForEach(action);
        List<Entity> dataList = new List<Entity>();
        protected virtual Vector3 poolPosition => Vector3.one * 30000;
        protected virtual bool Filter(Entity e) => true;
        protected virtual void OnCreateView(Entity newEntity, TView newView) => UpdateView(newEntity, newView);
        protected virtual void OnDestroyView(TView oldView) { }
        protected virtual float GetDestroyingDuration(TView view) => -1;
        protected bool inited { get; private set; }
        protected virtual void Update()
        {
            //Profiler.BeginSample("GetAllEntitiesWith");
            itemsData.Clear();
            ECSUtils.GetAllEntitiesWith<TFlag>(dataList);
            //Profiler.EndSample();
            //Profiler.BeginSample("itemsData");
            foreach (var item in dataList)
            {
                if (item.Exists() && Filter(item))
                {
                    var visualKey = GetVisualKey(item);
                    if (itemsData.ContainsKey(visualKey))
                    {
                        // Smth went wrong, visual keys collided.
                        var otherData = itemsData[visualKey];
                        Debug.LogError($"ItemsView keys collision visualKey = {visualKey}, TFlag = {typeof(TFlag).Name}, " +
                            $"item1 = {otherData.Index}:{otherData.Version}, item2 = {item.Index}:{item.Version}");
                    }
                    itemsData.Add(visualKey, item);
                }
            }
            //Profiler.EndSample();
            //Profiler.BeginSample("UpdatePrefabsDictionary");
            Utils.UpdatePrefabsDictionary(itemViews, itemsData,
                entity =>
                {
                    var prefabInd = GetPrefabInd(entity);
                    TView view = null;
                    if (usePool && poolByPrefabInd.TryGetValue(prefabInd, out var currPrefabPool))
                    {
                        while (currPrefabPool.Count > 0 && view == null)
                            view = currPrefabPool.Dequeue();
                        if (view != null)
                        {
                            //view.gameObject.SetActive(true);

                        }
                    }
                    if (view == null)
                    {
                        var prefab = prefabs[prefabInd];
                        view = MonoBehaviour.Instantiate(prefab, transform);
                        prefabIndByView.Add(view, prefabInd);
                    }
                    OnCreateView(entity, view);
                    return view;
                }, view =>
                {
                    if (view == null)
                        return;
                    var destroyingDuration = GetDestroyingDuration(view);
                    if (destroyingDuration <= 0)
                        DestroyView(view);
                    else
                        destroyAfterTime.Add((view, destroyingDuration));
                    //view.gameObject.SetActive(false);
                }, UpdateView);
            UpdateDestroyAfterTime();
            //Profiler.EndSample();
            inited = true;
        }
        void DestroyView(TView view)
        {
            OnDestroyView(view);
            if (usePool)
            {
                var prefabInd = prefabIndByView[view];
                if (!poolByPrefabInd.TryGetValue(prefabInd, out var currPrefabPool))
                {
                    currPrefabPool = new Queue<TView>();
                    poolByPrefabInd.Add(prefabInd, currPrefabPool);
                }
                currPrefabPool.Enqueue(view);
                view.transform.position = poolPosition;//.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 30000;
            }
            else
            {
                Destroy(view.gameObject);
            }
        }
        List<(TView, float)> destroyAfterTime = new List<(TView, float)>();
        void UpdateDestroyAfterTime()
        {
            for (int i = destroyAfterTime.Count - 1; i >= 0; i--)
            {
                var (view, remaining) = destroyAfterTime[i];
                remaining -= Time.deltaTime;
                if (remaining > 0)
                    destroyAfterTime[i] = (view, remaining);
                else {
                    DestroyView(view);
                    destroyAfterTime.RemoveAt(i);
                }    
            }
        }
    }
}
#endif