using FriendsGamesTools;
using UnityEngine;
using UnityEngine.UI;

namespace Superball
{
    public class JoystickShower: MonoBehaviour
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private Transform _originTr;
        [SerializeField] private Transform _pointerTr;
        [SerializeField] private Transform _joyBg;
        private void Update()
        {
            _joystick.maxDistance = BallConfig.instance.joystickMaxDistance;
            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].enabled = BallConfig.instance.showJoystick;
            }
        }
        private void LateUpdate()
        {
            var pos= _pointerTr.position;
            pos.y = _originTr.position.y;
            _originTr.rotation = Quaternion.identity;
            _pointerTr.position = pos;
            _joyBg.position=_originTr.position;
            
        }
    }
}