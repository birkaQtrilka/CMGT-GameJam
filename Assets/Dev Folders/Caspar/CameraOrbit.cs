using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 3;

    [SerializeField] float _xClampMin;
    [SerializeField] float _xClampMax;
    [SerializeField] Vector3 _offset = new (2f, 1f,10);

    Vector2 _mousePos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            _mousePos.x -= Input.GetAxis("Mouse X");
            _mousePos.y -= Input.GetAxis("Mouse Y");
        }
        if (Cursor.lockState == CursorLockMode.None) return;

        _mousePos.x += Input.GetAxis("Mouse X");
        _mousePos.y += Input.GetAxis("Mouse Y");
        _mousePos.y = Mathf.Clamp(_mousePos.y, _xClampMin / MouseSensitivity, _xClampMax / MouseSensitivity);
        Quaternion rot = Quaternion.AngleAxis(_mousePos.x * MouseSensitivity, Vector3.up);
        Quaternion up = Quaternion.AngleAxis(_mousePos.y * MouseSensitivity, Vector3.left);

        transform.rotation = rot * up;
        transform.position = Target.position - transform.forward * _offset.z + _offset.x * transform.right + _offset.y * transform.up;

        Target.transform.rotation = rot;
    }
}
