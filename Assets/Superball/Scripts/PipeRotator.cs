using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Superball
{
    public class PipeRotator : MonoBehaviour
    {
        [SerializeField] private float _rotationInterval = 1;
        [SerializeField] private float _rotationTime = 0.3f;
        [SerializeField] private float _rotationAngle = 90;
        [SerializeField] private float _timeOffset = 0;
        [SerializeField] private Transform _arrows;
        private float _rotation;
        public void Init(float interval,float angle,float rotationTime,float timeOffset=0)
        {
            _rotationInterval=interval;
            _rotationAngle = angle;
            _rotationTime = rotationTime;
             _timeOffset =timeOffset;
        }
        private void Start()
        {
            _arrows.gameObject.SetActive(true);
            StartCoroutine(RotationRoutine());
        }
        private IEnumerator RotationRoutine()
        {
            yield return new WaitForSeconds(_timeOffset * _rotationInterval);
            while (true)
            {
                yield return new WaitForSeconds(_rotationInterval);
                if (_rotation > 10 || _rotation < -10)
                {
                    _rotationAngle = -_rotationAngle;
                       _rotation += _rotationAngle;
                }
                else
                {
                    _rotation += _rotationAngle;
                }
            
                transform.DORotateQuaternion( Quaternion.AngleAxis(_rotation, Vector3.forward)* Quaternion.AngleAxis(90,Vector3.right), _rotationTime);
            }
        }
    }
}