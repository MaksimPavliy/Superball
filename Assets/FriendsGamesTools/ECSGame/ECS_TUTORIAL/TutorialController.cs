#if TUTORIAL
using FriendsGamesTools.ECSGame;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public struct TutorialData : IComponentData { bool ignored; }
    [InternalBufferCapacity(200)]
    public struct TutorialChapterData : IBufferElementData
    {
        public int completedCount;
        public int lastCompletedStep;
    }
    public class TutorialController:Controller
    {
        Entity e => ECSUtils.GetSingleEntity<TutorialData>();
        public override void InitDefault()
        {
            if (TutorialManager.instance == null)
                return;
            var e = ECSUtils.CreateEntity(new TutorialData { });
            e.AddBuffer<TutorialChapterData>();
            foreach (var c in TutorialManager.instance.chapters)
                e.GetBuffer<TutorialChapterData>().Add(new TutorialChapterData { completedCount = 0, lastCompletedStep = -1 });
            Highlighter.Cancel();
        }
        public bool completed => TutorialManager.instance.completed;
        public static TutorialController instance { get; private set; }
        public override void OnInited()
        {
            instance = this;
            lastCompletedStepTime = 0;
        }
        public TutorialChapterData GetChapterData(int ind) => e.GetBuffer<TutorialChapterData>()[ind];
        public void SetLastCompletedStep(int stepInd, int chapterInd)
            => ModifyChapterData(chapterInd, (ref TutorialChapterData data) => data.lastCompletedStep = stepInd);
        public void SetCompletedCount(int chapterInd, int completedCount)
        {
            ModifyChapterData(chapterInd, (ref TutorialChapterData data) => data.completedCount = completedCount);
        }
        public void SetCompleted(int chapterInd)
        {
            ModifyChapterData(chapterInd, (ref TutorialChapterData data) => data.completedCount++);
            lastCompletedStepTime = GameTime.time;
        }
        float lastCompletedStepTime; // Only curr session, cache.
        public float timeSinceLastStep => GameTime.time - lastCompletedStepTime;
        private void ModifyChapterData(int ind, RefAction<TutorialChapterData> change)
        {
            var buffer = e.GetBuffer<TutorialChapterData>();
            var data = buffer[ind];
            change(ref data);
            buffer[ind] = data;
        }
        public void DebugCompleteAll()
        {
            foreach (var c in TutorialManager.instance.chapters)
                c.completedCount = c.repeatsCount;
        }
    }
}
#endif