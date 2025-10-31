using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Transform cam;

    [Header("Settings")]
    public float distance = 3f;
    public float sensitivity = 100f;
    public float minY = -40f;
    public float maxY = 80f;
    public float smoothSpeed = 10f;

    private PlayerControls controls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
