using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace FriendsGamesTools
{
    public static partial class Utils
    {
#if UNITY_EDITOR
        public static bool IsPrefabOpened() => PrefabStageUtility.GetCurrentPrefabStage() != null;
        public static bool PrefabChangesAllowed(GameObject go)
        {
            var opening = PrefabUtils.IsPrefabOpening;
            var opened = PrefabUtils.openedPrefab;
            var outer = UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            return !PrefabUtils.IsPrefabOpening &&
            (PrefabUtils.openedPrefab == go || UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(go) == null);
        }
    
#else
        public static bool PrefabChangesAllowed(GameObject go) => true;
        public static bool IsPrefabOpened() => false;
#endif
        public static void UpdatePrefabsList<TView, TModel>(List<TView> views, IReadOnlyList<TModel> data,
            TView prefab, Action<TModel, TView> updateView) where TView : MonoBehaviour
            => UpdatePrefabsList(views, data, prefab, null, updateView);
        public static void UpdatePrefabsList<TView, TModel>(List<TView> views, IReadOnlyList<TModel> data,
            TView prefab, Transform parent, Action<TModel, TView> updateView, bool findExistingItems = false) where TView : MonoBehaviour
            => UpdatePrefabsList(views, data.Count, prefab, parent, () => {
                for (int i = 0; i < data.Count; i++)
                    updateView(data[i], views[i]);
            }, findExistingItems);
        public static void UpdatePrefabsList<TView>(List<TView> views, int count,
            TView prefab, Transform parent, Action updateViews, bool findExistingItems = false) where TView : MonoBehaviour {
            if (parent == null)
                parent = prefab.transform.parent;
            if (findExistingItems && views.Count == 0)//prefab.transform.parent != null && parent == prefab.transform.parent && !views.Contains(prefab))
                parent.GetComponentsInChildren<TView>(true).ForEach(ch => views.AddIfNotExists(ch));
            if (prefab.transform.parent == parent)
                views.AddIfNotExists(prefab);
            for (int i = views.Count; i < count; i++)
                views.Add(MonoBehaviour.Instantiate(prefab, parent));
            for (int i = 0; i < count; i++)
                views[i].gameObject.SetActive(true);
            updateViews?.Invoke();
            for (int i = count; i < views.Count; i++)
                views[i].gameObject.SetActive(false);
        }
        static List<long> toRemove = new List<long>();
        public static void UpdatePrefabsDictionary<TView, TModel>(
            Dictionary<long, TView> views, Dictionary<long, TModel> data,
            Func<TModel, TView> getPrefab, Transform parent, Action<TModel, TView> updateView) where TView : Component
        {
            UpdatePrefabsDictionary(views, data, model =>
                MonoBehaviour.Instantiate(getPrefab(model), parent),
                view => MonoBehaviour.Destroy(view.gameObject),
                updateView);
        }
        public static void UpdatePrefabsDictionary<TView, TModel>(
            Dictionary<long, TView> views, Dictionary<long, TModel> data,
            Func<TModel, TView> createView, Action<TView> destroyView, Action<TModel, TView> updateView) where TView : Component
        {
            toRemove.Clear();
            foreach (var (key, view) in views)
            {
                if (!data.TryGetValue(key, out var model) || view == null)
                {
                    destroyView?.Invoke(view);
                    toRemove.Add(key);
                } else
                    updateView(model, view);
            }
            if (toRemove != null)
            {
                foreach (var key in toRemove)
                    views.Remove(key);
            }
            foreach (var (key, model) in data)
            {
                if (views.ContainsKey(key))
                    continue;
                var view = createView(model);
                views.Add(key, view);
                updateView(model, view);
            }
        }
        public static void DestroyEditOrPlayMode(this GameObject go)
        {
            if (Application.isPlaying)
                MonoBehaviour.Destroy(go);
            else
                MonoBehaviour.DestroyImmediate(go);
        }
    }

#if UNITY_EDITOR
    public static class PrefabUtils
    {
        public static void SetPrefabDirty()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }

        public static void InitOnLoad() => PrefabStage.prefabStageOpened += stage =>
        {
            if (FGTLocalSettings.instance != null)
                FGTLocalSettings.instance.openedPrefab = stage?.prefabContentsRoot;
        };
        public static bool IsPrefabOpening => openedPrefab != PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot;
        public static GameObject openedPrefab => FGTLocalSettings.instance?.openedPrefab;
        public static T InstantiatePrefab<T>(UnityEngine.Object prefab,Transform parent) where T:class
        {
            return PrefabUtility.InstantiatePrefab(prefab, parent) as T;
        }
        public static void Open(GameObject prefab)
        {
            if (prefab != null)
                AssetDatabase.OpenAsset(prefab);
            else
            {
                var scene = EditorSceneManager.GetActiveScene();
                if (scene != null)
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
            }
        }
    }
#else
    public static class PrefabUtils
    {
        public static bool IsPrefabOpening => false;
        public static GameObject openedPrefab => null;
        public static void SetPrefabDirty(){}
        public static T InstantiatePrefab<T>(UnityEngine.Object prefab,Transform parent) where T:class
        {
            return null;
        }
    }
#endif
}