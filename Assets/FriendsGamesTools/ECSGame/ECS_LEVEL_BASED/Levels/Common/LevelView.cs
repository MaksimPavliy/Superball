#if ECS_LEVEL_BASED
using FriendsGamesTools.ECSGame.Locations;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class LevelView: LocationView
    {
        [SerializeField] string _locationName;
        public override string locationName
            => _locationName.IsNullOrEmpty() ? $"LEVEL {GameRoot.instance.Get<WinnableLocationsController>().currLocationInd + 1}" : _locationName;
    }
}
#endif