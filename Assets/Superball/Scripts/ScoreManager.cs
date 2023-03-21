using FriendsGamesTools;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace Superball
{
    public class ScoreManager : MonoBehaviourHasInstance<ScoreManager>
    {
        [SerializeField] private GameObject _parent;
        private TextMeshProUGUI _label;
        private TweenScale _tween;
        private int _score;

        public int Score { get => _score; set => _score = value; }

        void Start()
        {
            _label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _tween = gameObject.GetComponentInChildren<TweenScale>();
            ActiveScoreText(false);
        }

        public void UpdateScore()
        {
            Score++;
            _tween.PlayOnce();
            _label.text = Score.ToString();
        }

        public void ClearScore()
        {
            Score = 0;
            _tween.PlayOnce();
            _label.text = Score.ToString();
        }

        public void ActiveScoreText(bool value)
        {
            _parent.SetActive(value);
        }
    }
}