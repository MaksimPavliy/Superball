#if ADS
using System;
using System.Threading.Tasks;

namespace FriendsGamesTools.Ads
{
    public interface IInterstitialSource
    {
        bool available { get; }
        void Show(Action onCompleted);
    }
    public class InterstitialSource : IInterstitialSource
    {
        Func<IInterstitialSource> getSDKSource;
        Action onAdShown, onAdHidden;
        public InterstitialSource(Func<IInterstitialSource> getSDKSource,
            Action onAdShown, Action onAdHidden)
        {
            this.getSDKSource = getSDKSource;
            this.onAdShown = onAdShown;
            this.onAdHidden = onAdHidden;
        }
        bool omit => AdsManager.instance.sourceTypeInterstitial == AdSourceType.omitShowingAndSuccess;
        public bool available => omit || ((getSDKSource()?.available ?? false) && !AdsManager.instance.isInSimulatedDelay && AdsManager.instance.interstitialWithTimerAvailable);
        public void Show(Action onCompleted = null)
        {
            if (omit || !available)
            {
                onCompleted?.Invoke();
                return;
            }
            onAdShown?.Invoke();
            getSDKSource().Show(() => {
                onAdHidden?.Invoke();
                onCompleted?.Invoke();
            });
        }

        public async Task Showing()
        {
            var completed = false;
            Show(() => completed = true);
            while (!completed)
                await Awaiters.EndOfFrame;
        }
    }
}
#endif