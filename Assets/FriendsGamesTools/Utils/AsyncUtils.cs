using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class AsyncUtils
    {
        public static async Task SimulateInternetDelayIfDebug() {
            if (!BuildMode.release)
                await Awaiters.SecondsRealtime(FGTSettings.instance.simulatedInternetDelay);
        }
        public static async Task SecondsWithProgress(float duration, Action<float> onProgressUpdated, bool realTime = false)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                onProgressUpdated(elapsed / duration);
                elapsed += realTime ? Time.unscaledDeltaTime : Time.deltaTime;
                await Awaiters.EndOfFrame;
            }
            onProgressUpdated(1);
        }
        public static async void ExecuteAfter(float duration, Action action)
        {
            await Awaiters.Seconds(duration);
            action?.Invoke();
        }
        public static async void ExecuteAfterRealTime(float duration, Action action)
        {
            await Awaiters.SecondsRealtime(duration);
            action?.Invoke();
        }
        public static async void ExecuteNextFrame(Action action)
        {
            await Awaiters.EndOfFrame;
            action?.Invoke();
        }
        public static async void ExecuteAfterFrames(int frames, Action action)
        {
            await FramesCount(frames);
            action?.Invoke();
        }
        public static async Task FramesCount(int frames)
        {
            for (int i = 0; i < frames; i++)
                await Awaiters.EndOfFrame;
        }
    }
}