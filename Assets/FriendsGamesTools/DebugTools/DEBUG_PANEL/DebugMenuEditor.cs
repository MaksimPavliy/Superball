using FriendsGamesTools.Ads;
using FriendsGamesTools.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using FriendsGamesTools.IAP;
using UnityEditor;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.DebugTools.ChromaKey;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways, Obsolete]
    public class DebugMenuEditor : MonoBehaviour
    {
        [SerializeField] List<GameObject> tabs;
        [SerializeField] Button tabButtonPrefab;
        [SerializeField] TabView tabsView;
        [SerializeField] Transform tabsParent;

        [SerializeField] LogsDebugView logsViewPrefab;
        [SerializeField] bool logs;

        [SerializeField] AdsDebugView adsPrefab;
        [SerializeField] bool ads;

        [SerializeField] IAPDebugView IAPPrefab;
        [SerializeField] bool iap;

        [SerializeField] ConfigDebugView ConfigPrefab;
        [SerializeField] bool config;

        [SerializeField] PerformanceDebugView ConfigPerformance;
        [SerializeField] bool performance;

        [SerializeField] ECSCommonDebugView ECSCommonPrefab;
        [SerializeField] bool ecs;

        [SerializeField] ChromaKeyDebugView ChromaKeyPrefab;
        [SerializeField] bool chromakey;

#if DEBUG_PANEL && UNITY_EDITOR
        private void Awake()
        {
            if (Application.isPlaying)
                Destroy(this);
        }
        long prevHash;
        private void Update()
        {
            if (!Utils.IsPrefabOpened())
                return;
            string prefabPath = "";
            prefabPath = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath;
            var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            if (prefabName != gameObject.name)
                return; // Dont manage, another prefab opened.
            if (prefabName == "DebugMenu")
                return; // Manage only variants.
            long currHash = 23412415;
#if DEBUG_LOGS
            currHash = currHash.ToHash(logs.ToHash());
#endif
#if ADS
            currHash = currHash.ToHash(ads.ToHash());
#endif
#if IAP
            currHash = currHash.ToHash(iap.ToHash());
#endif
#if DEBUG_CONFIG
            currHash = currHash.ToHash(config.ToHash());
#endif
#if DEBUG_PERFORMANCE
            currHash = currHash.ToHash(performance.ToHash());
#endif
#if ECS_GAMEROOT
            currHash = currHash.ToHash(ecs.ToHash());
#endif
#if CHROMA_KEY
            currHash = currHash.ToHash(chromakey.ToHash());
#endif
            tabs.ForEach(t => currHash = currHash.ToHash(t.GetHashCode()));
            if (prevHash == currHash)
                return;
            prevHash = currHash;
            UpdateDebugMenu();
        }
        
        void AddTab(GameObject tab) => tabsView.AddTab(tab.name, tab, tabButtonPrefab);
        void FitInParent(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = Vector2.one;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        private void UpdateDebugMenu()
        {
            tabsView.transform.DestroyChildrenImmediate();
            tabsView.tabButtons.Clear();
            tabsView.tabs.Clear();
            tabs.ForEach(tab => AddTab(tab));

#if DEBUG_LOGS
            UpdateLogsView();
#endif
#if ADS
            UpdateAdsView();
#endif
#if IAP
            UpdateIAPView();
#endif
#if DEBUG_CONFIG
            UpdateConfigView();
#endif
#if DEBUG_PERFORMANCE
            UpdatePerformanceView();
#endif
#if ECS_GAMEROOT
            UpdateECSView();
#endif
#if CHROMA_KEY
            UpdateChromaKeyView();
#endif
            tabsView.tabs.ForEachWithInd((tab, ind) => tab.SetActive(ind == tabsView.startTabInd));
            EditorUtility.SetDirty(gameObject);
        }
        void UpdateChildPrefabTab<T>(T prefab, bool tabEnabled, Action<T> onCreated = null, Action onDestroyed = null) where T : MonoBehaviour
        {
            var existingTab = transform.GetComponentInChildren<T>(true);
            if (tabEnabled)
            {
                if (existingTab == null)
                    existingTab = (T)PrefabUtility.InstantiatePrefab(prefab, tabsParent);
                existingTab.transform.name = prefab.name.Replace("DebugView", "");
                FitInParent(existingTab.gameObject);
                AddTab(existingTab.gameObject);
                onCreated?.Invoke(existingTab);
            }
            else
            {
                if (existingTab != null)
                    DestroyImmediate(existingTab.gameObject);
                onDestroyed?.Invoke();
            }
        }

#if DEBUG_LOGS
        void UpdateLogsView() => UpdateChildPrefabTab(logsViewPrefab, logs);
#endif
        
#if ADS
        void UpdateAdsView() => UpdateChildPrefabTab(adsPrefab, ads);
#endif

#if IAP
        void UpdateIAPView() => UpdateChildPrefabTab(IAPPrefab, iap);
#endif

#if DEBUG_CONFIG
        void UpdateConfigView() => UpdateChildPrefabTab(ConfigPrefab, config);
#endif

#if DEBUG_PERFORMANCE
        void UpdatePerformanceView() => UpdateChildPrefabTab(ConfigPerformance, performance);
#endif

#if ECS_GAMEROOT
        void UpdateECSView() => UpdateChildPrefabTab(ECSCommonPrefab, ecs);
#endif

#if CHROMA_KEY
        void UpdateChromaKeyView() => UpdateChildPrefabTab(ChromaKeyPrefab, chromakey);
#endif

#endif
    }
}
