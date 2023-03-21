#if ECSGame
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame // Probably can be moved to be more general than ECS.
{
    // Frames playing order to make some anim process shown.
    [Serializable] public class AnimationStoryboard
    {
        [Serializable] public class Item
        {
            public int startFrame;
            public int endFrame;
            public int _repeats = 1;
            public int repeats => Mathf.Max(1, _repeats);
            public int frames => (Mathf.Abs(endFrame - startFrame) + 1) * repeats;
        }
        public List<Item> items;
        public List<int> frames { get; private set; }
        private bool inited => frames != null;
        public void InitIfNeeded()
        {
            if (inited) return;
            frames = new List<int>();
            items.ForEach(item => Utils.Repeat(item.repeats,
                () => (item.startFrame, item.endFrame).FromTo(frame => frames.Add(frame))));
        }
        public int framesCount
        {
            get
            {
                InitIfNeeded();
                return frames.Count;
            }
        }
        public int GetFrame(int ind, bool reverse = false)
        {
            InitIfNeeded();
            return frames[ind];
        }
        public int GetFrame(float progress, bool reverse = false)
        {
            InitIfNeeded();
            int ind = Sprites.AnimInd(progress, framesCount, reverse);
            return frames[ind];
        }
    }
}
#endif