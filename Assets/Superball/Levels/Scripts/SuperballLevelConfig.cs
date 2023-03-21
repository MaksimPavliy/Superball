using FriendsGamesTools.DebugTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superball
{
    public class SuperballLevelConfig : BalanceSettings<SuperballLevelConfig>
    {
        public float startLength = 100;

        public float lengthInc = 100;
        public float maxLevelHeight = 200;
        

        public float pipeBiasX_Base_min = 7;
        public float pipeBiasX_Base_max = 7;
        public float pipeBiasY_Base_min = -12;
        public float pipeBiasY_Base_max = 5;

        public float pipeBiasX_perPipeIncrement_min = 0.1f;
        public float pipeBiasX_perPipeIncrement_max = 0.2f;
        public float pipeBiasY_perPipeIncrement_min = 0.1f;
        public float pipeBiasY_perPipeIncrement_max = 0.5f;

        public float pipeBiasX_perLevelIncrement_min = 0.0f;
        public float pipeBiasX_perLevelIncrement_max = 0.2f;
        public float pipeBiasY_perLevelIncrement_min = 0.0f;
        public float pipeBiasY_perLevelIncrement_max = 0.7f;

        public float pipeBiasX_limit = 20;
        public float pipeBiasY_limit = 25;

        public float obstaclePatrolSpeed_min = 1f;
        public float obstaclePatrolSpeed_max_coef = 0.14f;
        public float obstaclesStartLevel = 4;
        public float obstacleChance_offset = 0.1f;
        public float obstacleChance_coef = 0.05f;
        public float obstacleChance_clampMin = 0f;
        public float obstacleChance_clampMax = 0.7f;

        public float pipeBound_top = 100;
        public float pipeBound_bottom = 2;

        public float GetObstacleChance(int levelIndex)
        {
            return levelIndex < (obstaclesStartLevel - 1) ?
                0 : Mathf.Clamp(obstacleChance_offset + obstacleChance_coef * levelIndex, obstacleChance_clampMin, obstacleChance_clampMax);
        }
        public int GetLevelIndex()
        {
            return SuperballRoot.instance.Get<SuperballLevelsController>().currLocationInd;
        }

      
        public float GetLevelLength()
        {
            int levelIndex = GetLevelIndex();

            return instance.startLength + levelIndex * lengthInc;// - (((levelIndex) / 5) * 150); //Декремент пока закомментим
        }

    }
}