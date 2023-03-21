using FriendsGamesTools;
using FriendsGamesTools.UI;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class CoreGameView : MonoBehaviourHasInstance<CoreGameView>
    {
        [SerializeField] GameObject shownParent;
#if ECS_LEVEL_BASED
        public bool shown => shownParent.activeSelf;
        public bool isPlaying => GameRoot.instance.Get<WinnableLocationsController>().state == Level.State.playing;
        protected virtual void Update()
        {
            var shouldBeShown = isPlaying;
#if ECS_SKINS
            if (shouldBeShown && Windows.Get<SkinsWindow>() && Windows.Get<SkinsWindow>().shown)
                shouldBeShown = false;
#endif
            if (shown != shouldBeShown)
                SetShown(shouldBeShown);
        }
        protected virtual void SetShown(bool shown)
        {
            shownParent.SetActive(shown);
            if (shown)
                OnCoreGameShown();
        }
        protected virtual void OnCoreGameShown()
        {
            LevelBasedView.SetLevelText(string.Empty);
        }
#endif
        }
    }