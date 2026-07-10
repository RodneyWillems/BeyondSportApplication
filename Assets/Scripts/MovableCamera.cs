using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovableCamera : MonoBehaviour
{
    [SerializeField] private int m_movementSpeed;

    private CameraInputs m_inputs;
    private bool m_holding;
    private Rigidbody m_rb;

    private float m_xRotation;
    private float m_yRotation;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();

        m_inputs = new();
        m_inputs.Enable();
        m_inputs.Default.Hold.started += Hold;
        m_inputs.Default.Hold.canceled += Release;
        m_inputs.Default.Speedup.started += Speedup;
        m_inputs.Default.Speedup.canceled += Speeddown;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Move()
    {
        Vector2 direction = m_inputs.Default.Move.ReadValue<Vector2>();
        Vector3 moveDirection = direction;

        if (!m_holding)
        {
            moveDirection = transform.forward * direction.y + transform.right * direction.x;
        }
        m_rb.AddForce(m_movementSpeed * moveDirection.normalized, ForceMode.Force);
    }

    private void Hold(InputAction.CallbackContext context)
    {
        m_holding = true;
    }

    private void Release(InputAction.CallbackContext context)
    {
        m_holding = false;
    }

    private void Speedup(InputAction.CallbackContext context)
    {
        m_movementSpeed *= 2;
    }

    private void Speeddown(InputAction.CallbackContext context)
    {
        m_movementSpeed /= 2;
    }

    private void Look()
    {
        Vector2 direction = m_inputs.Default.Look.ReadValue<Vector2>();

        m_xRotation -= direction.y;
        m_yRotation += direction.x;

        transform.localRotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);
    }

    private void FixedUpdate()
    {
        Move();
        Look();
    }
}
