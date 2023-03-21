using DG.Tweening;
using FriendsGamesTools;
using HcUtils;
using SplineMesh;
using System;
using System.Collections;
using TweenFunctions;
using UnityEngine;

namespace Superball
{
    public enum BallState
    {
        FreeFlight,
        EnteringPipe,
        InPipe,
        LeavingPipe
    }
    public class Ball : MonoBehaviour, IControllable
    {
        [SerializeField] private GameObject groundEdge;
        [SerializeField] private ParticleSystem _deathParticles;
        private Pipe previousPipe;
        public Pipe currentPipe = null;
        private CurveSample sample;
        private Rigidbody2D _rigidbody;
        private Vector3 gForce = new Vector3(0f, 0f, 8f);
        private bool InPipe => State == BallState.InPipe;
        private bool FreeFlight => State == BallState.FreeFlight;
        private Vector3 velocity;
        private Vector3 inVelocity;
        private Vector3 _localGravity;
        public BallState State { private set; get; } = BallState.FreeFlight;
        private float _tubeDistance;
        private CircleCollider2D _collider;
        public int jumpCounter;

        /* private bool InTube => enteredRightTube && enteredLeftTube;*/

        public event Action JumpSucceded;
        public event Action<bool> CanControlChanged;


        private BallConfig BallConfig => BallConfig.instance;
        public float maxOffset = 5.4f;
        private int _tubeMoveDirectionSign = 1;
        private float _tubeSpeed;

        private bool _touchingTheEntrance = false;
        private float _minAcceleration;
        private float _maxAcceleration;
        private float _minPipeVelocity = 5f;
        private float _pipeMiddleVelocity = 0;
        private float _pipeVelocity = 0;

        bool _dragged = false;
        float _angularVelocity = 0;
        bool _finished = false;
        private float _outVelocityMultiplier = 1;
        private float _inVelocityModule;

        public event Action EnteredPipe;
        public event Action LeftPipe;
        public event Action SamePipeEntered;

        int _samePipeInRow = 0;

        float _minHapticDuration = 0.25f;
        float _lastHapticTime = 0;
        private void Start()
        {
            _collider = GetComponent<CircleCollider2D>();
            CanControlChanged?.Invoke(true);
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.simulated = false;
            EventSignature();
        }

        private void EventSignature()
        {
            GameManager.instance.PlayPressed.AddListener(OnPlay);
            GameManager.instance.OnLevelLost.AddListener(LevelDone);
        }

        public void OnPlay()
        {
            _rigidbody.simulated = true;
            _rigidbody.angularVelocity = -1000f;
        }

        public void LevelDone()
        {
            ParticleSystem ps = Instantiate(_deathParticles, transform.position, Quaternion.identity);
            // ps.gameObject.transform.up = tempVelocity.normalized;
            ps.Play();
            Destroy(gameObject);
            Destroy(ps.gameObject, 2f);
        }

        private void OnDestroy()
        {
            if (!GameManager.instance) return;
            GameManager.instance.PlayPressed.RemoveListener(OnPlay);
            GameManager.instance.OnLevelLost.RemoveListener(LevelDone);
        }

        private void Update()
        {

            _touchingTheEntrance = false;
            if (_finished) return;
            if (FreeFlight)
            {
                // var position = transform.position + Vector3.right * Joystick.instance.dragDir.x * _sensitivityTouch * Time.deltaTime * 20f;
                if (_dragged)
                {
                    var dragDir = Joystick.instance.dragDir;
                    var velocity = _rigidbody.velocity;


                    // var easeX =BallConfig.joystickSmooth? (Easing.EaseOut(Mathf.Abs(dragDir.x), EasingType.Cubic) * Mathf.Sign(dragDir.x)) : dragDir.x;
                    var easeX = dragDir.x;
                    velocity.x = easeX * BallConfig.controlSensitity;
                    _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity,velocity, BallConfig.strafeVelocity * Time.deltaTime);
                    _rigidbody.angularVelocity = Mathf.Clamp(Mathf.Abs(easeX * 1000f), 600f, 1500f) * Mathf.Sign(-easeX);
                }

            }

            Vector3 sampleWorldPosition;
            Vector3 sampleWorldDirection;
            float distanceThreshold = 0;
            switch (State)
            {
                case BallState.FreeFlight:
                    break;
                case BallState.EnteringPipe:
                    sample = currentPipe.GetSampleAtDistance(_tubeDistance);
                    sampleWorldPosition = currentPipe.GetSampleWorldPosition(sample);

                    float delta = inVelocity.magnitude * 2f * Time.deltaTime;
                    float distance = Vector3.Distance(transform.position, sampleWorldPosition);
                    distanceThreshold = delta - distance;
                    transform.position = Vector3.MoveTowards(transform.position, sampleWorldPosition, delta);
                    transform.Rotate(Vector3.forward, _angularVelocity * Time.deltaTime);
                    if (Vector3.Distance(transform.position, sampleWorldPosition) == 0)
                    {
                        State = BallState.InPipe;
                        var closestSample = currentPipe.GetClosestSample(transform.position);
                        _tubeDistance = closestSample.distanceInSpline;
                    }
                    break;
                case BallState.InPipe:
                    sample = currentPipe.GetSampleAtDistance(_tubeDistance);

                    sampleWorldPosition = currentPipe.GetSampleWorldPosition(sample);
                    sampleWorldDirection = currentPipe.GetSampleWorldDirection(sample);

                    transform.position = sampleWorldPosition;

                    var direction = sampleWorldDirection.normalized;

                    float progressValue = 0;
                    float currAcceleration = 0;
                    Vector3 acceleration;

                    if (_tubeDistance < (currentPipe.Length / 2f))
                    {
                        progressValue = _tubeDistance / (currentPipe.Length / 2f);
                 

                        _pipeVelocity = _tubeMoveDirectionSign > 0 ? Mathf.Lerp(_inVelocityModule, _pipeMiddleVelocity, progressValue) :
                            Mathf.Lerp(_inVelocityModule, _pipeMiddleVelocity, (progressValue));
                    }
                    else
                    {
                        progressValue = (_tubeDistance - currentPipe.Length / 2f) / (currentPipe.Length / 2f);

                        _pipeVelocity = _tubeMoveDirectionSign > 0 ? Mathf.Lerp(_inVelocityModule, _pipeMiddleVelocity, 1-progressValue) :
                           Mathf.Lerp(_inVelocityModule, _pipeMiddleVelocity, (1-progressValue));
                    }
                 //   Debug.Log($"value:{progressValue} velModule:{_pipeVelocity} vel:{velocity}");
                    _tubeDistance += _pipeVelocity * Time.deltaTime * _tubeMoveDirectionSign;
                    _tubeDistance = Mathf.Clamp(_tubeDistance, 0, currentPipe.Length);
                    transform.Rotate(Vector3.forward, _angularVelocity * Time.deltaTime);
                    if (_tubeDistance >= currentPipe.Length || _tubeDistance <= 0)
                    {
                        ExitPipe();
                    }
                    break;
                case BallState.LeavingPipe:
                    if (!CheckPipeEntrance())
                    {
                        State = BallState.FreeFlight;
                        currentPipe = null;

                    }
                    break;
                default:
                    velocity = _rigidbody.velocity;
                    break;
            }

        }

        private bool CheckPipeEntrance()
        {
            if (_finished) return false;
            var colliders = Physics2D.OverlapCircleAll(transform.position, _collider.radius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("pipeEntrance"))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator FinishRoutine()
        {
            _rigidbody.isKinematic = true;
          //  _rigidbody.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.2f);
            //yield return new WaitForFixedUpdate();
            //yield return new WaitForEndOfFrame();
           Vector3 _startPos = transform.position;
            CameraSwitch.instance.DisablePursuit();
            Haptic.Vibrate(HapticType.Medium);
            transform.DOShakePosition(1f, 0.1f, 10, 90, false, false) ;
            EffectsManager.instance.PlayFinishBlow(_startPos);
            yield return new WaitForSeconds(1f);
            transform.DOMove(_startPos + Vector3.up * 50f, 2f);
            Haptic.Vibrate(HapticType.Heavy);

            GameManager.instance.OnLevelComplete();

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_finished) return;

            if (collision.CompareTag("Finish"))
            {

                _finished = true;
                CanControlChanged?.Invoke(false);
             
            //    _rigidbody.AddForce(Vector2.right * 1000f);
                StartCoroutine(FinishRoutine());


            }

            if (collision.CompareTag("pipeEntrance"))// && velocity.y < 0)
            {
                if (FreeFlight)
                {
                    //входим в трубу только если мы ещё не в ней
                    EnterTube(collision);
                    //при входе в левую трубу стартовая дистанция 0

                }
            }
            else if (collision.CompareTag("obstacle"))
            {
                GameManager.instance.OnLose();
                OnDragEnded();
                CanControlChanged?.Invoke(false);
                Haptic.Vibrate(HapticType.Heavy);
            }
            else if (collision.CompareTag("coin"))
            {
                var coin = collision.GetComponent<Coin>();
                if (coin.Collect())
                {
                    GameManager.instance.AddCoins();
                    Haptic.Vibrate(HapticType.Light);
                }
            }

        }

        public void OnDragged(Vector2 dir)
        {
            _dragged = true;
        }

        private void EnterTube(Collider2D collision)
        {
            Haptic.Vibrate(HapticType.Light);
            State = BallState.InPipe;

            var pipeEntrance = collision.gameObject.GetComponent<PipeEntrance>();
            _localGravity = _rigidbody.velocity.normalized * gForce.magnitude;

            currentPipe = pipeEntrance.Pipe;

         
         

            _tubeMoveDirectionSign = pipeEntrance.DirectionSign;
            _tubeDistance = _tubeMoveDirectionSign > 0 ? 0 : currentPipe.Length;

            groundEdge.GetComponent<BoxCollider2D>().enabled = false;
            _tubeSpeed = _rigidbody.velocity.magnitude;


            sample = currentPipe.GetSampleAtDistance(_tubeDistance);


            var sampleWorldDirection = currentPipe.GetSampleWorldDirection(sample).normalized * _tubeMoveDirectionSign;

            if (currentPipe != previousPipe)
            {
                inVelocity = gForce.magnitude * sampleWorldDirection;
                _samePipeInRow = 1;
            }
            else
            {
                _samePipeInRow++;
                inVelocity *= BallConfig.instance.samePipeSpeedMultiplier;
                SamePipeEntered?.Invoke();
                EffectsManager.instance.PlayIncomeText(_samePipeInRow, currentPipe.transform.position + Vector3.up * 2);
            }
            _outVelocityMultiplier = BallConfig.instance.samePipeSpeedMultiplier;

         
            _inVelocityModule = inVelocity.magnitude;
            _pipeMiddleVelocity = _inVelocityModule * 1.1f;

            _rigidbody.simulated = false;

         //   ScoreManager.instance.UpdateScore();
            _angularVelocity = Mathf.Clamp(Mathf.Abs(_rigidbody.angularVelocity), 1000, 10000) * -pipeEntrance.DirectionSign;

            State = BallState.InPipe;
            var closestSample = currentPipe.GetClosestSample(transform.position);
            _tubeDistance = closestSample.distanceInSpline;
            EnteredPipe?.Invoke();
        }
        //выход из трубы

        
        private void ExitPipe()
        {
            Haptic.Vibrate(HapticType.Medium);
            OnDragEnded();
            State = BallState.LeavingPipe;
            _touchingTheEntrance = true;
            Debug.Log("Leave pipe");
            //на выходе из трубы снова включаем силы для ригидбоди и задаём начальную скорость, обратную скорости входа
            _rigidbody.simulated = true;
            //   inVelocity += inVelocity * 0.2f;

            sample = currentPipe.GetSampleAtDistance(_tubeDistance);

            var sampleWorldDirection = currentPipe.GetSampleWorldDirection(sample).normalized;
            // velocity = sampleWorldDirection * _rigidbody.velocity.magnitude;
            var outVelocity = _tubeMoveDirectionSign  * inVelocity.magnitude * sampleWorldDirection;

            _rigidbody.velocity = outVelocity;
            _rigidbody.angularVelocity = _angularVelocity;
            previousPipe = currentPipe;
             
          
            if (BallConfig.instance.resetJoystickInPipe)
            {
                Joystick.instance.ResetDrag();
            }  else
            {
                _dragged = true;
            }

            LeftPipe?.Invoke();
            //  StartCoroutine(AwaitLeavingPipe());
        }

        public void OnDragEnded()
        {
            _dragged = false;
        }
    }
}