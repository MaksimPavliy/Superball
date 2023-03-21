using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Superball
{
    public class SuperballLevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _finish;

        [SerializeField] private List<Pipe> pipePrefabs;
        [SerializeField] private Transform _pipesParent;

        [SerializeField] private GameObject finishPrefab;

        [SerializeField] private SpriteRenderer _groundSprite;
        [SerializeField] private SpriteRenderer _groundSpikes;
        private SuperballLevelConfig levelConfig => SuperballLevelConfig.instance;

        private float _groundAdditionalSpace = 20f;

        private Vector3 _lastPipePostion;

        [SerializeField] private Coin _coinPrefab;

        private List<Pipe> _pipes;
        [SerializeField] private List<Obstacle> _obstaclePrefabs;

        [SerializeField] private bool _setFinish = true;
        [SerializeField] private bool _spawnPipes = true;
        [SerializeField] private bool _spawnObstacles = true;
        [SerializeField] private bool _spawnCoins = true;
        [SerializeField] private Transform _ceiling;
        [SerializeField] private Transform _leftWall;
        private float _levelLength = 0;
        private float _groundLength = 0;
        private float _highestPipeHeight = 0;
        private LevelGeneratorController GeneratorController;
        bool _forceGenerate = false;
        public void Generate()
        {
            GeneratorController = SuperballRoot.instance.Get<LevelGeneratorController>();
            _pipes = _pipesParent.GetComponentsInChildren<Pipe>().ToList();
            SetFinish();
            _levelLength = _finish.transform.position.x;
            ApplyBounds();
            if (GeneratorController.GenerateNewLevel || _forceGenerate)
            {
                GeneratorController.ClearLevelObjectsData();
                SpawnPipes();
                SpawnCoins();
                SpawnSecondaryPipes();
                GeneratorController.SetCurrentLevelAsGenerated();
            }
            else
            {
                foreach (var pipe in _pipes)
                {
                    Destroy(pipe.gameObject);

                }
                _pipes.Clear();
                LoadLevel();

            }

            ApplyCeiling();

        }

        private void LoadLevel()
        {
            var pipesEntities = GeneratorController.GetPipes();

            foreach (var pipe in pipesEntities)
            {
                var data = pipe.GetComponentData<PipeData>();
                var p = CreatePipe(data.position, data.typeIndex);
                if (pipe.HasComponent<RotatorData>())
                {
                    var rotatorData = pipe.GetComponentData<RotatorData>();
                    AddPipeRotator(p, rotatorData.interval, rotatorData.angle, rotatorData.rotationTime, rotatorData.timeOffset);
                }
            }

            var coinsData = GeneratorController.GetLevelCoinsData();
            foreach (var data in coinsData)
            {
                CreateCoin(data.position);
            }

            var obstacles = GeneratorController.GetObstaclesData();
            foreach (var data in obstacles)
            {
                CreateObstacle(data.position, data.typeIndex, data.patrolOffset, data.speed, data.progressOffset);
            }
        }
        private void ApplyBounds()
        {
            float levelLength = _levelLength;
            _groundLength = levelLength + _groundAdditionalSpace * 2f;
            _groundSprite.size = new Vector2(_groundSprite.size.x, _groundLength);
            var groundPosition = _groundSprite.transform.position;
            groundPosition.x = levelLength / 2f;
            _groundSprite.transform.position = groundPosition;

            _groundSpikes.size = new Vector2(_groundLength, _groundSpikes.size.y);
            _groundSpikes.transform.position = new Vector3(levelLength / 2f, 0, 0);
            var collider = _groundSpikes.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(_groundLength, collider.size.y);

            _leftWall.transform.position -= Vector3.right * (_groundAdditionalSpace);
        }

        private void ApplyCeiling()
        {
            _highestPipeHeight = 0;
            foreach (var p in _pipes)
            {
                if (p.transform.position.y > _highestPipeHeight)
                {
                    _highestPipeHeight = p.transform.position.y;
                }
            }
            var ceilingHeight = Mathf.Clamp(_highestPipeHeight + 13f, 0, levelConfig.maxLevelHeight);
            _ceiling.transform.position = _groundSpikes.transform.position + Vector3.up * ceilingHeight;
            _ceiling.transform.localScale = new Vector3(_groundLength - _groundAdditionalSpace, _ceiling.transform.localScale.y, 1);
        }
        private void SpawnPipes()
        {
            _lastPipePostion = new Vector3(1, 0, 0);
            float levelLength = _levelLength;
            int levelIndex = levelConfig.GetLevelIndex();
            Pipe pipePrefab = null;

            Vector2 minPipeDistanceRandom = new Vector2(levelConfig.pipeBiasX_Base_min, levelConfig.pipeBiasY_Base_min);
            Vector2 maxPipeDistanceRandom = new Vector2(levelConfig.pipeBiasX_Base_max, levelConfig.pipeBiasY_Base_max);

            Vector2 perPipeIncreaseMin = new Vector2(levelConfig.pipeBiasX_perPipeIncrement_min, levelConfig.pipeBiasY_perPipeIncrement_min);
            Vector2 perPipeIncreaseMax = new Vector2(levelConfig.pipeBiasX_perPipeIncrement_max, levelConfig.pipeBiasY_perPipeIncrement_max);

            Vector2 perLevelIncreaseMin = new Vector2(levelConfig.pipeBiasX_perPipeIncrement_min, levelConfig.pipeBiasY_perPipeIncrement_min);
            Vector2 perLevelIncreaseMax = new Vector2(levelConfig.pipeBiasX_perPipeIncrement_max, levelConfig.pipeBiasY_perPipeIncrement_max);

            Vector2 maxPipeDistance = new Vector2(levelConfig.pipeBiasX_limit, levelConfig.pipeBiasY_limit);

            float _minObtacleSpeed = levelConfig.obstaclePatrolSpeed_min;
            float _maxObstacleSpeed = levelIndex * levelConfig.obstaclePatrolSpeed_max_coef;
            float _obstacleChance = levelConfig.GetObstacleChance(levelIndex);

            int pipesCount = 0;

            int startRotatorLevel = 7;


            while (pipesCount < 100000)
            {

                var offset = new Vector3(Utils.Random(minPipeDistanceRandom.x, maxPipeDistanceRandom.x), Utils.Random(minPipeDistanceRandom.y, maxPipeDistanceRandom.y));
                offset += new Vector3(Utils.Random(perPipeIncreaseMin.x, perPipeIncreaseMax.x * pipesCount), Utils.Random(perPipeIncreaseMin.y, perPipeIncreaseMax.y) * pipesCount);
                offset += new Vector3(Utils.Random(perLevelIncreaseMin.x, perLevelIncreaseMax.x * levelIndex), Utils.Random(perLevelIncreaseMin.y, perLevelIncreaseMax.y) * levelIndex);
                offset.x = Mathf.Clamp(offset.x, -1000, maxPipeDistance.x);
                offset.x = Mathf.Clamp(offset.y, -1000, maxPipeDistance.y);


                Vector3 pos = _lastPipePostion + offset;
                pos.x = Mathf.Clamp(pos.x, _lastPipePostion.x + minPipeDistanceRandom.x, 1000);
                pos.y = Mathf.Clamp(pos.y, levelConfig.pipeBound_bottom, levelConfig.pipeBound_top);
                if (pipesCount == 0)
                {
                    pos.y = Mathf.Clamp(pos.y, _lastPipePostion.y + 5f, 10000);
                }
                if (_spawnObstacles)
                {
                    bool createObstacle = Random.value < _obstacleChance;
                    if (createObstacle)
                    {
                        var obsOffset = Vector3.right * 3f + Vector3.up * (-5f);
                        pos.x += obsOffset.x;
                        var obsPos = _lastPipePostion + (pos - _lastPipePostion) / 2f;

                        var patrolOffset = new Vector3(0, Mathf.Clamp((pos - _lastPipePostion).y + Random.value * 4f, 4f, 1000), 0);
                        var partrolSpeed = Utils.Random(_minObtacleSpeed, _maxObstacleSpeed);
                        var progressOffset = Random.value;
                        int typeIndex = Utils.RandomInd(_obstaclePrefabs);
                        CreateObstacle(obsPos, typeIndex, patrolOffset, partrolSpeed, progressOffset);
                        GeneratorController.CreateObstacle(obsPos, typeIndex, patrolOffset, progressOffset, partrolSpeed);
                    }
                }

                if (pos.x >= levelLength - minPipeDistanceRandom.x / 2f)
                {
                    break;
                }

                int typeInd = Utils.RandomInd(pipePrefabs);


                CreatePipe(pos, typeInd);
                pipesCount++;
                _lastPipePostion = pos;
            }
            bool addRotator = levelIndex >= startRotatorLevel;
            var rotatorPipe = _pipes.Where(x => _pipes.IndexOf(x) > 0).ToList().RandomElement();
            for (int i = 0; i < _pipes.Count; i++)
            {
                var e = GeneratorController.CreatePipe(i, _pipes[i].transform.position, _pipes[i].typeIndex);
                if (addRotator && (rotatorPipe == _pipes[i]))
                {
                    var rotator = rotatorPipe.gameObject.GetComponent<PipeRotator>();
                    rotator.enabled = true;
                    float rotatorInterval = 5 + Utils.Random(-1f, 1f);
                    float rotatorAngle = (Utils.Random(0, 1f) < 0.5f ? 1 : -1) * 90;
                    float rotationTime = Utils.Random(0, 1f);
                    float rotationTimeOffset = 0;

                    AddPipeRotator(rotatorPipe, rotatorInterval, rotatorAngle, rotationTime, rotationTimeOffset);
                    GeneratorController.AddPipeRotator(e, rotatorInterval, rotatorAngle, rotationTime, rotationTimeOffset);
                }
            }
        }

        private void SpawnSecondaryPipes()
        {
            int typeInd = Utils.RandomInd(pipePrefabs);

            Vector2 min = new Vector2(_pipes[0].transform.position.x, _pipes[0].transform.position.y);

            _highestPipeHeight = 0;
            foreach (var p in _pipes)
            {
                if (p.transform.position.y > _highestPipeHeight)
                {
                    _highestPipeHeight = p.transform.position.y;
                }
            }
            Vector2 max = new Vector2(_levelLength, _highestPipeHeight);

            int _maxSecondaryPipes = Mathf.Clamp((int)(_pipes.Count / 1.5f), 1, 200);

            bool _validPos = false;
            for (int i = 0; i < _maxSecondaryPipes; i++)
            {
                bool _created = false;
                int iterationsCount = 0;
                while (!_created && iterationsCount<1000)
                {
                    _validPos = true;
                    var pos = new Vector3(Utils.Random(min.x, max.x), Utils.Random(min.y, max.y), 0);
                    for (int j = 0; j < _pipes.Count; j++)
                    {
                        if (Vector3.Distance(pos, _pipes[j].transform.position) < 8f)
                        {
                            _validPos = false;
                        }
                    }
                    if (_validPos)
                    {
                        var pipe = CreatePipe(pos, typeInd);
                        GeneratorController.CreatePipe(_pipes.IndexOf(pipe), pos, typeInd);
                        SpawnCoinsArc(10, pipe.Entrances[0].transform.position, pipe.Entrances[1].transform.position, Utils.Random(1.5f, 2.5f));

                        //var closestRightPipe=GetClosestPipeToTheRight(pipe);
                        //if (closestRightPipe)
                        //{
                        //    SpawnCoinsArc(10, pipe.Entrances[1].transform.position, closestRightPipe.Entrances[0].transform.position, Utils.Random(1.5f, 2.5f));
                        //}
                        _created = true;
                        break;
                    }
                    iterationsCount++;
                }
            }
        }

        private Pipe GetClosestPipeToTheRight(Pipe curPipe)
        {
            Pipe resultPipe = null;
            float minDistance = 100500;
            for (int i = 0; i < _pipes.Count; i++)
            {
                if (curPipe == _pipes[i]) continue;
                if (_pipes[i].transform.position.x < curPipe.transform.position.x + 5f) continue;
                var dist = Vector3.Distance(_pipes[i].transform.position, curPipe.transform.position);
                if (dist < minDistance)
                {
                    minDistance=dist;
                       resultPipe = _pipes[i];
                }
            }
            return resultPipe;
        }
        private void AddPipeRotator(Pipe pipe, float rotatorInterval, float rotatorAngle, float rotationTime, float rotationTimeOffset)
        {
            var rotator = pipe.gameObject.GetComponent<PipeRotator>();
            rotator.enabled = true;
            rotator.Init(rotatorInterval, rotatorAngle, rotationTime, rotationTimeOffset);
        }
        public void CreateObstacle(Vector3 position, int typeIndex, Vector3 patrolOffset, float speed, float progressOffset)
        {
            var prefab = _obstaclePrefabs[typeIndex];
            var obs = Instantiate(prefab, transform);
            obs.transform.position = position;
            var patrol = obs.GetComponent<PatrolBehaviour>();
            patrol.SetPatrolOffset(patrolOffset);
            patrol.SetSpeed(speed);
            patrol.SetProgressOffset(progressOffset);
        }
        public Pipe CreatePipe(Vector3 position, int typeIndex)
        {
            var pipePrefab = pipePrefabs[typeIndex];
            Pipe pipe = Instantiate(pipePrefab, _pipesParent);
            pipe.Spawn(position);
            _pipes.Add(pipe);
            return pipe;
        }
        private Coin CreateCoin(Vector3 position)
        {
            var coin = Instantiate(_coinPrefab, transform);
            coin.transform.position = position;
            return coin;
        }

        public void SpawnCoinsArc(int count, Vector3 _startPoint, Vector3 _endPoint, float jumpMultiplier)
        {
            Vector3 _lastPos = Vector3.zero;
            List<Coin> _coins = new List<Coin>();
            for (int j = 1; j < count; j++)
            {

                float t = j * jumpMultiplier / count;
                var pos = GameSettings.instance.GetTrajectoryPoint(_startPoint, _endPoint, t, jumpMultiplier);
                if (j > 1)
                {
                    if ((_lastPos - pos).magnitude < 0.7f)
                    {
                        continue;
                    }
                }
                _coins.Add(CreateCoin(pos));
                GeneratorController.CreateCoin(pos);
                _lastPos = pos;
            }
        }
        private void SpawnCoins()
        {
            for (int i = 1; i < _pipes.Count; i++)
            {
                var pt1 = _pipes[i - 1].Entrances[1];
                var pt2 = _pipes[i].Entrances[0];

                SpawnCoinsArc(10, pt1.transform.position, pt2.transform.position, 1.7f);
                //float _distance = 0;
                //Vector3 _lastPos = Vector3.zero;
                //int count = 10;
                //List<Coin> _coins = new List<Coin>();
                //float jumpMultiplier = 1.7f;
                //for (int j = 1; j < count; j++)
                //{

                //    float t = j * jumpMultiplier / count;
                //    var pos = GameSettings.instance.GetTrajectoryPoint(pt1.transform.position, pt2.transform.position, t, jumpMultiplier);
                //    if (j > 1)
                //    {
                //        if ((_lastPos - pos).magnitude < 0.7f)
                //        {
                //            continue;
                //        }
                //    }
                //    _coins.Add(CreateCoin(pos));
                //    GeneratorController.CreateCoin(pos);
                //    _lastPos = pos;
            }



        }
        private void SetFinish()
        {
            _finish.transform.position = new Vector2(levelConfig.GetLevelLength(), 0);
        }
    }
}