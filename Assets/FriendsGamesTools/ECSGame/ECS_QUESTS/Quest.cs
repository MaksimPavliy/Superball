#if QUESTS
using System.Linq;
using FriendsGamesTools.ECSGame.Player.Money;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public abstract class Quest
    {
        protected GameRoot root => GameRoot.instance;
        public virtual int maxQuestProgressItems => 1;
        public int progressItems => controller.currQuest == this ? controller.data.currQuestProgressItems : 0;
        public abstract void UpdateActive();
        QuestsController controller => root.Get<QuestsController>();
        protected virtual void AddProgress(int items = 1)
        {
            controller.AddProgress(items);
        }
        protected virtual void Complete() => AddProgress(maxQuestProgressItems - progressItems);
        public virtual double rewardMoney => -1;
        public virtual int rewardHardCurrency => -1;
        protected PlayerMoneyController moneyController => root.Get<PlayerMoneyController>();
        public virtual void ClaimReward()
        {
            if (rewardMoney > 0)
                moneyController.AddMoneySoaked(rewardMoney);
            if (rewardHardCurrency > 0)
                moneyController.AddMoneySoaked(rewardHardCurrency, currency: CurrencyType.Hard);
        }
        public virtual void OnEvent(string eventName, (string key, object value)[] parameters) { }
        public abstract QuestViewConfig viewConfig { get; }
        public bool visible => viewConfig != null;
        public int ind => controller.quests.IndexOf(this);
        public bool started => ind <= controller.currQuestInd;
        public bool completed => claimed || progressItems >= maxQuestProgressItems;
        public bool claimed => ind < controller.currQuestInd;

        protected bool ParamExists((string key, object value)[] currParameters, (string key, object value) targetParam)
            => currParameters.Any(p => p.key == targetParam.key && p.value.ToString() == targetParam.value.ToString());
        protected bool EventIs(string targetEventName, (string key, object value) targetParam1, (string key, object value) targetParam2,
            string currEventName, (string key, object value)[] currParameters)
        {
            if (targetEventName != currEventName) return false;
            if (!ParamExists(currParameters, targetParam1)) return false;
            if (!ParamExists(currParameters, targetParam2)) return false;
            return true;
        }
    }
}
#endif