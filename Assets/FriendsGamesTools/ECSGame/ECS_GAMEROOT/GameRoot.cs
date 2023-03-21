#if ECS_GAMEROOT
using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace FriendsGamesTools.ECSGame
{
    public abstract class GameRoot : FGTGameRoot
    {
        public static new GameRoot instance { get; private set; }
        protected EntityManager manager => World.Active.EntityManager;
        protected EntityManager.EntityManagerDebug debug => World.Active.EntityManager.Debug;
        List<int> framesToUpdate;
        List<Controller> _controllers;
        public IReadOnlyList<Controller> controllers => _controllers;
        private Dictionary<Type, Controller> controllersByType = new Dictionary<Type, Controller>();
        public T Get<T>() where T : Controller => Get(typeof(T)) as T;
        public Controller Get(Type t) => controllersByType.TryGetValue(t, out var inst) ? inst : default;
        List<ViewControllerBase> _views;
        public IReadOnlyList<ViewControllerBase> views => _views;
        private Dictionary<Type, ViewControllerBase> viewsByType = new Dictionary<Type, ViewControllerBase>();
        public T GetViewController<T>() where T : ViewControllerBase => viewsByType[typeof(T)] as T;
        protected abstract Type GetTSelf();

        private void CreateControllers()
        {
            List<Type> tControllers = new List<Type>();
            ReflectionUtils.GetAllDerivedTypes<Controller>()
                .Filter(t => t.CanCreateInstance())
                .ForEach(t => tControllers.Add(t));
            // Sort controllers using their 'update before' and 'update after' attributes.
            tControllers.SortPartialOrder((t1, t2) =>
            {
                if (t1.GetAllAttributeInstances<UpdateBeforeAttribute>().Any(t1Before => t1Before.SystemType.IsAssignableFrom(t2)))
                    return -1;
                if (t2.GetAllAttributeInstances<UpdateBeforeAttribute>().Any(t2Before => t2Before.SystemType.IsAssignableFrom(t1)))
                    return 1;
                if (t1.GetAllAttributeInstances<UpdateAfterAttribute>().Any(t1After => t1After.SystemType.IsAssignableFrom(t2)))
                    return 1;
                if (t2.GetAllAttributeInstances<UpdateAfterAttribute>().Any(t2After => t2After.SystemType.IsAssignableFrom(t1)))
                    return -1;
                return 0;
            });
            // Get classes.
            _controllers = new List<Controller>();
            controllersByType = new Dictionary<Type, Controller>();
            framesToUpdate = new List<int>();
            tControllers.ForEach(t =>
            {
                //var controller = (Controller)World.Active.GetExistingSystem(t);
                var controller = (Controller)World.Active.GetOrCreateSystem(t);
                _controllers.Add(controller);
                var type = controller.GetType();
                while (type != typeof(Controller))
                {
                    controllersByType[type] = controller;
                    type = type.BaseType;
                }
                framesToUpdate.Add(1);
            });
            // Setup properties.
            var members = GetTSelf().GetMembers();
            members.ForEach(m =>
            {
                var t = m.GetFieldPropertyType();
                if (t == null)
                    return;
                var inst = _controllers.Find(c => c.GetType() == t);
                if (inst != null)
                    m.SetValue(this, inst);
            });
        }
        private void CreateViews()
        {
            // Get views.
            List<Type> tViews = new List<Type>();
            if (ViewEnabled)
            {
                ReflectionUtils.GetAllDerivedTypes<ViewControllerBase>()
                    .Filter(t => t.CanCreateInstance())
                    .ForEach(t => tViews.Add(t));
            }
            _views = new List<ViewControllerBase>();
            viewsByType = new Dictionary<Type, ViewControllerBase>();
            tViews.ForEach(t =>
            {
                var view = (ViewControllerBase)t.CallStaticMethod("EnsureInstance");
                _views.Add(view);
                var type = view.GetType();
                while (type != typeof(ViewControllerBase))
                {
                    viewsByType[type] = view;
                    type = type.BaseType;
                }
            });
        }

        #region Initing
        public bool firstLaunch { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitWorld(() =>
            {
                if (!loadOnAwake || !Load()) // Load world, if save exists.
                    InitDefault();
                else
                    firstLaunch = false;
            });
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void OnBeforeWorldInited()
        {
            InitTime();
            Serialization.ClearWorld();
            CreateControllers();
            CreateViews();
        }
        protected virtual void InitWorld(Action setWorldData) // When world loaded or reset.
        {
            OnBeforeWorldInited();
            setWorldData();
            OnWorldInited();
        }
        public virtual void ResetWorld()
        {
            InitWorld(() => InitDefault());
        }
        protected virtual void InitDefault()
        {
            firstLaunch = true;
            Serialization.ClearWorld();
            _controllers.ForEach(u => u.InitDefault());
            if (ViewEnabled)
                _views.ForEach(v => v.InitDefault());
        }
        public event Action onWorldInited;
        protected virtual void OnWorldInited()
        {
            _controllers.ForEach(u => u.OnInited());
            if (ViewEnabled)
                _views.ForEach(v => v.OnInited());
            onWorldInited?.Invoke();
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            controllers.ForEach(c => (c as IOnSceneLoaded)?.OnSceneLoaded());
        }
#endregion

        protected virtual void Update()
        {
            UpdateAutoIfNeeded();
            UpdateAutoSave();

#if GDPR && UNITY_ANDROID
            FriendsGamesTools.GDPRWindow.ShowIfNeeded();
#endif
        }
        protected virtual void FixedUpdate() => controllers.ForEach(c => c.OnFixedUpdate(Time.fixedDeltaTime));
        protected virtual void OnApplicationQuit()
        {
            if (saveOnPauseOrExit) Save();
        }
        protected virtual void OnApplicationPause(bool paused)
        {
            if (saveOnPauseOrExit && paused) Save();
        }

#region Save/load
        public bool loadOnAwakeInDebug = true;
        public bool saveOnPauseOrExitInDebug = true;
        public bool loadOnAwake => loadOnAwakeInDebug || !BuildModeSettings.develop;
        public bool saveOnPauseOrExit => saveOnPauseOrExitInDebug || !BuildModeSettings.develop;
        public bool autosaveEnabled => saveOnPauseOrExit;
        public virtual void Save() => Serialization.SaveWorld();
        public bool SaveExists => Serialization.saveExists;
        protected virtual void OnNewVersion(int versionFrom, int versionTo) { }
        public bool Load()
        {
            if (!Serialization.TryLoadWorld(OnNewVersion))
                return false;
            return true;
        }
        protected virtual float autosaveInterval => 60;
        float remainingToAutoSave;
        void UpdateAutoSave()
        {
            if (!autosaveEnabled) return;
            if (autosaveInterval < 0) return;
            remainingToAutoSave -= GameTime.deltaTime;
            if (remainingToAutoSave > 0) return;
            remainingToAutoSave = autosaveInterval;
            Save();
        }
#endregion

#region Running & time        
        public GameRunningType runningType = GameRunningType.Auto;
        public bool ViewEnabled => runningType == GameRunningType.Auto;
        public float time { get; private set; }
        public float deltaTime { get; private set; }
        float _timeSpeed = 1;
        public virtual float timeSpeed
        { // real/game time.
            get => _timeSpeed;
            set
            {
                _timeSpeed = value;
                GameTime.UpdateUnityTimeSpeed();
            }
        }
        public bool paused { get; set; }
        void InitTime()
        {
            time = 0;
            deltaTime = 0;
            //Debug.Log("InitTime()");
            if (runningType == GameRunningType.Manual)
                UnityEngine.Debug.Assert(!loadOnAwake && !saveOnPauseOrExit, "Dont save when simulating game");
        }
        private void UpdatePlayerLoop(float deltaTime)
        {
            if (paused)
                return;
            this.deltaTime = deltaTime;
            time += deltaTime;
            //UnityEngine.Debug.Log($"UpdatePlayerLoop({deltaTime})");
            for (int i = 0; i < controllers.Count; i++)
            {
                framesToUpdate[i]--;
                if (framesToUpdate[i] > 0)
                    continue;
                var c = controllers[i];
                framesToUpdate[i] = c.updateEvery;
                Profiler.BeginSample(c.GetType().Name);
                c.Update();
                Profiler.EndSample();
            }
            if (ViewEnabled)
                _views.ForEach(v => v.OnUpdate());
        }
        public void UpdateManual(float deltaTime)
        {
            UnityEngine.Debug.Assert(runningType == GameRunningType.Manual);
            UpdatePlayerLoop(deltaTime);
        }
        private void UpdateAutoIfNeeded()
        {
            if (runningType == GameRunningType.Auto)
                UpdatePlayerLoop(UnityEngine.Time.deltaTime);
        }
        public void RunGame(float duration, float dt = 0.03f)
        {
            var prevType = runningType;
            runningType = GameRunningType.Manual;
            var remainingTime = duration;
            while (remainingTime > 0)
            {
                var currDt = Mathf.Min(remainingTime, dt);
                remainingTime -= currDt;
                UpdateManual(currDt);
            }
            runningType = prevType;
        }
#endregion
    }
    public abstract class GameRoot<TSelf> : GameRoot
        where TSelf : GameRoot<TSelf>
    {
        protected override Type GetTSelf() => typeof(TSelf);
        public static new TSelf instance { get; private set; }
        protected override void Awake()
        {
            instance = (TSelf)this;
            base.Awake();
        }
    }
    public enum GameRunningType { Auto, Manual }
}
#endif