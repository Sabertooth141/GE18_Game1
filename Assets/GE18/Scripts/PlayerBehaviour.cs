using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private Vector3 prevPos;
    public GameObject pivot;
    public CameraContorol cameraControl;
    private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Collision Settings")]
    public float wallCheckDistance = 0.5f;
    public float characterRadius = 0.3f;

    [Tooltip("トリガーコライダーを壁として扱うか")]
    public bool detectTriggers = false;

    [Header("Layer Settings")]
    [Tooltip("壁検出に使用するレイヤー")]
    [SerializeField] private LayerMask detectionLayers = ~0;

    private Vector3 pivotOffset;
    private Collider[] myColliders;

    // 入力値をキャッシュ
    private Vector2 inputVector;

    void Start()
    {
        prevPos = transform.position;
        if (pivot == null)
            pivot = GameObject.Find("Pivot");
        if (cameraControl == null)
            cameraControl = GameObject.Find("MainCamera").GetComponent<CameraContorol>();
        animator = GetComponent<Animator>();

        pivotOffset = pivot.transform.position - transform.position;

        // 自分自身のコライダーを取得（無視するため）
        myColliders = GetComponentsInChildren<Collider>();
    }

    // 入力処理は Update で行う
    void Update()
    {
        inputVector.x = 0f;
        inputVector.y = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            inputVector.y = 1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            inputVector.y = -1f;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            inputVector.x = -1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            inputVector.x = 1f;
        }
    }

    // 物理処理は FixedUpdate で行う
    void FixedUpdate()
    {
        float horizontal = inputVector.x;
        float vertical = inputVector.y;

        bool isMoving = vertical != 0f || horizontal != 0f;

        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isIdle", !isMoving);

        if (isMoving)
        {
            Vector3 cameraForward = pivot.transform.forward;
            Vector3 cameraRight = pivot.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                Vector3 moveAmount = moveDirection * moveSpeed * Time.fixedDeltaTime;
                Vector3 finalMove = GetWallSlideMovement(moveAmount);
                transform.position += finalMove;
            }
        }

        prevPos = transform.position;

        pivot.transform.position = transform.position + pivotOffset;
    }

    Vector3 GetWallSlideMovement(Vector3 moveAmount)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        // トリガーの扱いを設定
        QueryTriggerInteraction triggerInteraction = detectTriggers
            ? QueryTriggerInteraction.Collide
            : QueryTriggerInteraction.Ignore;

        // まず、近接するコライダーをチェック（埋まっている場合の対応）
        Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position,
            characterRadius + 0.1f,
            detectionLayers,
            triggerInteraction
        );

        // 近接コライダーから壁を探す
        foreach (Collider col in nearbyColliders)
        {
            if (IsMyCollider(col))
                continue;

            // 最も近い点を取得
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            Vector3 directionToWall = (closestPoint - transform.position).normalized;

            // 移動方向と壁の方向が似ている場合（壁に向かって移動している）
            if (Vector3.Dot(moveAmount.normalized, directionToWall) > 0.5f)
            {
                Vector3 wallNormal = (transform.position - closestPoint).normalized;
                Vector3 slideDirection = Vector3.ProjectOnPlane(moveAmount, wallNormal);
                return slideDirection;
            }
        }

        // 次に、通常のSphereCastで前方の壁をチェック
        RaycastHit[] hits = Physics.SphereCastAll(
            rayOrigin,
            characterRadius,
            moveAmount.normalized,
            wallCheckDistance,
            detectionLayers,
            triggerInteraction
        );

        // 自分以外の最も近いヒットを探す
        RaycastHit? closestHit = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            // 自分自身のコライダーをスキップ
            if (IsMyCollider(hit.collider))
                continue;

            // 最も近いヒットを記録
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestHit = hit;
            }
        }

        // 壁にヒットした場合、壁沿いに移動
        if (closestHit.HasValue)
        {
            Vector3 wallNormal = closestHit.Value.normal;
            Vector3 slideDirection = Vector3.ProjectOnPlane(moveAmount, wallNormal);
            return slideDirection;
        }

        return moveAmount;
    }

    /// <summary>
    /// 指定されたコライダーが自分自身のものかチェック
    /// </summary>
    private bool IsMyCollider(Collider collider)
    {
        if (myColliders == null || myColliders.Length == 0)
            return false;

        foreach (Collider myCollider in myColliders)
        {
            if (myCollider == collider)
                return true;
        }

        return false;
    }
}
