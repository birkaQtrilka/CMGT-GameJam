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

    void Update()
    {
        _movingSignal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) ;
        _jumpPressed = Input.GetKeyDown(KeyCode.Space);
        _grounded = Physics.CheckSphere(_collider.transform.position - _collider.transform.up * (_collider.bounds.extents.y + _groundCheckerOffset), _groundCheckerRadius, 1 << LayerMask.NameToLayer("Ground"));

        Vector3 velocity = (transform.right * _movingSignal.x + transform.forward * _movingSignal.z).normalized;
        velocity *= Time.deltaTime * (_grounded ? _playerSpeed : _strafePlayerSpeed);
        Vector3 jump = Vector3.zero;

        if (_grounded) _hasJumped = false;

        if (_jumpPressed && _grounded && !_hasJumped)
        {
            jump += Vector3.up * _jumpPower;
            _hasJumped = true;
            _jumpPressed = false;
        }

        if (_hasJumped && !_grounded && _rigidbody.velocity.y <= 0)
        {
            Debug.Log("In 1");
            if (_rigidbody.velocity.y > -_fallSpeedCap)
            {
                Debug.Log("down");
                jump.y -= _fallAcceleration;
            }
        }


        _rigidbody.MovePosition(transform.position + velocity);
        _rigidbody.AddForce(jump);
    }

}
