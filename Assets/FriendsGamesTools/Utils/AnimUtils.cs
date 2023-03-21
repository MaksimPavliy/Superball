using System;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class AnimUtils
    {
        public static bool IsPlaying(this Animator anim)
        {
            var state = anim.GetCurrentAnimatorStateInfo(0);
            return 1 > state.normalizedTime;
        }
        // Idea is to make some process with configured length 
        // consisting from looped and not looped anim clips, 
        // by tweaking loop count and animation speed 
        // in order to minimize animation speed changes from ideal anim speed.
        public static (float FPS, int loopsCount) 
            FitDurationFPS(float duration, int notLoopedFramesCount, int loopedFramesCount, float idealFPS = 30)
        {
            // duration = (loopedFramesCount*LoopsCount+notLoopedFramesCount)/FPS.
            // FPS = (loopedFramesCount*LoopsCount+notLoopedFramesCount)/duration = 30.
            // LoopsCount = (30*duration - notLoopedFramesCount)/loopedFramesCount.
            var loopsCount = Mathf.Max(1, Mathf.RoundToInt((idealFPS * duration - notLoopedFramesCount) / loopedFramesCount));
            var FPS = (loopedFramesCount * loopsCount + notLoopedFramesCount) / duration;
            return (FPS, loopsCount);
        }
        public static (float speed, int loopsCount)
            FitDuration(float duration, float notLoopedDuration, float loopedDuration, float idealSpeed = 1)
        {
            // duration = (loopedDuration*loopsCount+notLoopedDuration)/speed.
            // speed = (loopedDuration*loopsCount+notLoopedDuration)/duration = idealSpeed.
            // loopsCount = (idealSpeed*duration - notLoopedDuration)/loopedDuration.
            var loopsCount = Mathf.Max(1, Mathf.RoundToInt((idealSpeed * duration - notLoopedDuration) / loopedDuration));
            var speed = (loopedDuration * loopsCount + notLoopedDuration) / duration;
            return (speed, loopsCount);
        }
        public static void IterateClips(this Animator anim, Action<AnimationClip> action)
        {
            RuntimeAnimatorController ac = anim.runtimeAnimatorController;
            for (int i = 0; i < ac.animationClips.Length; i++)
                action(ac.animationClips[i]);
        }
    }
}