#if TUTORIAL
using FriendsGamesTools.EditorTools.BuildModes;
using FriendsGamesTools.UI;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    [ExecuteAlways]
    public class TutorialManager : MonoBehaviourHasInstance<TutorialManager>
    {
        public List<TutorialChapter> chapters;
        public T GetChapter<T>() where T : TutorialChapter
            => chapters.Find(c => c.GetType() == typeof(T)) as T;
        public bool IsCompleted<T>() where T : TutorialChapter
            => GetChapter<T>().completed;
        public TutorialChapter shownChapter { get; private set; }
        public bool chapterShown => shownChapter != null;
        public bool completed
        {
            get
            {
                if (chapterShown)
                    return false;
                if (!config.enabled)
                    return true;
                return chapters.TrueForAll(c => c.completed);
            }
        }
        [SerializeField] float chapterUpdateInterval = 0.2f;
        float remainingToChapterUpdate;
        private void OnEnable()
        {
            base.Awake();
            FillParentRect.Fill(transform.GetComponent<RectTransform>());
        }
        TutorialSettings config => TutorialSettings.instance;
        void Update()
        {
            if (!Application.isPlaying)
                return;
            if (!config.enabled)
                return;
            // if no chapter is showing, periodically check for new chapter to start.
            if (shownChapter != null)
                return;
            remainingToChapterUpdate -= UnityEngine.Time.unscaledDeltaTime;
            if (remainingToChapterUpdate > 0)
                return;
            UpdateShownChapter();
        }
        void UpdateShownChapter()
        {
            remainingToChapterUpdate = chapterUpdateInterval;
            TutorialChapter chapterForShowing = null;
            foreach (var chapter in chapters)
            {
                if ((chapter.repeatsCount > chapter.completedCount) && chapter.condition)
                {
                    chapterForShowing = chapter;
                    break;
                }
            }
            if (chapterForShowing != null)
                ShowingChapter(chapterForShowing);
        }
        async void ShowingChapter(TutorialChapter chapter)
        {
            shownChapter = chapter;
            await shownChapter.Showing();
            shownChapter.SaveLastStepDone();
            shownChapter = null;
            UpdateShownChapter();
        }
    }
}
#endif