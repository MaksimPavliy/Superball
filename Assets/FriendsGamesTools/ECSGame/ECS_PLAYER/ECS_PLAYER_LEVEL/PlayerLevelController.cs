#if ECS_PLAYER_LEVEL
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Player.Level
{
    [UpdateAfter(typeof(PlayerController))]
    public abstract class PlayerLevelController : Controller
    {
        public Entity e => PlayerController.entity;
        public PlayerLevel data => e.Get<PlayerLevel>();
        public int level => data.level;
        public double exp => data.exp;
        public override void InitDefault()
            => e.Add(new PlayerLevel() { level = 1 });
        protected virtual bool autoActivateLevelUp => false;
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (autoActivateLevelUp && levelupAvailable)
                ActivateLevelUp();
        }
        public virtual void AddExp(double exp)
        {
            if (exp <= 0)
                return;

            CheckThatExpIsNotGivenForMoneyForLevelUp();

            e.Modify((ref PlayerLevel level) => {
                level.exp += exp; // Levelup is only done in ActivateLevelUp.
            });
        }
        public abstract double GetExpForNextLevel(int currLevel);
        public virtual bool levelupAvailable => GetExpForNextLevel(data.level) <= data.exp;
        protected virtual bool multipleLevelups => false;
        public virtual bool ActivateLevelUp(double multiplier = 1)
        {
            var level = this.data;
            if (!levelupAvailable)
                return false;
            if (multipleLevelups)
            {
                var expForLevelUp = GetExpForNextLevel(level.level);
                level.exp -= expForLevelUp;
            } else
                level.exp = 0;
            level.level++;
            e.Set(level);
            return true;
        }
        public virtual void DebugAddLevel(int count)
        {
            var level = this.data;
            level.level++;
            e.Set(level);
        }

        void CheckThatExpIsNotGivenForMoneyForLevelUp()
        {
#if UNITY_EDITOR
            var stackTrace = Environment.StackTrace.Split('\n');
            List<string> methodsToFind = new List<string> { "AddExp (", "AddMoney (", "ActivateLevelUp (" };
            int searchMethodInd = 0;
            for (int i = 0; i < stackTrace.Length; i++)
            {
                if (stackTrace[i].Contains(methodsToFind[searchMethodInd]))
                {
                    searchMethodInd++;
                    if (methodsToFind.Count <= searchMethodInd)
                    {
                        Debug.LogError("Never add experiance when money is received after levelup.\n" +
                            "This is forbidden by game design.\n" +
                            "Make AddMoneyNoExp() and call it in ActivateLevelUp() instead");
                        return;
                    }
                }
            }
#endif
        }
    }
}
#endif