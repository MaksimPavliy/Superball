using UnityEngine;

namespace Superball
{
    public class BallSpeedTrail: MonoBehaviour
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private ParticleSystem _particles;
        bool _active = false;

        private void Activate()
        {
            if (_active) return;
            _active = true;
            _particles.Play();
        }
        private void Deactivate()
        {
            if (!_active) return;
            _active = false;
            _particles.Stop();
        }
        private void Start()
        {
            _ball.SamePipeEntered += Activate;
            _ball.LeftPipe += Deactivate;
        }
    }
}