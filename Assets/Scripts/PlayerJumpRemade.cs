using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpRemade : MonoBehaviour
{
    public ApplyGravity gravity;

    public float jumpVelocity = 8f;

    private bool _jumpPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravity = GetComponent<ApplyGravity>();
        if (gravity == null)
            Debug.LogError("ApplyGravity Component NOT FOUND");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
            _jumpPressed = true;
    }

    private void FixedUpdate()
    {
        if (!_jumpPressed)
            return;
        
        _jumpPressed = false;
        if (gravity.isGrounded)
            gravity.SetVelocity(new Vector3(0, jumpVelocity, 0));
    }
}
