#if ADS
using System;
using System.Threading.Tasks;
using FriendsGamesTools.UI;

namespace FriendsGamesTools.Ads
{
    public interface IRewardedVideoSource
    {
        bool available { get; }
        void Show(Action<bool> onCompleted);
    }
    public class RewardedVideoSource : IRewardedVideoSource
    {
        Func<IRewardedVideoSource> getSDKSource;
        Action onAdShown;
        Action<bool> onAdHidden;
        public RewardedVideoSource(Func<IRewardedVideoSource> getSDKSource,
            Action onAdShown, Action<bool> onAdHidden)
        {
            this.getSDKSource = getSDKSource;
            this.onAdShown = onAdShown;
            this.onAdHidden = onAdHidden;
        }
        bool omit => AdsManager.instance.sourceTypeRewarded == AdSourceType.omitShowingAndSuccess;
        public bool available
            => omit || ((getSDKSource()?.available ?? false) && !AdsManager.instance.isInSimulatedDelay);
        public void Show(Action<bool> onCompleted)
        {
            if (!available)
            {
                MessageWindow.Show("No ad available, try again later");
                onCompleted?.Invoke(false);
                return;
            }
            if (omit)
            {
                onCompleted?.Invoke(true);
                return;
            }
            onAdShown?.Invoke();
            getSDKSource().Show(success=> {
                onAdHidden?.Invoke(success);
                onCompleted?.Invoke(success);
            });
        }
        public void Show(Action onSuccess)
            => Show(success=> {
                if (success)
                    onSuccess?.Invoke();
            });
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