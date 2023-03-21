namespace HcUtils
{
    public class SuperballAdButtonBlocker : AdButtonBlocker
    {
#if ADS
        protected override bool GameLoaded => SuperballAdsManager.instance;
        protected override bool RewardedAvailable => SuperballAdsManager.instance.rewarded.available;
#endif
    }
}