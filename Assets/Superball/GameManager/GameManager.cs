using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player.Money;
using HcUtils;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Superball
{
    public class GameManager: MonoBehaviourHasInstance<GameManager>
    {
        [HideInInspector] public UnityEvent PlayPressed = new UnityEvent();
        [HideInInspector] public UnityEvent LevelComplete = new UnityEvent();
        [HideInInspector] public UnityEvent OnLevelLost = new UnityEvent();
        [SerializeField] private Ball ball;
        [SerializeField] private LevelSelector selector;
        [SerializeField] private Pipe pipe;
        [SerializeField] private new Camera camera;
        [SerializeField] private SuperballLevelGenerator _levelGenerator;
        [SerializeField] private FinishLine _finishLine;
        private bool isPlaying;
        //private int _score => ScoreManager.instance.Score;

        private int jumpCounter = 0;
        private int themeIndex = 0;
        public int CoinsAmount { private set; get; } = 0;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        private SuperballGeneralConfig Config => SuperballGeneralConfig.instance;

        private void Start()
        {
          //  ball.JumpSucceded += OnJumpSucceded;
            float ratio = camera.aspect;
            float maxBounds = ratio * camera.orthographicSize;
            ThemeSet.instance.ActivateSet(SuperballRoot.instance.levels.currLocationInd%4);
            /*pipe.maxOffset = maxBounds*1.2f;*/

            _levelGenerator.Generate();
            DistanceSlider.instance.SetMaxDistance(_finishLine.transform.position.x);
            DistanceSlider.instance.SetPassedDistance(ball.transform.position.x);
            DistanceSlider.instance.SetLevelText($"Level {GameRoot.instance.Get<SuperballLevelsController>().currLocationInd + 1}");
            DistanceSlider.instance.Show();
        }
        private void Update()
        {
            if (ball)
            {
                DistanceSlider.instance.SetPassedDistance(ball.transform.position.x);
            }
        }
        private void OnJumpSucceded()
        {
            jumpCounter++;
            AddCoins();
        }
        public void OnPlay()
        {
            IsPlaying = true;
            PlayPressed?.Invoke();
            SuperballMoneyView.instance.gameObject.SetActive(false);
        }

        public void AddCoins()
        {
            CoinsAmount++;
           // CoinsAmount = ball.jumpCounter % 5 == 0 ? CoinsAmount += 3 : CoinsAmount += 1;
          //  PlayerPrefs.SetInt("Coins", CoinsAmount);
        }
        public void OnLevelComplete()
        {
            SuperballMoneyView.instance.gameObject.SetActive(true);
            CameraSwitch.instance.DisablePursuit();
            GameRoot.instance.Get<SuperballLevelsController>().DoWin();
        //    GameRoot.instance.Get<HightScoreController>().TrySaveScore(_score);
            //FinishScore.instance.UpdateFinishScore();
            LevelComplete?.Invoke();
            IsPlaying = false;
            DistanceSlider.instance.Hide();
        }

        public void OnLose()
        {
            DistanceSlider.instance.Hide();
            SuperballMoneyView.instance.gameObject.SetActive(true);
            CameraSwitch.instance.DisablePursuit();
            GameRoot.instance.Get<SuperballLevelsController>().DoLose();
         //   GameRoot.instance.Get<HightScoreController>().TrySaveScore(_score);
            OnLevelLost?.Invoke();
            IsPlaying = false;
        }

        public void OnReset()
        {
           // ScoreManager.instance.ClearScore();
        }
    }
}