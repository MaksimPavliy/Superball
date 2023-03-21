#if UI
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public static partial class Sprites
    {
        public const float FPS = 30;
        public static int AnimInd(float progress, int spritesCount, bool reverse = false)
        {
            var maxInd = spritesCount - 1;
            var ind = Mathf.RoundToInt(progress * maxInd);
            return AnimInd(ind, spritesCount, reverse);
        }
        public static int AnimInd(int ind, int spritesCount, bool reverse = false)
        {
            var maxInd = spritesCount - 1;
            if (reverse)
                ind = maxInd - ind;
            if (ind < 0 || ind > maxInd)
                Debug.Log($"hi, AnimInd is bad, ind = {ind}, spritesCount = {spritesCount}");
            return ind;
        }
        public static int AnimInd(float progress, int startFrame, int endFrame, bool reverse = false)
        {
            var ind = AnimInd(progress, endFrame - startFrame + 1, reverse);
            ind += startFrame;
            return ind;
        }

        public static void ShowAnimProcess(ImgAndShadow pic,
            float startTime, List<SpriteAndShadow> sprites, float speed = 1f)
        {
            var ind = Mathf.RoundToInt((Time.time - startTime) * FPS * speed);
            ind %= sprites.Count;
            pic.Show(sprites[ind]);
        }
        public static void ShowAnimProcess(float startTime, Image pic, List<Sprite> sprites)
        {
            if (pic == null) return;
            var ind = Mathf.RoundToInt((Time.time - startTime) * FPS);
            ind %= sprites.Count;
            pic.sprite = sprites[ind];
        }
        public static void ShowAnimProcess(float startTime, SpriteRenderer pic, List<Sprite> sprites)
        {
            if (pic == null) return;
            var ind = Mathf.RoundToInt((Time.time - startTime) * FPS);
            ind %= sprites.Count;
            if (sprites.IndIsValid(ind))
                pic.sprite = sprites[ind];
        }
        public static void ShowAnimatedIco(Image pic, List<Sprite> sprites, bool shown, float lastShownChangeTime, 
            int appearFramesCount, int loopFramesCount, float loopWaitingDuration, int disappearFramesCount)
        {
            var ind = GetAnimatedIcoSpriteInd(shown, lastShownChangeTime, 
                appearFramesCount, loopFramesCount, loopWaitingDuration, disappearFramesCount);
            var active = ind != -1;
            pic.gameObject.SetActive(active);
            if (active)
                pic.sprite = sprites[ind];
        }
        // Gets anim ind for some ico with appear, loop, disappear sprites.
        public static int GetAnimatedIcoSpriteInd(bool shown, float lastShownChangeTime,
                                                   int appearFramesCount,
                                                   int loopFramesCount, float loopWaitingDuration,
                                                   int disappearFramesCount)
            => GetAnimatedIcoSpriteInd(shown, lastShownChangeTime, 0, appearFramesCount, appearFramesCount,
                loopFramesCount, loopWaitingDuration, appearFramesCount + loopFramesCount, disappearFramesCount);
        public static int GetAnimatedIcoSpriteInd(bool shown, float lastShownChangeTime,
                                                    int appearStartFrame, int appearFramesCount,
                                                   int loopStartFrame, int loopFramesCount, float loopWaitingDuration,
                                                   int disappearStartFrame, int disappearFramesCount)
        {
            var elapsedFrame = (int)((UnityEngine.Time.time - lastShownChangeTime) * Sprites.FPS);
            if (elapsedFrame < 0)
                return -1;
            if (!shown)
            {
                if (elapsedFrame < disappearFramesCount)
                    return disappearStartFrame + elapsedFrame;
                else
                    return -1;
            }
            else
            {
                if (elapsedFrame < appearFramesCount)
                    return appearStartFrame + elapsedFrame;
                elapsedFrame -= appearFramesCount;
                var waitingFrames = Mathf.RoundToInt(loopWaitingDuration * Sprites.FPS);
                elapsedFrame %= loopFramesCount + waitingFrames;
                if (elapsedFrame < waitingFrames)
                    return loopStartFrame;
                elapsedFrame -= waitingFrames;
                return loopStartFrame + elapsedFrame;
            }
        }
        public static void ShowAnimatedIco(Image pic, GameObject parent, bool shown,
                            ref bool prevShown, ref float lastShownChangeTime, float loopWaitingDuration,
                            List<Sprite> appearPics, List<Sprite> loopPics, List<Sprite> disappearPics)
        {
            if (prevShown != shown)
            {
                lastShownChangeTime = UnityEngine.Time.time;
                prevShown = shown;
            }
            var elapsedFrame = (int)((UnityEngine.Time.time - lastShownChangeTime) * Sprites.FPS);
            if (elapsedFrame < 0)
            {
                parent.SetActive(false);
                return;
            }
            if (!shown)
            {
                if (elapsedFrame < disappearPics.Count)
                {
                    parent.SetActive(true);
                    pic.sprite = disappearPics[elapsedFrame];
                    return;
                }
                else
                {
                    parent.SetActive(false);
                    return;
                }
            }
            else
            {
                parent.SetActive(true);

                if (elapsedFrame < appearPics.Count)
                {
                    pic.sprite = appearPics[elapsedFrame];
                    return;
                }
                elapsedFrame -= appearPics.Count;
                var waitingFrames = Mathf.RoundToInt(loopWaitingDuration * Sprites.FPS);
                elapsedFrame %= loopPics.Count + waitingFrames;
                if (elapsedFrame < waitingFrames)
                    pic.sprite = loopPics[0];
                else
                {
                    elapsedFrame -= waitingFrames;
                    pic.sprite = loopPics[elapsedFrame];
                }
            }
        }
#if ECS_GAMEROOT
        public static FriendsGamesTools.ECSGame.DurableProcess CreateAnimProcess(List<SpriteAndShadow> animSprites, float speed)
        {
            var process = CreateAnimProcess(animSprites);
            process.speed = speed;
            return process;
        }
        public static ECSGame.DurableProcess CreateAnimProcess(List<SpriteAndShadow> animSprites)
            => CreateAnimProcess(animSprites.Count);
        public static void ShowAnimProcess(ECSGame.DurableProcess process, ImgAndShadow pic,
            List<SpriteAndShadow> sprites, bool reverse = false)
        {
            if (sprites.Count > 0)
                pic.Show(sprites[AnimInd(process.progress, sprites.Count, reverse)]);
        }

        public static int ShowAnimProcess(ECSGame.DurableProcess process, ImgAndShadow pic,
            List<SpriteAndShadow> sprites, ECSGame.AnimationStoryboard storyboard, bool reverse = false)
        {
            var frame = storyboard.GetFrame(process.progress, reverse);
            //if (!sprites.IndIsValid(frame))
            //    Debug.Log("Hi");
            pic.Show(sprites[frame]);
            return frame;
        }

        public static ECSGame.DurableProcess CreateAnimProcess(FriendsGamesTools.ECSGame.AnimationStoryboard storyBoard, float speed = 1)
            => CreateAnimProcess(storyBoard.framesCount, speed);
        public static ECSGame.DurableProcess CreateAnimProcess(int spritesCount, float speed = 1)
        {
            var duration = spritesCount / FPS;
            return new ECSGame.DurableProcess { duration = duration / speed, speed = speed };
        }

        public static void ShowAnimProcess(ECSGame.DurableProcess process, Image pic,
            List<Sprite> sprites, bool reverse = false)
        {
            if (sprites.Count > 0)
                pic.sprite = sprites[AnimInd(process.progress, sprites.Count, reverse)];
        }

        public static void ShowAnimProcess(ECSGame.DurableProcess process, SpriteRenderer pic,
            List<Sprite> sprites, bool reverse = false)
        {
            if (sprites.Count > 0)
                pic.sprite = sprites[AnimInd(process.progress, sprites.Count, reverse)];
        }

        public static int ShowAnimProcess(ECSGame.DurableProcess process, Image pic,
            List<Sprite> sprites, ECSGame.AnimationStoryboard storyboard, bool reverse = false)
        {
            var frame = storyboard.GetFrame(process.progress, reverse);
            pic.sprite = sprites[frame];
            return frame;
        }
#endif
    }

    [Serializable]
    public class SpriteAndShadow
    {
        public Sprite pic;
        public Sprite shadow;
    }

    [Serializable]
    public class ImgAndShadow
    {
        public Image pic;
        public Image shadow;
        public SpriteRenderer picRenderer;
        public SpriteRenderer shadowRenderer;
        public void Show(SpriteAndShadow sprites, bool setNativeSize = false)
        {
            if (pic != null)
            {
                pic.sprite = sprites.pic;
                shadow.sprite = sprites.shadow;
                if (setNativeSize)
                {
                    pic.SetNativeSize();
                    shadow.SetNativeSize();
                }
            }
            if (picRenderer != null)
            {
                picRenderer.sprite = sprites.pic;
                shadowRenderer.sprite = sprites.shadow;
            }
        }
    }
}
#endif