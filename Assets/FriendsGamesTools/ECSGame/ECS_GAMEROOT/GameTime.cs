using FriendsGamesTools.EditorTools.BuildModes;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class GameTime // Mb it should work also without ecs.
    {
#if ECS_GAMEROOT
        public static float time => ECSGame.GameRoot.instance.time;
        public static float deltaTime => ECSGame.GameRoot.instance.deltaTime;
        private static float timeSpeed => ECSGame.GameRoot.instance.timeSpeed;
#else
        public static float time => Time.time;
        public static float deltaTime => Time.deltaTime;
        private static float timeSpeed => 1f;
#endif

        //        public static float timeSpeed => BuildModeSettings.release ? 1 :
        //#if DEBUG_PANEL
        //            DebugTools.DebugSettings.instance.timeSpeed;
        //#else
        //        1;
        //#endif
        private static float GetSpeedWithPause(bool pause)
            => pause ? 0 : DebugTools.DebugSettings.instance.timeSpeed * timeSpeed;
        public static void SetPause(bool pause)
        {
            if (IsChangingSoft)
                Debug.LogError($"pause set to {pause} while pause was changing soft");
            Time.timeScale = GetSpeedWithPause(pause);
        }
        public static void UpdateUnityTimeSpeed() => Time.timeScale = GetSpeedWithPause(paused);
        public static void Pause() => SetPause(true);
        public static void Unpause() => SetPause(false);
        public static bool paused => Time.timeScale == 0;
        public static async Task SetPauseSoft(bool pause, float duration = 1)
        {
            var startSpeed = Time.timeScale;
            var endSpeed = GetSpeedWithPause(pause);
            if (startSpeed == endSpeed)
                return;
            IsChangingSoft = true;
            await AsyncUtils.SecondsWithProgress(duration, progress
                => Time.timeScale = Mathf.SmoothStep(startSpeed,endSpeed, progress), true);
            IsChangingSoft = false;
        }
        public static bool IsChangingSoft { get; private set; }
        public static async Task PauseSoft(float duration = 1) => await SetPauseSoft(true, duration);
        public static async Task UnpauseSoft(float duration = 1) => await SetPauseSoft(false, duration);
    }
}
