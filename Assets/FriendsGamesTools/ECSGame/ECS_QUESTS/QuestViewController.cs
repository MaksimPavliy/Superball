#if QUESTS
using FriendsGamesTools.UI;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestViewController : ViewController<QuestViewController>
    {
        QuestsController controller => root.Get<QuestsController>();
        public Quest currQuest => controller.currQuest;
        public override void OnInited()
        {
            QuestsView.HideArrow();
        }
        public override void OnUpdate()
        {
            var controller = this.controller;
            if (controller == null) return;
            if (currQuest != null && !controller.claimAvailable)
                currQuest.UpdateActive();
            else if (controller.claimAvailable && !Windows.anyShown)
                QuestsView.ShowArrow(QuestTopView.instance.tutorialButton, Highlighter.Side.bottom);
            else
                QuestsView.HideArrow();
        }
    }
}
#endif