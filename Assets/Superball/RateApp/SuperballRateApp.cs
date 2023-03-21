using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Locations;
using FriendsGamesTools.UI;
using Unity.Entities;

namespace Superball
{
    public struct RateUsData : IComponentData
    {
        public int showTimes;
        public float lastShownTime;
    }

    public class SuperballRateApp : Controller
    {
        private Entity entity => ECSUtils.GetSingleEntity<RateUsData>(true);
        private RateUsData data => ECSUtils.GetSingleComponentData<RateUsData>();

        private GameTimeController TimeController => root.Get<GameTimeController>();

        public override int updateEvery => 30;

        private float timeFromLastShown => TimeController.totalTime - data.lastShownTime;
        private bool LastGameIsWin => root.Get<SuperballLevelsController>().LastGameIsWin; //set it manually on each win or lose

        public bool ToShow => false;//IsRateUsToShowAzur();
        public override void InitDefault()
        {
            base.InitDefault();
            CreateEntity();
        }

        public override void OnInited()
        {
            base.OnInited();
            if (entity == Entity.Null)
                CreateEntity();
            
            //SetDebugTime(300);
        }

        public void SetDebugTime(int time) =>
            ECSUtils.GetSingleEntity<GameTimeData>().ModifyComponent((ref GameTimeData gt) => gt.totalOnlineTime = time);

        private void CreateEntity()
        {
            ECSUtils.CreateEntity(new RateUsData
            {
                showTimes = 0,
                lastShownTime = 0,
            });
        }

     

        public bool IsRateUsToShowAzur()
        {
            if (!LastGameIsWin) return false;

            float secondsFor1stShow = 300;
            float secondsFor2ndShow = 86400; //24 hours

            if (data.showTimes >= 3) return false;
            if (TimeController.totalOnlineTime < secondsFor1stShow) return false;
            if (data.showTimes > 0 && timeFromLastShown < secondsFor2ndShow) return false;
            return true;
        }

        public void IncrementShown() => entity.ModifyComponent((ref RateUsData d) =>
        {
            d.showTimes++;
            d.lastShownTime = TimeController.totalTime;
        });

        public void NeverShowAgain() => entity.ModifyComponent((ref RateUsData d) =>
        {
            d.showTimes = 3;
        });

    }
}