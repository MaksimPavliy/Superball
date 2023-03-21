using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Superball
{
    public struct LevelObjectFlag : IComponentData
    {
        bool _;
    }
    public struct GeneratedLevelData : IComponentData
    {
        public int index;
    }
    public struct PipeData : IComponentData
    {
        public int index;
        public int typeIndex;
        public SerializableVector3 position;
    }
    public struct ObstacleData : IComponentData
    {
        public SerializableVector3 patrolOffset;
        public float progressOffset;
        public float speed;
        public int typeIndex;
        public SerializableVector3 position;
    }
    public struct RotatorData : IComponentData
    {
        public float interval;
        public float angle;
        public float rotationTime;
        public float timeOffset;
    }

    public struct CoinData : IComponentData
    {
        public SerializableVector3 position;
    }

    [UpdateAfter(typeof(SuperballLevelsController))]
    public class LevelGeneratorController : Controller
    {
        public Entity LevelEntity => ECSUtils.GetSingleEntity<GeneratedLevelData>(true);
        public int GeneratedLevelIndex => LevelEntity.GetComponentData<GeneratedLevelData>().index;
        public bool GenerateNewLevel => GeneratedLevelIndex != SuperballRoot.instance.levels.currLocationInd;
        public override void OnInited()
        {
            base.OnInited();

            if (LevelEntity == Entity.Null)
            {
                ECSUtils.CreateEntity(new GeneratedLevelData { index = -1 });
            }
        }

        public void SetCurrentLevelAsGenerated()
        {
            LevelEntity.ModifyComponent((ref GeneratedLevelData data) => data.index = SuperballRoot.instance.levels.currLocationInd);
        }

        public void ClearLevelObjectsData()
        {
            ECSUtils.RemoveAllEntitiesWith<LevelObjectFlag>();
        }
        public Entity CreateObstacle(Vector3 position, int typeIndex, Vector3 patrolOffset, float progressOffset, float speed)
        {
            return ECSUtils.CreateEntity(new LevelObjectFlag(), new ObstacleData { position = position, typeIndex = typeIndex, patrolOffset = patrolOffset, progressOffset = progressOffset, speed = speed });
        }
        public Entity CreatePipe(int index, Vector3 position, int typeIndex)
        {
            return ECSUtils.CreateEntity(new LevelObjectFlag(), new PipeData { index = index, position = position, typeIndex = typeIndex });
        }
        public Entity AddPipeRotator(Entity pipe, float interval, float angle, float rotationTime, float timeOffset = 0)
        {

            pipe.AddComponent(new RotatorData { interval = interval, angle = angle, rotationTime = rotationTime, timeOffset = timeOffset });
            return pipe;
        }
        public Entity CreateCoin(Vector3 position)
        {
            return ECSUtils.CreateEntity(new LevelObjectFlag(), new CoinData { position = position });
        }
        public List<CoinData> GetLevelCoinsData() => GetLevelObjectsData<CoinData>();

        public List<PipeData> GetPipesData() => GetLevelObjectsData<PipeData>();
        public List<Entity> GetPipes() => ECSUtils.GetAllEntitiesWith<PipeData>().ToList();
        public List<ObstacleData> GetObstaclesData() => GetLevelObjectsData<ObstacleData>();
        public List<T> GetLevelObjectsData<T>() where T : struct, IComponentData
        {

            var entities = ECSUtils.GetAllEntitiesWith<T>().ToList();
            var result = new List<T>();
            foreach (var entity in entities)
            {
                result.Add(entity.GetComponentData<T>());
            }
            return result;
        }
    }
}