#if APP_LOVIN
using FriendsGamesTools.Ads;
using System;
using System.Collections;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class AppLovinRewarded : IRewardedVideoSource
    {
        Action<bool> onRewardedVideoCompleted;
        public bool available => !isShowing && (AppLovinManager.instance.editorTestModeEnabled || AppLovin.IsIncentInterstitialReady());
        public bool isShowing { get; private set; }
        public void Init() => StartLoading();
        public void Show(Action<bool> onCompleted)
        {
            isShowing = true;
            if (AppLovinManager.instance.editorTestModeEnabled)
            {
                AppLovinManager.instance.StartCoroutine(ExecuteAfterDelay(1, () =>
                {
                    isShowing = false;
                    onCompleted(true);
                }));
                return;
            }
            if (!AppLovin.IsIncentInterstitialReady())
            {
                isShowing = false;
                onCompleted(false);
                return;
            }
            onRewardedVideoCompleted = onCompleted;
            AppLovin.ShowRewardedInterstitial();
        }
        public void StartLoading() => AppLovin.LoadRewardedInterstitial();

        IEnumerator ExecuteAfterDelay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        private void OnRewardedVideoCompleted(bool success)
        {
            isShowing = false;
            onRewardedVideoCompleted?.Invoke(true);
            onRewardedVideoCompleted = null;
        }
        public void RewardedVideoAppLovinEventReceived(string eventName)
        {
            // The format would be "REWARDAPPROVEDINFO|AMOUNT|CURRENCY"
            if (eventName.Contains("REWARDAPPROVEDINFO"))
            {
                //// For this example, assume the event is "REWARDAPPROVEDINFO|10|Coins"
                //var delimeter = '|';
                //var split = eventName.Split(delimeter);
                //double amount = double.Parse(split[1]);
                //string currencyName = split[2];
                OnRewardedVideoCompleted(true);
            }
            else if (eventName.Contains("LOADEDREWARDED"))
            {
                // A rewarded video was successfully loaded.
            }
            else if (eventName.Contains("LOADREWARDEDFAILED"))
            {
                // A rewarded video failed to load.
            }
            else if (eventName.Contains("HIDDENREWARDED"))
            {
                // A rewarded video has been closed.  Preload the next rewarded video.
                StartLoading();
                OnRewardedVideoCompleted(false);
            }
        }
    }
}
#endif