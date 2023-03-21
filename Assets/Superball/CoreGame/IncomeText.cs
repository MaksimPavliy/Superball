using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HcUtils
{
    public class IncomeText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Transform _animationParent;
        [SerializeField] private Graphic[] _targetGraphic;
        [SerializeField] private float _distance = 100;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;
        public event Action<IncomeText> Disposed;
        public void Play(string text, Vector3 position, Vector3 lookRotation, bool positive)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(lookRotation);
            _text.text = text;
            _animationParent.transform.localPosition = Vector3.zero;
            _text.color = positive ? positiveColor : negativeColor;
            for (int i = 0; i < _targetGraphic.Length; i++)
            {
                var color = _targetGraphic[i].color;
                color.a = 1f;

                _targetGraphic[i].DOFade(0, _duration).ChangeStartValue(color);
            }

            _animationParent.DOLocalMoveY(_distance, _duration).ChangeStartValue(Vector3.zero).OnComplete(() =>
            {
                Dispose();
            });
        }
        private void Dispose()
        {
            Disposed?.Invoke(this);
        }
    }
}