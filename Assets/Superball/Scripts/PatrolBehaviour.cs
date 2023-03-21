using DG.Tweening;
using UnityEngine;

namespace Superball
{
    public class PatrolBehaviour: MonoBehaviour
    {
        [SerializeField] private Vector3 _patrolOffset;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _progressOffset = 0;
        private float _speed = 1f;
        private Vector3 _startPoint;
        private Vector3 _targetPoint;
        public void SetPatrolOffset(Vector3 offset)=>_patrolOffset = offset;
        public void SetProgressOffset(float offset) => _progressOffset = offset;
        public void SetSpeed(float speed) => _speed = speed;

        private float _progress = 0;
        private float _time = 0;
        private float _durationOffset;
        private void Start()
        {
            _startPoint = transform.position;
            _targetPoint = _startPoint + _patrolOffset;
            _duration = _patrolOffset.magnitude / _speed;
            _durationOffset = _duration * _progressOffset;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            _progress = Mathf.PingPong(_durationOffset + _time/_duration, 1f);
            transform.position = Vector3.Lerp(_startPoint, _targetPoint, _progress);
        }

    }
}