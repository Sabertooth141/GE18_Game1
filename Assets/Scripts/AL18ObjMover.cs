using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class AL18ObjMover : MonoBehaviour
{
    InputAction moveAction;
    Keyboard currentKeyboard;
    KeyControl currKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentKeyboard = Keyboard.current;
        if (currentKeyboard == null)
        {
            Debug.LogError("currentKeyboard Not Found");
        }

        moveAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        currentKeyboard = Keyboard.current;
        currKey = currentKeyboard.aKey; 
        if (currKey.wasPressedThisFrame)
        {
            Debug.Log("AAAAA");
        }

        if (currKey.wasReleasedThisFrame)
        {
            Debug.Log("aaaaaaa");
        }

        if (currKey.isPressed)
        {
            Debug.Log("a");
        }

        if (moveAction.IsPressed())
        {
            Debug.Log("PSACEASJDASPODJAOSIDJA");
        }

    }
}
