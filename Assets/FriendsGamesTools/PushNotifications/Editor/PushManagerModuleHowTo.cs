using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.PushNotifications
{
    public class PushNotificationsHowTo : HowToModule
    {
        public override string forWhat => "sending push notifications";
        protected override void OnHowToGUI()
        {
            manager.ShowOnGUI("take this prefab to your scene");
            EditorGUIUtils.RichMultilineLabel(
                $"Add <b>PushNotificationsManager.AddNotificationConfig</b> to any <b>Start()</b> method");
            GUILayout.Space(10);
            EditorGUIUtils.RichMultilineLabel("check if permissions granted by calling <b>PushNotificationsManager.permission</b>");
            //GUILayout.Label(
            //    "Works for IOS and android\n" + 
            //    "Notifications would be automatically rescheduled every time you loose focus in mobile application.\n"
            //    );

            GUILayout.BeginHorizontal();
            EditorGUIUtils.RichMultilineLabel($"use emojis like <b>0x04ad3</b> in notifications");
            GUIUtils.URL("UTF-32 emojis list", "https://unicode.org/emoji/charts/emoji-list.html");
            GUILayout.EndHorizontal();

        }
        protected override string docsURL => "https://drive.google.com/open?id=14tzPVvPKOuSYG2AjLCmkFp28boPF9LfdjZmmgONx75E";

        ExamplePrefab manager;
        public PushNotificationsHowTo()
        {
            //example = new ExampleScript("PushNotificationManagerExample");
            manager = new ExamplePrefab("PushNotificationsManager");
        }
    }
}


