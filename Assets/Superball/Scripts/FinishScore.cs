using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using TMPro;
using UnityEngine;

namespace Superball
{
    public class FinishScore : MonoBehaviourHasInstance<FinishScore>
    {
        [SerializeField] private TextMeshProUGUI _hightScoreLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        private void Start()
        {
            UpdateFinishScore();
        }

        public void UpdateFinishScore()
        {
            var score = ScoreManager.instance.Score;
            var hightScoreLabel = GameRoot.instance.Get<HightScoreController>().GetScore();
            _scoreLabel.text = score.ToString();
            _hightScoreLabel.text = hightScoreLabel.ToString();
        }
    }
}