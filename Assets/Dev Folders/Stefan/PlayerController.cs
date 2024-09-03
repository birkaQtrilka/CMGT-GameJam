using System.Collections;
using UnityEngine;

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

    int _currentJump;
    float _speedStartMoveTimer = 0;

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

    void FixedUpdate()
    {
        _grounded = Physics.CheckSphere(GetGroundCheckerPos(), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground"));
        
        //relative to left and right local axis
        velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * _playerSpeed * GetSmoothSpeedValue();

        if (_jumpPressed)
        {
            if(_currentJump > 0)
            {
                Jump();
                _grounded = false;
                _currentJump--;
            }
            _jumpPressed = false;

        }

        if (_grounded)
        {
            _currentJump = _availableJumps;
        }
        else
        {
            bool isFalling = _rigidbody.velocity.y < 0;
            if (isFalling && _rigidbody.velocity.y > -_fallSpeedCap)
            {
                _rigidbody.AddForce(-Vector3.up * _fallAcceleration);
            }
            Strafe();
        }

        _rigidbody.MovePosition(transform.position + velocity);

    }

    void Jump()
    {
        StopAllCoroutines();
        StartCoroutine(JumpRoutine());
        _speedBeforeJump = velocity;
    }

    IEnumerator JumpRoutine()
    {
        float time = .5f;
        while(time > 0)
        {
            //from 1 to 0
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpCurve.Evaluate(time / .5f) * _jumpPower, _rigidbody.velocity.z);
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
        return _collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y - _groundCheckerRadius);
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
