using FriendsGamesTools;
using FriendsGamesTools.Ads;

namespace Superball
{
    public partial class HCAnalyticsManager
    {
        public static void LevelStart(string levelNumber)
        {
            //SendFB("Level started", ("level number", levelNumber));

            //IncAndSaveCounters(CountersType.level_count);

            //LevelStartAM(levelNumber);

            TinySauce.OnGameStarted(levelNumber: levelNumber);
        }

        public static void LevelFailed(string levelNumber)
        {
            //SendFB("Level failed", ("level number", levelNumber));

            //LevelFinishAM(EndGameType.lose, levelNumber);

            TinySauce.OnGameFinished(false, 0, levelNumber: levelNumber);
        }

        public static void LevelFinish(string levelNumber, int winStarsCount)
        {
            //SendFB("Level finished", ("level number", levelNumber));

            //LevelFinishAM(EndGameType.win, levelNumber);

            TinySauce.OnGameFinished(true, winStarsCount, levelNumber: levelNumber);
        }

        private static void LevelStartAM(string levelNumber)
        {
            SendAM("level_start",
                ("level_count", LoadCounters(CountersType.level_count)),
                ("level_name", levelNumber),
                ("level_random", 1)
                );

            levelStartTime = GameTime.time;
        }

        private static void LevelFinishAM(EndGameType result, string levelNumber)
        {
            var duration = GameTime.time - levelStartTime;

            SendAM("level_finish",
                ("time", (int)duration),
                ("result", result.ToString()),
                ("level_count", LoadCounters(CountersType.level_count)),
                ("continue", LoadCounters(CountersType.continue_count)),
                ("level_name", levelNumber),
                ("level_random", 1),
                ("progress", 100)
                );

            if (EndGameType.win == result)
                ResetCounters(CountersType.continue_count);
        }

        public static void SkinBought(string ind) =>
            SendFB("Skin bought", ("skin index", ind));

        public static void OnAdPressed(AdType type, string placement, bool available)
        {
            SendAM("video_ads_available",
                  ("ad_type", GetAdType(type)),
                  ("placement", placement),
                  ("result", available ? "success" : "not_available"),
                  ("connection", internet));

            if (available)
            {
                SendAM("video_ads_started",
                    ("ad_type", GetAdType(type)),
                    ("placement", placement),
                    ("result", "start"),
                    ("connection", internet)
                    );
            }

            isWatchedSent = false;
        }

        public static void OnAdShowingFinished(AdType type, string placement, string state)
        {
            if (isWatchedSent) return;

            SendAM("video_ads_watch",
                  ("ad_type", GetAdType(type)),
                  ("placement", placement),
                  ("result", state),
                  ("connection", internet)
                  );

            isWatchedSent = true;
        }

        private static string GetAdType(AdType type)
        {
            return type == AdType.Interstitial ? "interstitial" : type == AdType.RewardedVideo ? "rewarded" : "NONE";
        }

        public static void OnRateUs(int result)
        {
            SendAM("rate_us",
               ("show_reason", "new_version"),
               ("rate_result", result));
        }
    }
}
