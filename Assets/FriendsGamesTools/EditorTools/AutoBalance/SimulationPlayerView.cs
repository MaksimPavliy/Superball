#if AUTO_BALANCE
using FriendsGamesTools.DebugTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.EditorTools.AutoBalance
{
    public class SimulationPlayerView : DebugPanelItemView
    {
        public const string Tab = "AutoBalance"; 
        public override (string tab, string name) whereToShow => (Tab, "simulation player");

        SimulationManager simulation => SimulationManager.instance;

        [SerializeField] Button startSimulatingButton;
        [SerializeField] Button cancelSimulatingButton;
        [SerializeField] TextMeshProUGUI elapsedRealTime;
        [SerializeField] TextMeshProUGUI elapsedSimulatedTime;
        [SerializeField] TextMeshProUGUI simulationSpeedLabel;

        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            startSimulatingButton.onClick.AddListener(OnStartSimulationPressed);
            cancelSimulatingButton.onClick.AddListener(OnCancelSimulationPressed);
            UpdateView();
        }
        protected override void UpdatePlaying()
        {
            base.UpdatePlaying();
            if (simulation.isRunning)
                UpdateView();
        }
        private void OnStartSimulationPressed()
        {
            simulation.StartSimulation();
            UpdateView();
        }
        private void OnCancelSimulationPressed()
        {
            simulation.CancelSimulation();
            UpdateView();
        }
        void UpdateView()
        {
            startSimulatingButton.gameObject.SetActive(!simulation.isRunning);
            cancelSimulatingButton.gameObject.SetActive(simulation.isRunning);
            elapsedRealTime.text = simulation.realTime.ToShownTime();
            elapsedSimulatedTime.text = simulation.simulatedTime.ToShownTime();
            simulationSpeedLabel.text = $"x{(int)simulation.simulationSpeed}";
        }
    }
}
#endif