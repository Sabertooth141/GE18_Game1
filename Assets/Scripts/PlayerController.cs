using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))] 
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 1f;
    public float gravity = -9.8f;

    public Transform groundCheck;
    public float distanceToGround = .4f;
    public LayerMask groundMask;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isGrounded = true;

    private PlayerControls _inputControls;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if ( _characterController == null )
        {
            Debug.LogError("ERR: CharacterControlller not found");
        }

        _inputControls = new PlayerControls();

        _inputControls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputControls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        _inputControls.Player.Enable();
    }

    private void OnDisable()
    {
        _inputControls.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = GroundCheck();

        if (_isGrounded)
        {
            Debug.Log("1");
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;

        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(_moveInput.x, _velocity.y, _moveInput.y);
        move = transform.TransformDirection(move);

        _characterController.Move(move * walkingSpeed * Time.deltaTime);
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, distanceToGround, groundMask);
    }
}
