using System.Text;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class PerformanceBudgetDebugView : MonoBehaviour
    {
        [SerializeField] ShowFPS fps;
        [SerializeField] DebugMonobehavioursPerformance scripts;
        [SerializeField] TextMeshProUGUI stateLabel;
        [SerializeField] TMP_InputField prefabNameInput;
        [SerializeField] TMP_InputField prefabSimulatedCountInput;
        [SerializeField] TMP_InputField prefabSimulatedDistanceInput;

#if DEBUG_PERFORMANCE
        DebugPerformanceSettings settings => DebugPerformanceSettings.instance;

        private void Awake()
        {
            if (!DebugPerformanceLocalSettings.CPUSpeedKnown)
                PerformanceBudgetManager.instance.CalcCPUSpeed();
        }
        void OnEnable() => UpdateShownState();
        StringBuilder sb = new StringBuilder();
        float GetFrameMS() => 1000f / fps.fpsFloat;
        void Update() {
            sb.Clear();
            sb.Append(shownStateString);
            sb.AppendLine($"fps = {fps.fps}, dt = {Mathf.RoundToInt(GetFrameMS())}ms");
            var CPUItemsSpent = PerformanceBudgetManager.GetCPUItemsSpent();
            sb.AppendLine($"CPU {CPUItemsSpent}/{settings.CPUPerformanceBudget} ({settings.CPUPerformanceSimulatedSpending})");
            stateLabel.text = sb.ToString();
        }
        string shownStateString;
        void UpdateShownState() {
            PerformanceBudgetManager.instance.UpdateSimulatedPrefabs();
            sb.Clear();
            sb.AppendLine($"prefab shown count/budget (simulated count)");
            foreach (var (prefabName, shownCount) in PerformancePrefab.shownCounts)
            {
                var (budget, simulatedCount) = settings.GetPrefabSettings(prefabName);
                sb.AppendLine($"{prefabName} {shownCount}/{budget} ({simulatedCount})");
            }
            sb.AppendLine($"CPU speed = {DebugPerformanceLocalSettings.CPUSpeed.ToStringWithSuffixes()} items/sec");
            shownStateString = sb.ToString();
        }
        
        public void OnApplySimulatedCountPressed()
        {
            var count = int.Parse(prefabSimulatedCountInput.text);
            if (prefabNameInput.text.ToUpper() == "CPU")
            {
                settings.CPUPerformanceSimulatedSpending = count;
            }
            else
            {
                var prefabSettings = settings.prefabsBudget.Find(p => p.prefabName == prefabNameInput.text);
                if (prefabSettings == default)
                {
                    Debug.LogError($"prefab {prefabNameInput.text} does not exist");
                    return;
                }
                prefabSettings.simulatedCount = count;
            }
            UpdateShownState();
        }
        public void OnSimulateAllBudgetPrefabs()
        {
            settings.prefabsBudget.ForEach(p => p.simulatedCount = p.budgetCount);
            settings.CPUPerformanceSimulatedSpending = settings.CPUPerformanceBudget;
            UpdateShownState();
        }
        public void OnApplySimulatedPrefabsDist()
        {
            settings.prefabsSimulationDist = float.Parse(prefabSimulatedDistanceInput.text);
            UpdateShownState();
        }
#endif
    }
}
