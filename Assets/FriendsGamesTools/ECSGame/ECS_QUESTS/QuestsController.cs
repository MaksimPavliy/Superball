#if QUESTS
using System.Collections.Generic;
using FriendsGamesTools.UI;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public struct QuestData : IComponentData {
        public int currQuestProgressItems;
        public int currQuestInd;
    }
    public abstract class QuestsController:Controller
    {
        protected abstract List<Quest> GetQuestInstances();
        public List<Quest> quests { get; private set; }
        public new T Get<T>() where T : Quest => quests.Find(q => q is T) as T;
        bool dataExists => ECSUtils.GetSingleEntity<QuestData>(true) != Entity.Null;
        protected Entity e => ECSUtils.GetSingleEntity<QuestData>();
        public QuestData data => e.GetComponentData<QuestData>();
        public int currQuestInd => data.currQuestInd;
        public bool done => quests.Count == currQuestInd; // all quests claimed.
        public override void InitDefault()
        {
            ECSUtils.CreateEntity(new QuestData { });
        }
        public override void OnInited()
        {
            EventsForQuests.Init();
            lastCompletedStepTime = 0;
            quests = GetQuestInstances();
        }

        public Quest currQuest
        {
            get
            {
                if (dataExists && quests.IndIsValid(data.currQuestInd))
                    return quests[data.currQuestInd];
                else
                    return null;
            }
        }
        public void AddProgress(int items)
        {
            if (currQuest == null) return;
            e.ModifyComponent((ref QuestData q) => q.currQuestProgressItems += items);
            if (!currQuest.visible)
                ClaimReward();
        }
        public bool claimAvailable => currQuest != null && data.currQuestProgressItems >= currQuest.maxQuestProgressItems;
        public bool ClaimReward()
        {
            if (!claimAvailable) return false;
            currQuest.ClaimReward();
            e.ModifyComponent((ref QuestData q) =>
            {
                q.currQuestInd++;
                q.currQuestProgressItems = 0;
            });
            lastCompletedStepTime = GameTime.time;
            return true;
        }
        float lastCompletedStepTime; // Only curr session, cache.
        public float timeSinceLastStep => GameTime.time - lastCompletedStepTime;

        public void DebugCompleteAll()
        {
            e.ModifyComponent((ref QuestData q) => q.currQuestInd = quests.Count);
        }
    }
}
#endif