using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float gravity = -9.8f;

    public float lookSense = 1.5f;
    public float pitchClamp = 80f;
    public Transform playerCamera;

    private CharacterController _playerController;
    private Vector3 _velocity;
    private float _xRotation;

    public InputAction moveAction;
    public InputAction lookAction;

    private bool _canMove = false;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();    
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = GetComponent<CharacterController>();

        if (!_playerController)
        {
            Debug.LogError("No player controller found");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.isGrounded && !_canMove)
        {
            _canMove = true;
        }
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _playerController.Move(move * moveSpeed * Time.deltaTime);

        if (_playerController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _playerController.Move(_velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = _lookInput.x * lookSense;
        float mouseY = _lookInput.y * lookSense;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -pitchClamp, pitchClamp);

        playerCamera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up, mouseX);
    }

    public void OnMove(InputValue value)
    {
        if (!_canMove)
            return;
        _moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }
}