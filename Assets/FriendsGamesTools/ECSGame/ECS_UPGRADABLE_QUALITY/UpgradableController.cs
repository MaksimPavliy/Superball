#if ECS_UPGRADABLE_QUALITY || ECS_UPGRADABLE_COUNT
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class UpgradableController : Controller
    {
        public virtual double GetPrice(Entity e) => -1;
        public bool HasPrice(Entity e) => HasPrice(GetPrice(e));
        public bool HasPrice(double price) => price >= 0;
        public abstract bool GetAvailable(Entity e);
    }
}
#endif