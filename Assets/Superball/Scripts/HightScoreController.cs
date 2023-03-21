using FriendsGamesTools.ECSGame;
using Unity.Entities;
using UnityEngine;

namespace Superball
{
    public struct Score : IComponentData
    {
        public int value;
    }

    public class HightScoreController: Controller
    {
        Entity entity => GetSingletonEntity<Score>();
        private int currentHightScore => entity.GetComponentData<Score>().value;

        public override void InitDefault()
        {
            base.InitDefault();
            ECSUtils.CreateEntity(new Score { });
        }

        public bool TrySaveScore(int newScore)
        {
            if(currentHightScore< newScore)
            {
                SaveNewScore(newScore);
                return true;
            }
            return false;
        }

        private void SaveNewScore(int score)
        {
            entity.ModifyComponent((ref Score sc) => {
                sc.value = score;
            });
        }

        public int GetScore()
        {
            return currentHightScore;
        }
    }
}