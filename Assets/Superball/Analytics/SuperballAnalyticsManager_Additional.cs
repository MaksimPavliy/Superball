#if FACEBOOK
using Facebook.Unity;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Superball
{
    public partial class HCAnalyticsManager
    {
        private static float levelStartTime;
        private static bool isWatchedSent;

        private static bool internet => Application.internetReachability != NetworkReachability.NotReachable;

        private static void SendFB(string eventName, params (string key, object value)[] parameters)
        {
#if FACEBOOK
            var dict = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (var (key, value) in parameters)
                    dict.Add(key, value);
            }

            FB.LogAppEvent(eventName, null, dict);
#endif
        }

        private static void SendAM(string eventName, params (string key, object value)[] parameters)
        {
#if APPMETRICA
            var dict = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (var (key, value) in parameters)
                    dict.Add(key, value);
            }

            AppMetrica.Instance.ReportEvent(eventName, dict);
#endif
        }

        private static int LoadCounters(CountersType type)
        {
            return PlayerPrefs.GetInt(type.ToString());
        }

        private static void ResetCounters(CountersType type)
        {
            PlayerPrefs.SetInt(type.ToString(), 0);
        }

        public static void IncAndSaveCounters(CountersType type)
        {
            int current = LoadCounters(type);

            PlayerPrefs.SetInt(type.ToString(), ++current);
        }
    }

    public enum CountersType
    {
        level_count,
        continue_count
    }

    public enum EndGameType
    {
        win,
        lose
    }

    public enum NameInAnalytics
    {
        NONE
    }
}
