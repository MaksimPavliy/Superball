#if ECS_LOCATIONS
using FriendsGamesTools.EditorTools.BuildModes;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Locations
{
    public abstract class LocationsController : Controller
    {
        public Entity e => ECSUtils.GetSingleEntity<LocationsData>();
        public abstract int locationsCount { get; }
        public int currLocationInd => e.GetComponentData<LocationsData>().currLocationInd;
        public bool isMaxLocation => !loop && locationsCount - 1 <= currLocationInd;
        /// <summary>
        /// If you repeat locations somehow, you can set repeating order here
        /// </summary>
        /// <param name="locationInd"></param> as seen by player
        /// <returns>source location to show to player</returns>
        public virtual int GetPrefabInd(int locationInd) => loop ? locationInd %= locationsCount : locationInd;
        public int currSourceLocationInd => GetPrefabInd(currLocationInd);
        public virtual bool loop => false;

        protected virtual int defaultLocationInd =>
#if !BUILD_MODES
            0;
#else
        BuildModeSettings.develop ? debugLocation : 0;
        protected virtual int debugLocation => 0;
#endif
        public override void InitDefault()
        {
            ECSUtils.CreateEntity(new LocationsData { });
            SetLocation(defaultLocationInd);
        }
        public virtual double GetMultiplier(int locationInd) => 1;
        public double multiplier => GetMultiplier(currLocationInd);
        public bool canChange => !isMaxLocation && canChangePrivate;
        protected abstract bool canChangePrivate { get; }
        public virtual bool ChangeLocation()
        {
            if (!canChange) return false;
            var newLocationInd = currLocationInd + 1;
            SetLocation(newLocationInd);
            return true;
        }
        public virtual void RestartLocation() => SetLocation(currLocationInd);
        public virtual void SetLocation(int newLocationInd)
        {
            var changed = currLocationInd != newLocationInd;
            e.ModifyComponent((ref LocationsData l) => l.currLocationInd = newLocationInd);
            CallLocationSet(newLocationInd);
            if (changed)
                CallLocationChanged(newLocationInd);
        }
        public virtual void DebugChangeLocation(int newLocationInd)
            => SetLocation(newLocationInd);
        public virtual void HideLocations() => SetLocation(-1);

        #region Callbacks
        void CallLocationChanged(int newLocationInd) => root.controllers.ForEach(c => {
            if (c is ILocationChanged l)
                l.OnLocationChanged(newLocationInd);
        });
        void CallLocationSet(int newLocationInd) {
            root.controllers.ForEach(c => {
                if (c is ILocationSet l)
                    l.OnLocationSet(newLocationInd);
            });
            root.views.ForEach(c => {
                if (c is ILocationSet l)
                    l.OnLocationSet(newLocationInd);
            });
        }
        #endregion
    }
    public class LocationsViewController : ViewController<LocationsViewController>, ILocationSet
    {
        public void OnLocationSet(int newLocationInd)
        {
            LocationsView.instance.Reset();
        }
    }
}
#endif