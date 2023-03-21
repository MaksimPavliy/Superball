using FriendsGamesTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Superball
{
    public class DistanceSlider : MonoBehaviourHasInstance<DistanceSlider>
    {
        [SerializeField] private GameObject _shownParent;
        [SerializeField] private RectTransform _wholeDistanceParent;
        [SerializeField] private Image _passedDistanceImage;
        [SerializeField] private RectTransform _distancePointer;
        [SerializeField] private TextMeshProUGUI _maxDistanceText;
        [SerializeField] private TextMeshProUGUI _levelText;
        private float _maxDistance=100;
        private float _passedDistance = 30;
        private float Ratio => _passedDistance / _maxDistance;
        private float _parentLength;
        public void Show()=>_shownParent.SetActive(true);
        public void Hide() => _shownParent.SetActive(false);
        private void Start()
        {
            _parentLength = _wholeDistanceParent.rect.width;
        }
        public void SetMaxDistance(float distance)
        {
            _maxDistance = distance;
            _maxDistanceText.text = $"{Mathf.FloorToInt(distance)}m";
            UpdateView();
        }
        public void SetPassedDistance(float distance)
        {
            _passedDistance = distance;
            UpdateView();
        }
        public void SetLevelText(string text)
        {
            _levelText.text = text;
        }
        private void UpdateView()
        {
            var _distancePointerPosX = _parentLength * Mathf.Clamp(Ratio,0.005f,1);
            _distancePointer.anchoredPosition = new Vector2(_distancePointerPosX, 0);
            _passedDistanceImage.fillAmount = Ratio;
        }

    }
}