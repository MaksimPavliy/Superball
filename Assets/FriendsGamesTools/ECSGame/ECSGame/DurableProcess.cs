#if ECSGame && ECS_GAMEROOT
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public struct DurableProcess : IDurableProcess
    {
        public float elapsed;
        public float duration;
        public bool paused;
        [SerializeField]
        private float _speedMinus1;
        
        public float speed { get => _speedMinus1 + 1; set => _speedMinus1 = value - 1; }
        public float remaining => duration - elapsed;
        public float progress {
            get => Mathf.Clamp01(elapsed / duration);
            set => elapsed = Mathf.Clamp(value * duration, 0, duration);
        }
        public bool finished => elapsed >= duration;

        bool IDurableProcess.paused => paused;
        float IDurableProcess.elapsed { get => elapsed; set => elapsed = value; }



#if ECS_GAME_TIME
        bool IDurableProcess.ticksOffline => ticksOffline;
        public bool ticksOffline;
        public static DurableProcess Create(float duration, float elapsed = 0, bool ticksOffline = false)
            => new DurableProcess { duration = duration, elapsed = elapsed, ticksOffline = ticksOffline };
#else
        public static DurableProcess Create(float duration, float elapsed = 0)
            => new DurableProcess { duration = duration, elapsed = elapsed };
#endif
    }

    public static class DurableProcessUtils
    {
        public static void Tick<TDurableProcess>(ref TDurableProcess p, float deltaTime) where TDurableProcess : IDurableProcess
            => p.elapsed += deltaTime * p.speed;
        public static DurableProcess Add(this DurableProcess p1, DurableProcess p2)
            => new DurableProcess { duration = p1.duration + p2.duration };
        public static DurableProcess SplitLoops(this DurableProcess p, int loops)
        {
            var duration = p.duration / loops;
            return new DurableProcess { duration = duration, elapsed = p.elapsed % duration };
        }
        public static DurableProcess SpeedUp(this DurableProcess p, float speedMul)
        {
            p.speed *= speedMul;
            return p;
        }
        public static DurableProcess Join(params int[] frameCounts)
            => Sprites.CreateAnimProcess(frameCounts.Sum());
        public static DurableProcess Split(this DurableProcess p, int ind, params int[] frameCounts)
        {
            int startFrame = 0;
            for (int i = 0; i < ind; i++)
                startFrame += frameCounts[i];
            var endFrame = startFrame + frameCounts[ind];
            var totalCount = frameCounts.Sum();
            var startProgress = startFrame / (float)totalCount;
            var endProgress = endFrame / (float)totalCount;
            var res = new DurableProcess { duration = (endProgress - startProgress) * p.duration };
            return res.WithProgress((p.progress - startProgress) / (endProgress - startProgress));
        }
        public static DurableProcess WithProgress(this DurableProcess p, float progress)
        {
            p.elapsed = Mathf.Clamp(progress * p.duration, 0, p.duration);
            return p;
        }
    }
    public class DurableProcessSystem : AbstractDurableProcessSystem<DurableProcess> { }
}
#endif