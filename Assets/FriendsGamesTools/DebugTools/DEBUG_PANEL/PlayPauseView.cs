using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class PlayPauseView : MonoBehaviourHasInstance<PlayPauseView>
    {
        [SerializeField] GameObject parent, playParent, pauseParent;
#if DEBUG_PANEL
        protected override void Awake()
        {
            base.Awake();
            parent.SetActive(shouldBeShown);
        }
        void Update()
        {
            if (!parent.activeSelf) return;
            var paused = GameTime.paused;
            playParent.SetActive(paused);
            pauseParent.SetActive(!paused);
        }
        public void OnPlayPressed() => GameTime.Unpause();
        public void OnPausePressed() => GameTime.Pause();

        const string key = "ShownInDebug";
        public static bool shouldBeShown => ShownInDebug && !BuildModeSettings.release;
        public static bool ShownInDebug
        {
            get =>
#if ECS_GAME_TIME
                PlayerPrefs.GetInt(key, 1) == 1;
#else
                false;
#endif
            set
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
                instance?.parent.SetActive(shouldBeShown);
            }
        }
#endif
        }
    }
