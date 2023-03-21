using FriendsGamesTools.UI;
using UnityEngine;

namespace FriendsGamesTools
{
    public class GDPRWindow : Window
    {
#if GDPR
        [SerializeField] private bool quitAppOnDeny = true;
        private static GDPRSettings settings => GDPRSettings.instance;
        private static GDPRWindow instance => Windows.Get<GDPRWindow>();

        public static void ShowIfNeeded()
        {
            if (instance?.shown ?? false)
                return;

            if (GDPRManager.conscentRequired && GDPRManager.state == GDPRState.NotSet)
                Show(settings.window.prefab);
        }

        public override void OnClosePressed()
        {
            GDPRManager.state = GDPRState.Accepted;
            base.OnClosePressed();
#if ECS_LEVEL_BASED
            if (!settings.openMainMenuOnClose) return;

            ECSGame.MainMenuWindow.Show();
#endif
        }

        public virtual void OnDenyPressed()
        {
            shown = false;

            if (quitAppOnDeny)
                Application.Quit();
            else
                GDPRManager.state = GDPRState.Declined;
        }
#endif
    }
}