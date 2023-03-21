#if ADS
namespace FriendsGamesTools.Ads
{
    public interface IAdsSource
    {
        IRewardedVideoSource rewarded { get; }
        IInterstitialSource interstitial { get; }
        IBannerSource banner { get; }
        void SetUserConscent(bool accepted);
        bool conscentRequired { get; }
    }
}
#endif