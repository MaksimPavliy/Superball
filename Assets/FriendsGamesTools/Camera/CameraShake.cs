#if CAMERA
using UnityEngine;

namespace FriendsGamesTools
{
    public class CameraShake : MonoBehaviour
    {
        // Transform of the camera to shake. Grabs the gameObject's transform
        // if null.
        public Transform camTransform;

        // How long the object should shake for.
        public float shakeDuration = 0.5f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float shakeAmount = 0.7f;

        Vector3 originalPos;
        float remaining = -1;
        void Awake()
        {
            if (camTransform == null)
                camTransform = GetComponent<Transform>();
        }
        bool _isShaking;
        public bool isShaking
        {
            get => _isShaking;
            set
            {
                if (_isShaking == value)
                    return;
                _isShaking = value;
                if (!_isShaking)
                    camTransform.localPosition = originalPos;
                else
                    originalPos = camTransform.localPosition;
            }
        }
        public void Shake(float duration = -1)
        {
            if (duration < 0)
                duration = shakeDuration;
            remaining = duration;
            isShaking = true;
        }
        void Update()
        {
            //if (Input.GetKeyUp(KeyCode.Return))
            //    Shake();
            if (isShaking)
            {
                camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                if (remaining>0)
                {
                    remaining -= Time.deltaTime;
                    if (remaining <= 0)
                        isShaking = false;
                }
            }
        }
    }
}
#endif