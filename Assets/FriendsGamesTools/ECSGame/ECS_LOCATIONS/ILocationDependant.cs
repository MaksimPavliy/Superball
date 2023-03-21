#if ECS_LOCATIONS

using System;

namespace FriendsGamesTools.ECSGame.Locations
{
    public interface ILocationSet { void OnLocationSet(int newLocationInd); }
    public interface ILocationChanged { void OnLocationChanged(int newLocationInd); }
    public interface ILocationPlay { void OnLocationPlay(); }
    public interface ILocationLeave { void OnLocationLeave(); }
}
#endif