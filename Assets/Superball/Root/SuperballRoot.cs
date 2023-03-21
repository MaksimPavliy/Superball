using FriendsGamesTools.ECSGame;
using FriendsGamesTools.PushNotifications;
using System;

namespace Superball
{
    public class SuperballRoot : GameRoot<SuperballRoot>
    {
        public SuperballLevelsController levels;
        public SuperballMoneyController money;
        public MoneySkinController moneySkin;
        public ProgressSkinController progressSkin;

        protected override void OnWorldInited()
        {
            base.OnWorldInited();

            MainMenuWindow.Show();
        }

        protected override void Start()
        {
            base.Start();

            SetPushNotification();
        }

        protected override void Update()
        {
            base.Update();

#if GDPR
            FriendsGamesTools.GDPRWindow.ShowIfNeeded();
#endif
        }

        private void SetPushNotification()
        {
            PushNotificationsManager.AddNotificationConfig(
                "after_game_time",
               () => true,
               () => "",
               () => "We miss you! :(",
               () => DateTime.Now.AddDays(1)
                );
        }
    }
}