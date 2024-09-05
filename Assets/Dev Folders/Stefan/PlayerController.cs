using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public int MaxJumps = 1;

    [field:SerializeField] public float PlayerSpeed { get; private set; } = 10f;
    [field:SerializeField] public float StrafePlayerSpeed { get; private set; } = 1f;
    [SerializeField] float _playerSpeedIncrease = 3.7f;
    [SerializeField] float _rollSpeed = 20f;
    [SerializeField] float _jumpPower;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] float _fallAcceleration;
    [SerializeField] float _fallSpeedCap;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Collider _collider;
    [SerializeField] float _groundCheckerRadius;

    [SerializeField] float _rotationSpeed;
    float _currAngle;
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _jumpAbilitySound;
    [SerializeField] AudioClip _rollSound;
    AudioSource _audio;

    //[SerializeField] float _groundCheckerOffset;
    Vector3 _movingSignal;
    Vector3 velocity;
    Vector3 _speedBeforeJump;

    [Header("For debugging only, don't touch it")]
    [SerializeField] bool _jumpPressed;
    [SerializeField] bool _grounded;
    Action _jumpCall;
    public bool Grounded => _grounded;
    public int _currentJump;
    //int _lastFrameCurrentJump;
    Collider[] _collisionResult = new Collider[1];
    Collider _prevPlatform;

    float _speedStartMoveTimer = 0;
    bool _wasGrounded;
    bool _wasPlatformLastFrame = false;

    Animator _animator;
    Coroutine _rollCoroutine;

    Vector3 _originalLocalPosition;
    float _originalRunPlayerSpeed;
    float _originalStrafePlayerSpeed;
    bool _wasIdle;

    void Start()
    {
        _currentJump = MaxJumps;
        _animator = GetComponentInChildren<Animator>();
        _originalRunPlayerSpeed = PlayerSpeed;
        _originalStrafePlayerSpeed = StrafePlayerSpeed;
        _audio = GetComponent<AudioSource>();
    }

    void OnDrawGizmos()
    {
        if (_collider == null) return;
        Vector3 groundCheckerPos = GetGroundCheckerPos();

        if (Physics.CheckSphere(GetGroundCheckerPos(), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground") ))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawSphere(groundCheckerPos, _groundCheckerRadius);
    }

    void Update()
    {
        _movingSignal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(Input.GetKeyDown(KeyCode.Space))
            _jumpPressed = true;
        _currAngle += _movingSignal.x * _rotationSpeed * Time.deltaTime;
        //transform.rotation = Quaternion.AngleAxis(_currAngle, transform.up);
        //_movingSignal.x = 0;
        //_rigidbody.MovePosition(transform.position + );
    }

    //the bug is t

    public void GiveInertia(Vector3 velocity)
    {
        _speedBeforeJump += velocity;
    }

    void PlaySound(AudioClip audioClip)
    {
        _audio.clip = audioClip;
        _audio.Play();
    }

    void FixedUpdate()
    {
        _grounded = GetIsGrounded();
        //relative to left and right local axis
        velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * PlayerSpeed * GetSmoothSpeedValue();

        DoPlatform();

        if (_jumpPressed)
        {
            if(_currentJump > 0)
            {
                Jump(_jumpPower);
                _grounded = false;
                _animator.SetTrigger("Jump");
                PlaySound(_jumpSound);


            }
            _jumpPressed = false;

        }

        if (_grounded)
        {//On run stay
            //_lastFrameCurrentJump = _currentJump;
            _currentJump = MaxJumps;
            _speedBeforeJump = Vector3.zero;

            SetRunningOrIdleAnimation();

            if (!_wasGrounded)//On Ground hit
            {
                _rigidbody.velocity = Vector3.zero;
                _wasGrounded = true;
                _animator.SetTrigger("Land");

                if (!_animator.GetBool("Rolling")) //Set normal speed when you land while not rolling
                {
                    PlayerSpeed = _originalRunPlayerSpeed;
                    StrafePlayerSpeed = _originalStrafePlayerSpeed;
                }


                //_animator.SetBool("Running", true);
            }

            if (IsNotMoving())
                StopRoll();
        }
        else
        {
            if (_wasGrounded)
            {//on air enter
                //stop rolling
                StopRoll();
            }
            _wasGrounded = false;
            bool isFalling = _rigidbody.velocity.y < 0;
            if (isFalling && _rigidbody.velocity.y > -_fallSpeedCap)
            {
                _rigidbody.AddForce(-Vector3.up * _fallAcceleration);
            }
            Strafe();

        }

        _jumpCall?.Invoke();
        _jumpCall = null;

        Vector3 position = transform.position + velocity;
        //if (transform.parent != null)
            //position = transform.parent.TransformPoint(transform.localPosition + velocity);
        //_rigidbody.MovePosition(position);
        transform.position = position;
    }

    public void AddJumpCount( int count)
    {
        if (_currentJump - count > -1)
            _currentJump += count;
    }
    
    public void SetCurrentJump(int count)
    {
         _currentJump = count;


    }

    void SetRunningOrIdleAnimation()
    {
        //if (!_wasIdle && _movingSignal == Vector3.zero)
        //{
        //    _animator.SetBool("Running", false);
        //    _wasIdle = true;
        //}
        //else
        //{
        //    if (_wasIdle && _movingSignal != Vector3.zero)
        //    {
        //        _animator.SetBool("Running", true);

        //    }
        //    _wasIdle = false;
        //}
         _animator.SetBool("Running", _movingSignal != Vector3.zero);

    }

    public void Roll(float runSpeed, float strafeSpeed, float time, float heightOffset)
    {
        if (_movingSignal == Vector3.zero) return;

        PlaySound(_rollSound);

        _originalRunPlayerSpeed = PlayerSpeed;
        _originalStrafePlayerSpeed = StrafePlayerSpeed;

        PlayerSpeed = runSpeed;
        StrafePlayerSpeed = strafeSpeed;

        _animator.SetBool("Running", false);
        _animator.SetBool("Rolling", true);
        _originalLocalPosition = _animator.transform.localPosition;
        _animator.transform.localPosition += Vector3.up * heightOffset;

        _rollCoroutine = StartCoroutine(DoRollAnimation(time));
    }

    public void StopRoll()
    {
        if (_rollCoroutine == null) return;

        if (Grounded) //Set normal speed when stop rolling on the ground
        {
            PlayerSpeed = _originalRunPlayerSpeed;
            StrafePlayerSpeed = _originalStrafePlayerSpeed;
        }
        if(_audio.clip == _rollSound)
            _audio.Stop();

        _animator.SetBool("Running", true);
        _animator.SetBool("Rolling", false);
        
        StopCoroutine(_rollCoroutine);
        _rollCoroutine = null;

        _animator.transform.localRotation = Quaternion.identity;
        _animator.transform.localPosition = _originalLocalPosition;
    }


    IEnumerator DoRollAnimation(float time)
    {
        float currTime = 0;
        float currAngle = 0;
        while(currTime < time)
        {
            _animator.transform.localRotation = Quaternion.AngleAxis(currAngle, Vector3.right);

            currAngle += Time.deltaTime * _rollSpeed;
            yield return null;
            currTime += Time.deltaTime;
        }
    }

    public bool IsNotMoving()
    {
        return _movingSignal == Vector3.zero;
    }

    void DoPlatform()
    {
        Collider col = _collisionResult[0];
        if (col != null && col.CompareTag("Platform"))
        {
            if(!_wasPlatformLastFrame)
            {//onEnter
                _prevPlatform = col;
                transform.parent = col.transform;
                _wasPlatformLastFrame = true;
            }
            //onStay
        }
        else
        {
            if(_wasPlatformLastFrame)
            {//on exit
                transform.parent = null;
                var platform = _prevPlatform.GetComponent<Platform>();
                GiveInertia(platform.Velocity);
                _prevPlatform = null;
            }
            _wasPlatformLastFrame = false;
        }


    }

    void Jump(float jumpPower)
    {
        StopAllCoroutines();
        StartCoroutine(JumpRoutine(jumpPower));
        _speedBeforeJump += velocity;
        _currentJump--;

    }

    public void RegisterJump(float jumpPower)
    {
        //int jumpCount = _grounded ? _lastFrameCurrentJump : _currentJump;

        _jumpCall = () =>
        {
            //Debug.Log(jumpCount);
             Jump(jumpPower);
            _currentJump = MaxJumps;
        };
    }

    IEnumerator JumpRoutine(float jumpPower)
    {
        float time = .5f;
        while(time > 0)
        {
            //from 1 to 0
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpCurve.Evaluate(time / .5f) * jumpPower, _rigidbody.velocity.z);
            yield return null;
            time -= Time.deltaTime;
        }
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -.01f, _rigidbody.velocity.z);

    }

    void Strafe()
    {
        //keeping velocity while jumping
        _speedBeforeJump += StrafePlayerSpeed * Time.deltaTime * velocity.normalized;
        //caping strafe velocity to grounded velocity
        float speedCap = PlayerSpeed * Time.deltaTime;
        if (_speedBeforeJump.magnitude > speedCap)
            _speedBeforeJump = _speedBeforeJump.normalized * speedCap;

        velocity = _speedBeforeJump;
    }

    Vector3 GetGroundCheckerPos()
    {
        return _collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y - _groundCheckerRadius + 0.01f);
    }

    bool GetIsGrounded()
    {
        int collisions = Physics.OverlapSphereNonAlloc(GetGroundCheckerPos(), _groundCheckerRadius, _collisionResult, 1 << LayerMask.NameToLayer("Ground"));
        
        if (collisions > 0)
            return true;

        _collisionResult[0] = null;
        return false;
    }

    float GetSmoothSpeedValue()
    {
        if (_movingSignal.sqrMagnitude != 0)
            _speedStartMoveTimer += Time.deltaTime * _playerSpeedIncrease;
        else _speedStartMoveTimer = 0;

        _speedStartMoveTimer = Mathf.Min(_speedStartMoveTimer, 1);
        return _speedStartMoveTimer;
    }
}
