using System;
using System.Collections.Generic;

namespace FriendsGamesTools.DebugTools
{

    [Serializable]
    public class PerformancePrefabBudget
    {
        public string prefabName;
        public int budgetCount, simulatedCount;
    }
    public class DebugPerformanceSettings : SettingsScriptable<DebugPerformanceSettings>
    {
        public bool performanceBudgetTracking;
        public List<PerformancePrefabBudget> prefabsBudget = new List<PerformancePrefabBudget>();
        // Simulation.
        public long CPUPerformanceSimulatedSpending;
        public float prefabsSimulationDisp = 0.1f;
        public float prefabsSimulationDist = 10;
        public List<PerformancePrefab> performancePrefabsForSimulation = new List<PerformancePrefab>();
        public long CPUPerformanceBudget;

#if DEBUG_PERFORMANCE
        public int GetBudget(string prefabName) => GetPrefabSettings(prefabName).budget;
        public (int budget, int simulated) GetPrefabSettings(string prefabName)
        {
            var setting = prefabsBudget.Find(s => s.prefabName == prefabName);
            if (setting == null) return (0, 0);
            else return (setting.budgetCount, setting.simulatedCount);
        }
#endif
    }
}
