using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _playerSpeed;
    [SerializeField] float _strafePlayerSpeed;
    [SerializeField] float _jumpPower;
    [SerializeField] float _fallAcceleration;
    [SerializeField] float _fallSpeedCap;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Collider _collider;
    [SerializeField] float _groundCheckerRadius;
    [SerializeField] float _groundCheckerOffset;

    Vector3 _movingSignal;
    Vector3 _speedBeforeJump;

    [SerializeField] bool _jumpPressed;
    [SerializeField] bool _hasJumped;
    [SerializeField] bool _grounded;

    void Start()
    {
                
    }

    void OnDrawGizmos()
    {
        if (_collider == null) return;
        if (Physics.CheckSphere(_collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y + _groundCheckerOffset), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground") ))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawSphere(_collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y + _groundCheckerOffset), _groundCheckerRadius);
    }
    float timer;

    void Update()
    {
        _movingSignal = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical")) ;
        _jumpPressed = Input.GetKeyDown(KeyCode.Space);
        _grounded = Physics.CheckSphere(_collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y + _groundCheckerOffset), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground"));

        Vector3 velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * _playerSpeed ;
        Vector3 jump = Vector3.zero;

        if (!_grounded)
        {
            _speedBeforeJump += _strafePlayerSpeed * Time.deltaTime * velocity.normalized;
            float speedCap = _playerSpeed * Time.deltaTime;
            if (_speedBeforeJump.magnitude > speedCap)
                _speedBeforeJump = _speedBeforeJump.normalized * speedCap;
            velocity = _speedBeforeJump;
            timer += Time.deltaTime;
        }

        if (_jumpPressed && _grounded && !_hasJumped)
        {
            jump += Vector3.up * _jumpPower;
            _speedBeforeJump = velocity;
            _hasJumped = true;
            _jumpPressed = false;
            _grounded = false;
        }

        if (_hasJumped && !_grounded && _rigidbody.velocity.y < 0)
        {
            if (_rigidbody.velocity.y > -_fallSpeedCap)
                jump.y -= _fallAcceleration;

        }
        if (_grounded && timer > .05f)
        {
            _hasJumped = false;
            timer = 0;
        }


        transform.position += velocity;
        //_rigidbody.MovePosition(transform.position + );
        _rigidbody.AddForce(jump);
    }

}
