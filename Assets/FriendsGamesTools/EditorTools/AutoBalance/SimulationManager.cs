#if AUTO_BALANCE
using FriendsGamesTools.ECSGame;
using System;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.AutoBalance
{
    public class SimulationManager
    {
        #region Singleton
        static SimulationManager _instance;
        public static SimulationManager instance => _instance ?? (_instance = new SimulationManager());
        #endregion

        #region Simulation state
        SimulationSettings config => SimulationSettings.instance;
        protected GameRoot root => GameRoot.instance;
        public bool isRunning { get; private set; }
        public bool finished => events.finished || loopsReachedMax || simTimeReachedMax;
        int currLoops;
        public bool loopsReachedMax => config.maxLoops > 0 && config.maxLoops <= currLoops;
        public float simulatedTime => root.time;
        public bool simTimeReachedMax => config.maxSimTime > 0 && config.maxSimTime <= simulatedTime;
        float currStartTime;
        float currStopTime;
        public float realTime => (isRunning ? Time.realtimeSinceStartup : currStopTime) - currStartTime;
        public float simulationSpeed => simulatedTime / realTime;
        #endregion

        #region Start/stop
        public void StartSimulation()
        {
            currLoops = 0;
            currEventsChecksCount = 0;
            events.ResetEvents();
            root.runningType = GameRunningType.Manual;
            root.loadOnAwakeInDebug = false;
            root.saveOnPauseOrExitInDebug = false;
            root.ResetWorld();
            ResetPlayer();
            isRunning = true;
            currStartTime = Time.realtimeSinceStartup;
        }
        public void CancelSimulation() => StopSimulation();
        void StopSimulation()
        {
            if (!isRunning) return;
            isRunning = false;
            currStopTime = Time.realtimeSinceStartup;
        }
        void Update()
        {
            if (!isRunning)
                return;
            for (int i = 0; i < config.loopsPerFrame; i++)
            {
                root.UpdateManual(config.loopDt);
                player.Update();
                var eventsChecksNeeded = (int)(root.time / config.eventsCheckInterval);
                if (eventsChecksNeeded > currEventsChecksCount)
                {
                    currEventsChecksCount++;
                    events.CheckEvents();
                }
                currLoops++;
            }
            if (finished)
                StopSimulation();
        }
        #endregion

        #region Player
        // Player can be overriden to any player, but default player is just ideal tapper.
        PlayerAI CreatePlayer() 
            => ReflectionUtils.CreateInstance<PlayerAI>(ReflectionUtils.GetTypeByName(config.selectedAIPlayerFullName, false, true));
        PlayerAI _player;
        PlayerAI player => _player ?? (_player = CreatePlayer());
        void ResetPlayer() => _player = null;
        #endregion

        #region Checking events
        public BalancedEvents CreateEvents()
            => ReflectionUtils.CreateInstance<BalancedEvents>(ReflectionUtils.GetTypeByName(config.selectedEventsFullName, false, true));
        BalancedEvents _events;
        public BalancedEvents events => _events ?? (_events = CreateEvents());
        int currEventsChecksCount;
        #endregion
    }
    public class BalancedEvents
    {
        public bool finished { get; internal set; }
        public void ResetEvents()
        {
            throw new NotImplementedException();
        }
        public void CheckEvents()
        {
            throw new NotImplementedException();
        }
    }
}
#endif