#if PUSH_MANAGER
using System;
using UnityEngine;

namespace FriendsGamesTools.PushNotifications
{
    public partial class PushNotificationsManager
    {
        class NotificationConfig
        {
            public string configId;
            string hashId => configId + "_hash";
            public Func<bool> getExists;
            public Func<string> getTitle;
            public Func<string> getDesc;
            public Func<DateTime> getWhen;
            public void Schedule()
            {
                // Remove existing notification if needed.
                int id = PlayerPrefs.GetInt(configId, -1);
                var exists = getExists();
                if (id != -1 && !exists)
                {
                    RemoveNotification(id);
                    return;
                }
                int existingHash = PlayerPrefs.GetInt(hashId, -1);
                var title = getTitle().WithUTF32Support();
                var desc = getDesc().WithUTF32Support();
                var when = getWhen();
                int currHash = (int)((long)title.GetHashCode()).ToHash(desc.GetHashCode(), when.Ticks.GetHashCode());
                // Remove if needs update.
                if (id!=-1 && currHash!=existingHash)
                    RemoveNotification(id);
                if (currHash == existingHash)
                    return;
                // Add notification.
                id = PushNotificationsManager.Send(title, desc, when);
                PlayerPrefs.SetInt(configId, id);
                PlayerPrefs.SetInt(hashId, currHash);
            }
            void RemoveNotification(int id)
            {
                manager.CancelNotification(id);
                PlayerPrefs.SetInt(configId, -1);
            }
        }
    }
}
#endif