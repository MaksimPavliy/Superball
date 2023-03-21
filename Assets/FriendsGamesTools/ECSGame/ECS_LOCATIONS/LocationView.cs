#if ECS_LOCATIONS
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Locations
{
    public class LocationView : MonoBehaviour
    {
        /// <summary>
        /// Visual title of the level to show player. Optional.
        /// </summary>
        public virtual string locationName => transform.name;
        Dictionary<Type, ILocationBalanceSettings> locationBalance;
        public T GetBalance<T>() where T : DebugTools.BalanceSettings<T>
        {
            if (locationBalance == null) {
                locationBalance = new Dictionary<Type, ILocationBalanceSettings>();
                transform.GetInterfacesInChildren<ILocationBalanceSettings>(true).ForEach(item => locationBalance.Add(item.GetType(), item));
            }
            return locationBalance.TryGetValue(typeof(T), out var val) ? (T)val : null;
        }
        public virtual void OnLocationShown() {
        }
        protected virtual void Awake() { }
    }

#if DEBUG_CONFIG
    public abstract class LocationSettings<TSelf> : DebugTools.BalanceSettings<TSelf>, ILocationBalanceSettings
       where TSelf : LocationSettings<TSelf>
    {
        public static new TSelf instance => LocationsView.instance.GetBalance<TSelf>();
    }
    public interface ILocationBalanceSettings { }
#endif
}
#endif