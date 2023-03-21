#if ECS_INCOME_FOR_VIDEO
using FriendsGamesTools.ECSGame.Player.Money;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.IncomeForVideo
{
    public struct IncomeForVideoData : IComponentData { public bool ignored; }
    public abstract class IncomeForVideoController : Controller
    {
        public virtual bool available => true;
        public virtual bool active => ECSUtils.Count<IncomeForVideoData>() > 0;
        public virtual double multiplier => 2;
        public virtual float duration => 10;
        public virtual void AddMultiplier()
        {
            var e = PlayerMoneyController.instance.AddMultiplier(multiplier, duration);
            e.AddComponent(new IncomeForVideoData());
        }
    }
}
#endif