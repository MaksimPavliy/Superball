#if QUESTS
using FriendsGamesTools.UI;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestWindow : Window
    {
        public static void Show() => Show<QuestWindow>();
        QuestsController controller => GameRoot.instance.Get<QuestsController>();
        public void OnClaimPressed()
        {
            if (!controller.claimAvailable) return;
            shown = false;
            controller.ClaimReward();
        }
    }
}
#endif