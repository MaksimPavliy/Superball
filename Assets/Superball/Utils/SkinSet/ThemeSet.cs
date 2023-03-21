using FriendsGamesTools;
using System;
using System.Collections;
using System.Collections.Generic;
namespace HcUtils
{
    public class ThemeSet : MonoBehaviourHasInstance<ThemeSet>
    {
        public static Action<int> ThemeSetChanged;
        public static int activeSet = 0;
        public int startSet = -1;
        public int setsCount = 5;
        protected override void Awake()
        {
            base.Awake();
            ThemeSetChanged += OnThemeChanged;
            ActivateSet(startSet);
        }

        public void ActivateSet(int index)
        {
            if (index < 0)
            {
                index = Utils.Random(0, setsCount - 1);
            }
            activeSet = index;

            ThemeSetChanged?.Invoke(activeSet);
        }

        private void OnThemeChanged(int ind) => activeSet = ind;
    }
}