#if ECS_TRAJECTORIES
using System;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [Serializable]
    public class TrajectoryPt
    {
        public float speedMul => 1;
        [SerializeField] Vector3 _pos; // World.
        [SerializeField] Transform _posTransform;
        public Vector3 pos => _posTransform != null ? _posTransform.position : _pos;
        public Transform posTransform => _posTransform;
        public void SetPos(Vector3 pos)
        {
            _pos = pos;
            if (_posTransform != null)
                _posTransform.position = _pos;
        }
        public void SetPos(Transform pos)
        {
            _posTransform = pos;
        }
        public TrajectoryPt(Vector3 pos) => SetPos(pos);
        public TrajectoryPt(Transform pos) => SetPos(pos);
        const string ControlledPtName = "pt";
        public TrajectoryPt AddTransform(Transform parent)
        {
            if (_posTransform == null)
            {
                _posTransform = new GameObject(ControlledPtName).transform;
                _posTransform.parent = parent;
                _posTransform.position = _pos;
                _posTransform.localRotation = Quaternion.identity;
                _posTransform.localScale = Vector3.one;
            }
            return this;
        }
        public void Destroy()
        {
            if (_posTransform != null && _posTransform.name == ControlledPtName)
            {
                if (Application.isPlaying)
                    MonoBehaviour.Destroy(_posTransform.gameObject);
                else
                    MonoBehaviour.DestroyImmediate(_posTransform.gameObject);
            }
        }
    }
}
#endif