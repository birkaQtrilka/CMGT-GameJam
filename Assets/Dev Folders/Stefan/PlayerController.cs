using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _playerSpeed;
    [SerializeField] float _playerSpeedIncrease = 8f;
    [SerializeField] float _strafePlayerSpeed;
    [SerializeField] float _jumpPower;
    [SerializeField] float _fallAcceleration;
    [SerializeField] float _fallSpeedCap;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Collider _collider;
    [SerializeField] float _groundCheckerRadius;
    [SerializeField] float _groundCheckerOffset;
    [SerializeField] AnimationCurve _dampCurve;
    Vector3 _movingSignal;
    Vector3 _speedBeforeJump;

    [Header("For debugging only, don't touch it")]
    [SerializeField] bool _jumpPressed;
    [SerializeField] bool _hasJumped;
    [SerializeField] bool _grounded;

    float _speedStartMoveTimer = 0;

    void Start()
    {
                
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
    float _airTimer;

    void Update()
    {
        _movingSignal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) ;
        _jumpPressed = Input.GetKeyDown(KeyCode.Space);
        _grounded = Physics.CheckSphere(GetGroundCheckerPos(), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground"));

        //relative to left and right local axis
        Vector3 velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * _playerSpeed * GetSmoothSpeedValue();

        if (!_grounded)
        {
            //keeping velocity while jumping
            _speedBeforeJump += _strafePlayerSpeed * Time.deltaTime * velocity.normalized;
            //caping strafe velocity to grounded velocity
            float speedCap = _playerSpeed * Time.deltaTime;
            if (_speedBeforeJump.magnitude > speedCap)
                _speedBeforeJump = _speedBeforeJump.normalized * speedCap;
            velocity = _speedBeforeJump;
            _airTimer += Time.deltaTime;
        }

        if (_jumpPressed && _grounded && !_hasJumped)
        {
            _rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);

            _speedBeforeJump = velocity;
            _hasJumped = true;
            _jumpPressed = false;
            _grounded = false;
        }

        bool isRising = _rigidbody.velocity.y < 0;

        if (_hasJumped && !_grounded && isRising)
        {
            if (_rigidbody.velocity.y > -_fallSpeedCap)
                _rigidbody.AddForce(-Vector3.up * _fallAcceleration);

        }

        if (_grounded && _airTimer > .02f)
        {
            _hasJumped = false;
            _airTimer = 0;
            //_rigidbody.velocity = new Vector3();
        }

        transform.position += velocity;
        //_rigidbody.MovePosition(transform.position + );
    }

    Vector3 GetGroundCheckerPos()
    {
        return _collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y + _groundCheckerOffset);
    }

    float GetSmoothSpeedValue()
    {
        if (_movingSignal.sqrMagnitude != 0)
            _speedStartMoveTimer += Time.deltaTime * _playerSpeedIncrease;
        else _speedStartMoveTimer = 0;

        _speedStartMoveTimer = _dampCurve.Evaluate(Mathf.Min(_speedStartMoveTimer, 1));
        return _speedStartMoveTimer;
    }
}
