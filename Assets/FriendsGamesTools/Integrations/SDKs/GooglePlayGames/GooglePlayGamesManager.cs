#if GOOGLE_PlAY_GAMES
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class GooglePlayGamesManager : IntegrationManager<GooglePlayGamesManager>
    {
        void Start()
        {
#if UNITY_ANDROID
            //  ADD THIS CODE BETWEEN THESE COMMENTS

            // Create client configuration
            PlayGamesClientConfiguration config = new
                PlayGamesClientConfiguration.Builder()
                .Build();

            // Enable debugging output (recommended)
            PlayGamesPlatform.DebugLogEnabled = true;

            // Initialize and activate the platform
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            // END THE CODE TO PASTE INTO START
#endif
        }
        public static bool authed =>
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.localUser.authenticated;
#else
        false;
#endif
        public static void Authenticate(bool silent, Action<bool> onResponse)
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(onResponse, silent);
#else
        Debug.LogError("GooglePlayGamesManager works only on android");
        onResponse?.Invoke(false);
#endif
        }
    }
}
#elif SDKs
using System;

namespace FriendsGamesTools.Integrations
{
    public class GooglePlayGamesManager : IntegrationManager<GooglePlayGamesManager>
    {
        public static bool authed => false;
        public static void Authenticate(bool silent, Action<bool> onResponse) => onResponse?.Invoke(false);
    }
}
#endif
