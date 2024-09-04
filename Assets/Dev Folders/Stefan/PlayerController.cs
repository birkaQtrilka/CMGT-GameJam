using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _playerSpeed;
    [SerializeField] float _playerSpeedIncrease = 8f;
    [SerializeField] float _strafePlayerSpeed;
    [SerializeField] float _jumpPower;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] int _availableJumps = 1;
    [SerializeField] float _fallAcceleration;
    [SerializeField] float _fallSpeedCap;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Collider _collider;
    [SerializeField] float _groundCheckerRadius;
    //[SerializeField] float _groundCheckerOffset;
    Vector3 _movingSignal;
    Vector3 velocity;
    Vector3 _speedBeforeJump;

    [Header("For debugging only, don't touch it")]
    [SerializeField] bool _jumpPressed;
    [SerializeField] bool _grounded;

    Action _jumpCall;
    public bool Grounded => _grounded;
    int _currentJump;
    //int _lastFrameCurrentJump;
    Collider[] _collisionResult = new Collider[1];
    Collider _prevPlatform;

    float _speedStartMoveTimer = 0;
    bool _wasGrounded;
    bool _wasPlatformLastFrame = false;

    void Start()
    {
        _currentJump = _availableJumps;
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
        

        //_rigidbody.MovePosition(transform.position + );
    }

    public void GiveInertia(Vector3 velocity)
    {
        _speedBeforeJump += velocity;
    }

    void FixedUpdate()
    {
        _grounded = GetIsGrounded();
        //relative to left and right local axis
        velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * _playerSpeed * GetSmoothSpeedValue();

        DoPlatform();

        if (_jumpPressed)
        {
            if(_currentJump > 0)
            {
                Jump(_jumpPower);
                _grounded = false;
            }
            _jumpPressed = false;

        }

        if (_grounded)
        {
            //_lastFrameCurrentJump = _currentJump;
            _currentJump = _availableJumps;
            _speedBeforeJump = Vector3.zero;

            if (!_wasGrounded)//for only one frame
            {
                _rigidbody.velocity = Vector3.zero;
                _wasGrounded = true;
            }
        }
        else
        {
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
            _currentJump = _availableJumps;
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
        _speedBeforeJump += _strafePlayerSpeed * Time.deltaTime * velocity.normalized;
        //caping strafe velocity to grounded velocity
        float speedCap = _playerSpeed * Time.deltaTime;
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
