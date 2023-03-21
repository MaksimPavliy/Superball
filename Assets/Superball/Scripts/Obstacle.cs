using UnityEngine;

namespace Superball
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Bounds _bounds;
        public Bounds Bounds => _bounds;

        private void OnValidate()
        {
            if (_collider == null)
            {
                _collider = GetComponentInChildren<Collider2D>();
                if (_collider)
                {
                    _bounds = _collider.bounds;
                }
            }

        }
    }
}