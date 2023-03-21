#if ECS_LEVEL_BASED
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Locations;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public struct Level : IComponentData
    {
        public enum State { inMenu, playing, win, lose }
        public State state;
        public int levelsPlayed;
    }
    public abstract class LevelBasedController : WinnableLocationsController
    {
        protected override bool canChangePrivate => data.state == Level.State.lose || data.state == Level.State.win;
        public override bool loop => true;
        protected virtual long levelsSalt => 12354;
        public override int GetPrefabInd(int locationInd)
        {
            if (locationInd < locationsCount) return locationInd;
            return Mathf.Abs((int)Utils.ToHash(locationInd + levelsSalt)) % locationsCount;
        }
    }
}
#endif