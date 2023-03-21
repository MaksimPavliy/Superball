#if ECS_LOCATIONS
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace FriendsGamesTools.ECSGame.Locations
{
    public abstract class LocationsView : MonoBehaviourHasInstance<LocationsView>
    {
        public LocationsController controller => GameRoot.instance?.Get<LocationsController>();
        protected virtual void Start()
        {
            UpdateView();
            GameRoot.instance.onWorldInited += Reset;
        }
        protected virtual void Update() => UpdateView();
        private int prevLocationInd = -2; // -1 is in menu.
        private bool prevCanChange = false;
        public virtual void Reset()
        {
            prevLocationInd = -2;
            prevCanChange = false;
        }
        private void UpdateView()
        {
            // Update location change possibiity view.
            var canChange = controller.canChange;
            if (prevLocationInd >= 0 && !prevCanChange && canChange)
                OnChangeLocationBecomeAvailable();
            prevCanChange = canChange;
            // Update location view.
            var currLocationInd = controller.currLocationInd;
            if (prevLocationInd == controller.currLocationInd)
                return;
            ShowLocation(currLocationInd);
            OnLocationShown();
            if (prevLocationInd != -2)
                OnLocationChanged(currLocationInd);
            prevLocationInd = currLocationInd;
        }
        public enum Mode { instantiateProjectPrefabs, showHideScenePrefabs }
        [SerializeField] protected Mode mode = Mode.instantiateProjectPrefabs;
        public abstract LocationView ShowLocation(int locationInd);
        protected abstract void OnLocationShown();
        protected virtual void OnLocationChanged(int newLocationInd) { }
        protected virtual void OnChangeLocationBecomeAvailable() { }
        public abstract int locationsCount { get; }
        protected abstract LocationView _currLocationView { get; }
        public abstract string LocationName { get; }
#if DEBUG_CONFIG
        public T GetBalance<T>() where T : DebugTools.BalanceSettings<T>
            => _currLocationView.GetBalance<T>();
#endif
    }
    public abstract class LocationsView<TLocationView, TChangeLocationWindow> : LocationsView
        where TLocationView : LocationView
        where TChangeLocationWindow : ChangeLocationsWindow
    {
        public static new LocationsView<TLocationView, TChangeLocationWindow> instance { get; private set; }
        public virtual TLocationView GetPrefab(int locationInd)
        {
            if (mode != Mode.instantiateProjectPrefabs)
                return null;
            var sourceLocationInd = controller.GetPrefabInd(locationInd);
            if (!locations.IndIsValid(sourceLocationInd))
                return null;
            return locations[sourceLocationInd];
        }

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            if (mode == Mode.instantiateProjectPrefabs)
                transform.DestroyChildrenImmediate();
            else {
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        public List<TLocationView> locations;
        public override int locationsCount => locations.Count;
        public TLocationView shownLocationView { get; private set; }
        protected override LocationView _currLocationView => currLocationView;
        public TLocationView currLocationView
            => controller == null ? null : (mode == Mode.instantiateProjectPrefabs ? GetPrefab(controller.currLocationInd) : locations[controller.currSourceLocationInd]);
        public override LocationView ShowLocation(int locationInd)
        {
            if (locations == null || locations.Count == 0)
                return null;
            shownLocationView = null;
            if (mode == Mode.instantiateProjectPrefabs) {
                transform.DestroyChildrenImmediate();
                var prefab = GetPrefab(locationInd);
                if (prefab != null)
                    shownLocationView = Instantiate(prefab, transform);
            }
            else {
                for (int i = 0; i < locations.Count; i++) {
                    locations[i].transform.SetDefaultLocalPosition();
                    locations[i].gameObject.SetActive(i == locationInd);
                    if (i == locationInd)
                        shownLocationView = locations[i];
                }
            }
            return shownLocationView;
        }
        public override string LocationName => currLocationView?.locationName;
        protected override void OnLocationShown() => shownLocationView?.OnLocationShown();
        protected override void OnChangeLocationBecomeAvailable() => ShowChangeLocationWindow();
        public virtual void ShowChangeLocationWindow() => ChangeLocationsWindow.Show<TChangeLocationWindow>();
    }
}
#endif