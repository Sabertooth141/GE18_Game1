using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class CameraContorol : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;
    public GameObject pivot;
    public GameObject lookTarget;

    public float mouseSensitivity = 300f;
    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        LockCursor();

        if (pivot == null)
            pivot = GameObject.Find("Pivot");

        if (lookTarget == null)
            lookTarget = GameObject.Find("Player");

        if (playerBehaviour == null)
            playerBehaviour = lookTarget.GetComponent<PlayerBehaviour>();
    }

    private void Update()
    {
        // Ctrlキーでカーソルのロック/解除を切り替え
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        // カーソルが解除されている状態でマウスクリックしたら再ロック
        else if (UnityEngine.Cursor.lockState == CursorLockMode.None && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    void LateUpdate()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * (mouseSensitivity / 100f);
        float mouseY = mouseDelta.y * (mouseSensitivity / 100f);

        // 縦方向の回転量を累積
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 横方向の回転量を累積
        float yRotation = pivot.transform.localEulerAngles.y + mouseX;

        // pivotに回転を適用
        pivot.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void LockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    // カーソルのロックを解除する
    void UnlockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

}