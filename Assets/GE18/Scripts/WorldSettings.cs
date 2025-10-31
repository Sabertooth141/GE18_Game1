using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    public static WorldSettings Instance;

    [Header("Physics Settings")]
    public float gravity = 9.8f;

    [Tooltip("地面として扱うレイヤー")]
    public LayerMask groundLayer;

    [Tooltip("壁として扱うレイヤー")]
    public LayerMask wallLayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}