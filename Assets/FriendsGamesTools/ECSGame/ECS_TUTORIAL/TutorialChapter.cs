#if TUTORIAL
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    [ExecuteAlways]
    public abstract class TutorialChapter : MonoBehaviour
    {
        #region Persistant data
        public GameRoot root => GameRoot.instance;
        public TutorialController controller => TutorialController.instance;
        public TutorialManager tutorial => TutorialManager.instance;
        public int ind => tutorial.chapters.IndexOf(this);
        public int completedCount {
            get => controller.GetChapterData(ind).completedCount;
            set => controller.SetCompletedCount(ind, value);
        }
        public int lastCompletedStep
        {
            get => controller.GetChapterData(ind).lastCompletedStep;
            set => controller.SetLastCompletedStep(value, ind);
        }
        public T Get<T>() where T : TutorialChapter => TutorialManager.instance.GetChapter<T>();
        #endregion

        public virtual bool repeatable => false;
        public virtual int repeatsCount => repeatable ? int.MaxValue : 1;
        public bool completed => completedCount > 0;
        public abstract bool condition { get; }

        /// <summary>
        /// Returns true if curr step not done.
        /// </summary>
        protected virtual bool Step(int stepInd)
        {
            if (lastCompletedStep >= stepInd || completed)
                return false;
            lastCompletedStep = stepInd - 1;
            return true;
        }
        public void SaveLastStepDone() => controller.SetCompleted(ind);
        public abstract Task Showing();
#if UI
        protected virtual bool windowShown => UI.Windows.anyShown;
        protected async Task NoWindowsShown()
        {
            while (windowShown)
                await Awaiters.EndOfFrame;
        }
#endif
    }
}
#endif