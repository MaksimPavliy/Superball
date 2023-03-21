#if GA
using GameAnalyticsSDK;
#endif
using UnityEngine;

namespace FriendsGamesTools.Analytics
{
    public class AnalyticsEventManager : MonoBehaviourHasInstance<AnalyticsEventManager>
    {
        protected bool isWatchedSent;
        protected bool internet => Application.internetReachability != NetworkReachability.NotReachable;
        protected string internetStr => internet ? "available" : "not_available";

        public virtual void LevelStart(string levelNumber)
        {
#if GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelNumber);
#endif
        }

        public virtual void LevelFailed(string levelNumber)
        {
#if GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelNumber);
#endif
        }

        public virtual void LevelFinish(string levelNumber)
        {
#if GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelNumber);
#endif
        }

        public virtual void OnAdPressed(string type, string placement, bool available)
        {
#if FIREBASE
            Send_Firebase("video_ads_available",
                 new Parameter("ad_type", type),
                 new Parameter("placement", placement),
                 new Parameter("result", available ? "success" : "not_available"),
                 new Parameter("connection", internetStr));

            if (available)
            {
                Send_Firebase("video_ads_started",
                    new Parameter("ad_type", type),
                    new Parameter("placement", placement),
                    new Parameter("result", "start"),
                    new Parameter("connection", internetStr));
            }

            isWatchedSent = false;
#endif
        }

        public virtual void OnAdShowingFinished(string type, string placement, string state)
        {
#if FIREBASE
            if (isWatchedSent) return;

            Send_Firebase("video_ads_watch",
                new Parameter("ad_type", type),
                  new Parameter("placement", placement),
                  new Parameter("result", state),
                  new Parameter("connection", internetStr));

            isWatchedSent = true;
#endif
        }
    }
}