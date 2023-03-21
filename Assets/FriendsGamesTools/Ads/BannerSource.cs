#if ADS

using System;

namespace FriendsGamesTools.Ads
{
    public interface IBannerSource
    {
        bool available { get; }
        void Show();
        void Hide();
        void Destroy();
    }

    public class BannerSource : IBannerSource
    {
        private Func<IBannerSource> getSDKSource;

        public bool available => omit || ((getSDKSource()?.available ?? false) && !AdsManager.instance.isInSimulatedDelay);
        private bool omit => AdsManager.instance.sourceTypeBanner == AdSourceType.omitShowingAndSuccess;

        public BannerSource(Func<IBannerSource> getSDKSource)
        {
            this.getSDKSource = getSDKSource;
        }

        public void Show()
        {
            if (omit || !available) return;

            getSDKSource().Show();
        }

        public void Hide()
        {
            getSDKSource().Hide();
        }

        public void Destroy()
        {
            getSDKSource().Destroy();
        }
    }
}
#endif