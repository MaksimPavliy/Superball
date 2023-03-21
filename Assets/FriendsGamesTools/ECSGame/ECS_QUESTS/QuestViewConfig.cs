#if QUESTS
using System;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    [Serializable]
    public class QuestViewConfig
    {
        public Sprite ico, customReward;
        public string title, description;
    }
}
#endif