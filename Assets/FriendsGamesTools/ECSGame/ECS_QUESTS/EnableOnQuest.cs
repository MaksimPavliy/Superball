#if QUESTS
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class EnableOnQuest : MonoBehaviour
    {
        [HideInInspector] public string showOnQuestTypeName, unlockOnQuestTypeName;
        int questIndToShow, questIndToUnlock;
        bool unlockExists => questIndToUnlock != -1 && !unlockOnQuestTypeName.IsNullOrEmpty();
        public GameObject parentToShow, lockedParent;
        QuestsController controller => GameRoot.instance.Get<QuestsController>();
        enum State { hidden, shownLocked, shown }
        State state = State.hidden;
        private void Start()
        {
            questIndToShow = controller.quests.FindIndex(questInstance => questInstance.GetType().Name == showOnQuestTypeName);
            if (questIndToShow == -1)
                questIndToShow = controller.quests.Count;
            parentToShow.SetActive(false);

            questIndToUnlock = controller.quests.FindIndex(questInstance => questInstance.GetType().Name == unlockOnQuestTypeName);
            if (questIndToUnlock == -1 && !unlockOnQuestTypeName.IsNullOrEmpty())
                questIndToUnlock = controller.quests.Count;
            lockedParent.SetActiveSafe(false);

            UpdateView();
        }
        private void Update() => UpdateView();
        private void UpdateView()
        {
            var neededState = controller.currQuestInd < questIndToShow ? State.hidden
                : ((unlockExists && controller.currQuestInd < questIndToUnlock) ? State.shownLocked : State.shown);
            if (state == neededState)
                return;
            state = neededState;
            lockedParent.SetActiveSafe(state == State.shownLocked);
            parentToShow.SetActiveSafe(state != State.hidden);
        }
    }
}
#endif