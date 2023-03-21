#if DEBUG_PERFORMANCE
using FriendsGamesTools.EditorTools.BuildModes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class PerformanceBudgetManager : MonoBehaviourSingleton<PerformanceBudgetManager>
    {
        DebugPerformanceSettings settings => DebugPerformanceSettings.instance;
        protected override void Awake()
        {
            base.Awake();
            gameObject.AddComponent<CPUTimeMeasuringFirstMonobehaviour>();
            gameObject.AddComponent<CPUTimeMeasuringLastMonobehaviour>();
        }
        private void OnEnable() => UpdateSimulatedPrefabs();
        private void Update()
        {
            if (!BuildModeSettings.develop || !settings.performanceBudgetTracking)
                return;
            UpdatePreabs();
            UpdateCPUPerformance();
        }
        const float PrefabsCheckInterval = 3;
        float remainingToCheckPrefabs;
        bool prefabsBudgetExceededErrorWritten;
        void UpdatePreabs()
        {
            if (prefabsBudgetExceededErrorWritten) return;
            remainingToCheckPrefabs -= Time.deltaTime;
            if (remainingToCheckPrefabs > 0)
                return;
            remainingToCheckPrefabs = PrefabsCheckInterval;
            foreach (var (prefabName, shownCount) in PerformancePrefab.shownCounts)
            {
                var budget = settings.GetBudget(prefabName);
                if (shownCount > budget)
                {
                    Debug.LogError($"prefab {prefabName} count is {shownCount} excedes budget {budget}. Performance problems inbound");
                    prefabsBudgetExceededErrorWritten = true;
                }
            }
        }

        #region CPU
        void UpdateCPUPerformance()
        {
            UpdateCPUPerformanceSimlation();
            UpdateErrorIfCPUBudgetExceeded();
        }
        bool cpuBudgetExceededErrorWritten;
        void UpdateErrorIfCPUBudgetExceeded()
        {
            if (cpuBudgetExceededErrorWritten) return;
            var cpuSpent = GetCPUItemsSpent();
            if (cpuSpent > settings.CPUPerformanceBudget)
            {
                cpuBudgetExceededErrorWritten = true;
                Debug.LogError($"CPU budget exceeded ({cpuSpent}/{settings.CPUPerformanceBudget})");
            }
        }
        System.Diagnostics.Stopwatch scriptsTotalSW = new System.Diagnostics.Stopwatch();
        public void OnFirstScriptStartedFrame()
        {
            scriptsTotalSW.Restart();
        }
        public void OnLastScriptFinishedFrame()
        {
            scriptsTotalSW.Stop();
            lastFrameScriptsElapsedMS = scriptsTotalSW.ElapsedMilliseconds;
            lastFramesScriptsElapsedMS.Add(lastFrameScriptsElapsedMS);
            while (lastFramesScriptsElapsedMS.Count > 100)
                lastFramesScriptsElapsedMS.RemoveAt(0);
            scriptsMSPerFrame = lastFramesScriptsElapsedMS.Sum() / (float)lastFramesScriptsElapsedMS.Count;
        }
        public static long lastFrameScriptsElapsedMS { get; private set; }
        List<long> lastFramesScriptsElapsedMS = new List<long>();
        public static float scriptsMSPerFrame { get; private set; }
        public static float GetCPUItemsSpent()
            => DebugPerformanceLocalSettings.CPUSpeedKnown ? 
            Mathf.RoundToInt(0.001f * scriptsMSPerFrame * (float)DebugPerformanceLocalSettings.CPUSpeed) : float.NaN;

        void UpdateCPUPerformanceSimlation()
        {
            if (settings.CPUPerformanceSimulatedSpending <= 0)
                return;
            simulationSW.Restart();
            for (int i = 0; i < settings.CPUPerformanceSimulatedSpending; i++)
                DoDummyCPUOperation();
            simulationSW.Stop();
            msPer1000CPUItems = 1000*simulationSW.ElapsedTicks/(double)System.Diagnostics.Stopwatch.Frequency 
                * (settings.CPUPerformanceSimulatedSpending / 1000d);
        }
        System.Diagnostics.Stopwatch simulationSW = new System.Diagnostics.Stopwatch();
        [SerializeField] double msPer1000CPUItems;
        [HideInInspector] public float dummyCount;
        private void DoDummyCPUOperation()
        {
            for (int i = 0; i < 100; i++)
            {
                dummyCount = Mathf.Sin(Mathf.Pow(Mathf.Sqrt(Utils.Random(10, 1000)), 2.4f));
                dummyCount = dummyCount * dummyCount + dummyCount * 32f / (dummyCount / 3f);
            }
        }
        public void CalcCPUSpeed()
        {
            // Calc dummy operations count per sec.
            simulationSW.Restart();
            var count = 0;
            while (simulationSW.ElapsedTicks < System.Diagnostics.Stopwatch.Frequency)
            {
                DoDummyCPUOperation();
                count++;
            }
            simulationSW.Stop();
            DebugPerformanceLocalSettings.instance.cpuSpeed = count / (simulationSW.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency);
#if UNITY_EDITOR
            DebugPerformanceLocalSettings.instance.SaveInEditorPlayMode();
#endif
        }
        #endregion

        Dictionary<string, List<PerformancePrefab>> simulatedPrefabs = new Dictionary<string, List<PerformancePrefab>>();
        public void UpdateSimulatedPrefabs()
        {
            var cam = Camera.main;
            settings.prefabsBudget.ForEach(prefabSettings=> {
                if (!simulatedPrefabs.TryGetValue(prefabSettings.prefabName, out var simulatedList))
                {
                    simulatedList = new List<PerformancePrefab>();
                    simulatedPrefabs.Add(prefabSettings.prefabName, simulatedList);
                }
                while (simulatedList.Count> prefabSettings.simulatedCount)
                {
                    Destroy(simulatedList[0].gameObject);
                    simulatedList.RemoveAt(0);
                }
                var prefab = settings.performancePrefabsForSimulation.Find(p => p.prefabName == prefabSettings.prefabName);
                while (simulatedList.Count< prefabSettings.simulatedCount)
                    simulatedList.Add(Instantiate(prefab, cam.transform));
                simulatedList.ForEach(s
                    => s.transform.localPosition = (Vector3.forward + Random.insideUnitSphere * settings.prefabsSimulationDisp) * settings.prefabsSimulationDist);
            });
        }
    }
}
#endif