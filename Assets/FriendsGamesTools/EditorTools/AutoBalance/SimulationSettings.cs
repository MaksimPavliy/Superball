#if AUTO_BALANCE

namespace FriendsGamesTools.EditorTools.AutoBalance
{
    public class SimulationSettings : SettingsScriptable<SimulationSettings>
    {
        public int maxLoops = -1;
        public float maxSimTime = -1;
        public float loopDt = 0.05f;
        public int loopsPerFrame = 100;
        public string selectedAIPlayerFullName = "FriendsGamesTools.EditorTools.AutoBalance.PlayerIdealTapper";
        public string selectedEventsFullName = "";
        public float eventsCheckInterval = 5;
    }
}
#endif