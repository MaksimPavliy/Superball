using System.Collections.Generic;
#if PUSH
using Unity.Notifications;
using Unity.Notifications.iOS;
using UnityEngine;
#endif

namespace FriendsGamesTools.PushNotifications
{
    public class PushNotificationsModuleFolder : ModulesFolder
    {
        public const string define = "PUSH";
        public override string Define => define;
        public override HowToModule HowTo() => new PushNotificationsHowTo();
        public override bool enabledWithoutDefine 
            => DefinesModifier.DefineExists(MobileNotificationsWrapperModule.define) 
            || DefinesModifier.DefineExists(ManagerModule.define);
        public override List<string> dependFromModules => base.dependFromModules.Adding(UIModule.define);

#if PUSH
        //iOSNotificationSettings settings => iOSNotificationCenter.GetNotificationSettings();
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            var changed = false;
            var RequestAuthorizationOnAppLaunch = NotificationSettings.iOSSettings.RequestAuthorizationOnAppLaunch;
            if (EditorGUIUtils.Toggle("ask permission on app start", ref RequestAuthorizationOnAppLaunch, ref changed, labelWidth: 180))
                NotificationSettings.iOSSettings.RequestAuthorizationOnAppLaunch = RequestAuthorizationOnAppLaunch;
            if (!RequestAuthorizationOnAppLaunch)
                EditorGUIUtils.RichMultilineLabel("You have to call <b>PushNotificationsManager.RequestPermission()</b> manually");
        }
#endif
    }
    public abstract class PushNotificationsModule : ModuleManager {
        public override string parentModule => PushNotificationsModuleFolder.define;
        public override bool hasDetailedView => false; 
    }
    public class MobileNotificationsWrapperModule : PushNotificationsModule
    {
        public const string define = "PUSH_WRAPPER";
        public override string Define => define;
        public override List<string> dependFromPackages 
            => base.dependFromPackages.Adding("com.unity.mobile.notifications");
    }
    public class ManagerModule : PushNotificationsModule
    {
        public const string define = "PUSH_MANAGER";
        public override string Define => define;
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(MobileNotificationsWrapperModule.define);
    }
}


