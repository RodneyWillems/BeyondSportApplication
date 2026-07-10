using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovableCamera : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_movementSpeed;

    private CameraInputs m_inputs;
    private bool m_holding;
    private Rigidbody m_rb;

    private float m_xRotation;
    private float m_yRotation;
    #endregion
    #region Setup
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
    #endregion
    #region Inputs
    private void Move()
    {
        Vector2 direction = m_inputs.Default.Move.ReadValue<Vector2>();
        Vector3 moveDirection = direction;

        // Check if right click is being hold before moving
        if (!m_holding)
        {
            moveDirection = transform.forward * direction.y + transform.right * direction.x;
        }
        m_rb.AddForce(m_movementSpeed * moveDirection.normalized, ForceMode.Force);
    }

    private void Hold(InputAction.CallbackContext context)
    {
        // When holding right click change the movement directions
        m_holding = true;
    }

    private void Release(InputAction.CallbackContext context)
    {
        // When releasing right click change the movement directions back
        m_holding = false;
    }

    private void Speedup(InputAction.CallbackContext context)
    {
        // When holding shift allow a faster speed
        m_movementSpeed *= 2;
    }

    private void Speeddown(InputAction.CallbackContext context)
    {
        // When releasing shift return back to the old speed
        m_movementSpeed /= 2;
    }

    private void Look()
    {
        // Save the rotations when looking around so the camera doesn't reset every other frame
        Vector2 direction = m_inputs.Default.Look.ReadValue<Vector2>();

        m_xRotation -= direction.y;
        m_yRotation += direction.x;

        transform.localRotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);
    }
    #endregion
    private void FixedUpdate()
    {
        Move();
        Look();
    }
}
