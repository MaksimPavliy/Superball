#if QUESTS
using FriendsGamesTools.UI;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestTopView : MonoBehaviourHasInstance<QuestTopView>
    {
        public TutorialButton tutorialButton;
        [SerializeField] GameObject parent;
        [SerializeField] Transform notificationTopPos;
        bool notifying, prevComplete;
        QuestsController controller => GameRoot.instance.Get<QuestsController>();
        bool complete => controller.claimAvailable;
        private void Update()
        {
            var shown = (!Windows.anyShown || notifying) && (controller.currQuest?.visible ?? false);
            parent.SetActive(shown);

            if (!prevComplete && complete && !notifying && Windows.anyShown)
                ShowNotification();
            prevComplete = complete;
        }
        async void ShowNotification()
        {
            notifying = true;
            await AsyncUtils.SecondsWithProgress(0.5f, progress => SetY(progress));
            await Awaiters.Seconds(0.5f);
            await AsyncUtils.SecondsWithProgress(0.5f, progress => SetY(1 - progress));
            notifying = false;
            parent.SetActive(false);
            SetY(1);
        }

        Vector3 parentPos;
        protected override void Awake()
        {
            base.Awake();
            parentPos = parent.transform.localPosition;
        }
        void SetY(float progress)
        {
            parent.transform.localPosition = Vector3.Lerp(notificationTopPos.localPosition, parentPos, Mathf.SmoothStep(0, 1, progress));
        }
        public const string QuestTopViewPressedEventName = "QuestTopViewPressed";
        public void OnPressed()
        {
            QuestWindow.Show();
            Analytics.AnalyticsManager.Send(QuestTopViewPressedEventName);
        }
    }
}
#endif