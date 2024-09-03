using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform Target;
    public float mouseSensitivity = 3;

    [SerializeField] float cameraDist;
    Vector2 mousePos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            mousePos.x -= Input.GetAxis("Mouse X");
            mousePos.y -= Input.GetAxis("Mouse Y");
        }
        if (Cursor.lockState == CursorLockMode.None) return;

        mousePos.x += Input.GetAxis("Mouse X");
        mousePos.y += Input.GetAxis("Mouse Y");
        Quaternion rot = Quaternion.AngleAxis(mousePos.x * mouseSensitivity, Vector3.up);
        Quaternion up = Quaternion.AngleAxis(mousePos.y * mouseSensitivity, Vector3.left);
        transform.rotation = rot * up;
        transform.position = Target.position - transform.forward * cameraDist + .2f * transform.right * cameraDist + .1f * transform.up * cameraDist;
    }
}
