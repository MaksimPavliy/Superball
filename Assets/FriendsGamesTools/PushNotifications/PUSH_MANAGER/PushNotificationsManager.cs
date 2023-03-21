using NotificationSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace FriendsGamesTools.PushNotifications
{
    // Android push notifications guidelines and conceptual explanations:
    // https://developer.android.com/guide/topics/ui/notifiers/notifications
    // When not to use notifications (also android's guide):
    // https://material.io/design/platform-guidance/android-notifications.html#usage
    // Unity's own notification doc:
    // https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.0/manual/index.html
    // Unity's own notification wrapper around ios and android notifications:
    // https://github.com/Unity-Technologies/NotificationsSamples
    public partial class PushNotificationsManager : MonoBehaviour
    {
        [SerializeField] GameNotificationsManager _manager;
        [SerializeField] bool initDefaultChannel = true;
        [SerializeField] bool loggingInEditor = true;
#if PUSH_MANAGER

        public static PushNotificationsManager instance { get; private set; }
        public static GameNotificationsManager manager => instance?._manager;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        
        static bool logging => !Application.isEditor || instance.loggingInEditor;
        void Initialize()
        {
            if (initDefaultChannel)
            {
                const string ChannelId = "main_notifications_channel";
                const string ChannelTitle = "Main notifications";
                const string ChannelDescription = "Important game events";
                var channel = new GameNotificationChannel(ChannelId, ChannelTitle, ChannelDescription);
                manager.Initialize(channel);
            }
            else
                manager.Initialize();
        }
        public static int Send(string title, string desc, DateTime when)
        {
            var notification = manager.CreateNotification();
            if (notification != null)
            {
                notification.Title = title;
                notification.Body = desc;
                notification.DeliveryTime = when;
                if (logging)
                    Debug.Log($"Sent notification title = {title}, desc={desc}, time={when.ToLongTimeString()}");
                return manager.ScheduleNotification(notification).Notification.Id.Value;
            }
            else
            {
                if (logging)
                    Debug.Log("Cant send notifications on this platform");
                return -1;
            }
        }

        public event Action onScheduleNotifications;
        void OnApplicationPause(bool pause)
        {
            //if (logging)
            //Debug.Log($"OnApplicationPause = {pause}");
            if (!pause)
                return;
            ScheduleNotifications();
        }
        private void ScheduleNotifications()
        {
            onScheduleNotifications?.Invoke();
            configs.ForEach(config => config.Schedule());
        }
        public static void DebugScheduleNotifications() => instance.ScheduleNotifications();

        #region Permissions
        public enum Permission { NotDetermined = 0, Granted = 1, Denied = 2 }
#if UNITY_EDITOR
        const string PrefsKey = "PushPermission";
        public static Permission permission
        {
            get => (Permission)PlayerPrefs.GetInt(PrefsKey, 0);
            private set => PlayerPrefs.SetInt(PrefsKey, (int)value);
        }
#elif UNITY_IOS
        static iOSNotificationSettings iosSettings => iOSNotificationCenter.GetNotificationSettings();
        public static Permission permission
        {
            get
            {
                switch (iosSettings.AuthorizationStatus)
                {
                    case AuthorizationStatus.Authorized: return Permission.Granted;
                    case AuthorizationStatus.Denied: return Permission.Denied;
                    default:
                    case AuthorizationStatus.NotDetermined: return Permission.NotDetermined;
                }
            }
        }
#else
        public static Permission permission => Permission.Granted; // Android does not have that.
#endif
        public static async Task<bool> RequestPermission()
        {
#if UNITY_EDITOR
            var granted = await AskPermissionNativeWindow.Show();
            if (granted)
                permission = Permission.Granted;
            else
                permission = Permission.Denied;
            return granted;
#elif UNITY_IOS
            var all = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.CarPlay | AuthorizationOption.Sound;
            using (var req = new AuthorizationRequest(all, true))
            {
                while (!req.IsFinished)
                    await Awaiters.EndOfFrame;
                return req.Granted;
            }
#else
            return true; // android has permission by default.
#endif
        }
        #endregion

        #region Notifications config        
        List<NotificationConfig> configs = new List<NotificationConfig>();
        public static void AddNotificationConfig(string configId, Func<bool> getExists, 
            Func<string> getTitle, Func<string> getDesc, Func<DateTime> getWhen)
        {
            var config = new NotificationConfig {
                configId = configId, getExists = getExists,
                getTitle = getTitle, getDesc = getDesc, getWhen = getWhen
            };
            instance.configs.Add(config);
            config.Schedule();
        }
        #endregion

#endif
    }
}
