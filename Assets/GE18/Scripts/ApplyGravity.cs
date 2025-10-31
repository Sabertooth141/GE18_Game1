using UnityEngine;

/// <summary>
/// 物理演算を使わずに、WorldSettingsの重力を参照してオブジェクトを落下させるスクリプト
/// 足元のレイキャストで地面を検出（Y座標が負の値にも対応）
/// </summary>
public class ApplyGravity : MonoBehaviour
{
    [Header("落下設定")]
    [Tooltip("地面のY座標（レイキャストが当たらない場合のフォールバック）\n負の値も設定可能")]
    public float groundLevel = 0f;

    [Tooltip("既に地面に着いているか")]
    public bool isGrounded = false;

    [Tooltip("着地時の跳ね返り係数 (0で跳ね返らない、1で完全に跳ね返る)")]
    [Range(0f, 1f)]
    public float bounciness = 0f;

    [Tooltip("速度の減衰率（着地後の滑り）")]
    [Range(0f, 1f)]
    public float friction = 0.9f;

    [Header("地面検出設定")]
    [Tooltip("足元から下方向への地面検出距離")]
    public float groundCheckDistance = 1f;

    [Tooltip("足元のレイキャスト開始位置のオフセット（Y軸）")]
    public float raycastOriginOffset = 0.2f;

    [Tooltip("レイキャストの半径（0でRaycast、0より大きいとSphereCast）")]
    public float groundCheckRadius = 0.2f;

    [Tooltip("トリガーコライダーを地面として扱うか")]
    public bool detectTriggers = false;

    [Header("Layer Settings")]
    [Tooltip("地面検出に使用するレイヤー")]
    [SerializeField] private LayerMask detectionLayers = ~0;

    [Header("安全設定")]
    [Tooltip("無限落下防止機能を有効にするか")]
    public bool enableFallLimit = true;

    [Tooltip("落下限界のY座標（これより下には落ちない）\n無効にするには非常に小さい値（-10000など）を設定")]
    public float fallLimitY = -100f;

    [Tooltip("落下限界に到達したときの動作")]
    public FallLimitBehavior fallLimitBehavior = FallLimitBehavior.StopAtLimit;

    [Header("デバッグ")]
    [Tooltip("地面検出のレイを表示するか")]
    public bool showDebugRay = true;

    [Tooltip("現在の状態をコンソールに表示")]
    public bool showDebugLog = false;

    // 現在の速度
    private Vector3 velocity = Vector3.zero;

    // 自分自身のコライダー（無視するため）
    private Collider[] myColliders;

    // 落下限界に到達したか
    private bool hasReachedFallLimit = false;

    public enum FallLimitBehavior
    {
        StopAtLimit,        // 落下限界で停止
        RespawnAtGround,    // groundLevelの位置に戻す
        Destroy             // オブジェクトを破棄
    }

    void Start()
    {
        // 自分自身のコライダーを取得
        myColliders = GetComponentsInChildren<Collider>();

        if (showDebugLog)
        {
            Debug.Log($"[ApplyGravity] 初期化完了 - Ground Level: {groundLevel}, Fall Limit: {(enableFallLimit ? fallLimitY.ToString() : "無効")}");
        }
    }

    void FixedUpdate()
    {
        // WorldSettingsが存在しない場合はエラー
        if (WorldSettings.Instance == null)
        {
            Debug.LogError("WorldSettingsが見つかりません。シーン内にWorldSettingsオブジェクトを配置してください。");
            return;
        }

        // 落下限界チェック
        if (enableFallLimit && transform.position.y < fallLimitY)
        {
            HandleFallLimit();
            return;
        }

        // 地面に着いていない場合
        if (!isGrounded)
        {
            // WorldSettingsの重力を加速度として適用
            velocity.y -= WorldSettings.Instance.gravity * Time.fixedDeltaTime;

            // 位置を更新（先に移動）
            Vector3 nextPosition = transform.position + velocity * Time.fixedDeltaTime;

            // 上昇中（かつある程度の速度がある場合）は地面判定をスキップ
            bool isRising = velocity.y > 0.5f;

            // 足元の地面をチェック（下降中または低速時）
            if (!isRising && CheckGround(out RaycastHit hitInfo))
            {
                // 次のフレームで地面を貫通するかチェック
                if (nextPosition.y <= hitInfo.point.y)
                {
                    // 地面の位置に補正して着地
                    nextPosition.y = hitInfo.point.y;

                    if (showDebugLog)
                    {
                        Debug.Log($"[ApplyGravity] 着地！ Y座標: {hitInfo.point.y:F2}");
                    }

                    // 跳ね返り処理
                    if (bounciness > 0.01f && Mathf.Abs(velocity.y) > 0.1f)
                    {
                        velocity.y = -velocity.y * bounciness;
                    }
                    else
                    {
                        // 跳ね返りがほぼない、または速度が小さい場合は着地
                        velocity.y = 0f;
                        isGrounded = true;
                    }

                    // 水平方向の速度に摩擦を適用
                    velocity.x *= friction;
                    velocity.z *= friction;
                }
                // まだ地面より上なら、そのまま落下を続ける
            }
            // Y座標による地面判定（Ground Levelに到達または通過している場合）
            else if (!isRising && nextPosition.y <= groundLevel)
            {
                // Ground Levelの位置に補正
                nextPosition.y = groundLevel;

                if (showDebugLog)
                {
                    Debug.Log($"[ApplyGravity] Ground Levelで着地 Y座標: {groundLevel:F2}");
                }

                if (bounciness > 0.01f && Mathf.Abs(velocity.y) > 0.1f)
                {
                    velocity.y = -velocity.y * bounciness;
                }
                else
                {
                    velocity.y = 0f;
                    isGrounded = true;
                }

                velocity.x *= friction;
                velocity.z *= friction;
            }

            // 最終的な位置を適用
            transform.position = nextPosition;
        }
        else
        {
            // 着地後も地面をチェック（落下する地形の場合）
            if (!CheckGround(out RaycastHit hitInfo))
            {
                // 地面がなくなったら再び落下
                isGrounded = false;

                if (showDebugLog)
                {
                    Debug.Log($"[ApplyGravity] 地面がなくなった！再び落下開始");
                }
            }
            else
            {
                // 着地後も摩擦を適用して徐々に停止
                velocity.x *= friction;
                velocity.z *= friction;

                // ほぼ停止したら完全に停止
                if (velocity.magnitude < 0.01f)
                {
                    velocity = Vector3.zero;
                }
                else
                {
                    transform.position += velocity * Time.fixedDeltaTime;
                }
            }
        }
    }

    /// <summary>
    /// 足元の地面をレイキャストでチェック
    /// </summary>
    private bool CheckGround(out RaycastHit hitInfo)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * raycastOriginOffset;
        Vector3 rayDirection = Vector3.down;

        QueryTriggerInteraction triggerInteraction = detectTriggers
            ? QueryTriggerInteraction.Collide
            : QueryTriggerInteraction.Ignore;

        // デバッグ表示
        if (showDebugRay)
        {
            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(rayOrigin, rayDirection * groundCheckDistance, rayColor);
        }

        RaycastHit[] hits;

        // SphereCastまたはRaycastを実行
        if (groundCheckRadius > 0f)
        {
            hits = Physics.SphereCastAll(
                rayOrigin,
                groundCheckRadius,
                rayDirection,
                groundCheckDistance,
                detectionLayers,
                triggerInteraction
            );
        }
        else
        {
            hits = Physics.RaycastAll(
                rayOrigin,
                rayDirection,
                groundCheckDistance,
                detectionLayers,
                triggerInteraction
            );
        }

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

        if (closestHit.HasValue)
        {
            hitInfo = closestHit.Value;
            return true;
        }

        hitInfo = default;
        return false;
    }

    /// <summary>
    /// 落下限界に到達したときの処理
    /// </summary>
    private void HandleFallLimit()
    {
        if (hasReachedFallLimit)
            return;

        hasReachedFallLimit = true;

        if (showDebugLog)
        {
            Debug.LogWarning($"[ApplyGravity] 落下限界に到達！ Y座標: {transform.position.y:F2}");
        }

        switch (fallLimitBehavior)
        {
            case FallLimitBehavior.StopAtLimit:
                // 落下限界で停止
                transform.position = new Vector3(
                    transform.position.x,
                    fallLimitY,
                    transform.position.z
                );
                velocity = Vector3.zero;
                isGrounded = true;
                break;

            case FallLimitBehavior.RespawnAtGround:
                // groundLevelの位置に戻す
                transform.position = new Vector3(
                    transform.position.x,
                    groundLevel,
                    transform.position.z
                );
                velocity = Vector3.zero;
                isGrounded = false;
                hasReachedFallLimit = false;

                if (showDebugLog)
                {
                    Debug.Log($"[ApplyGravity] Ground Levelにリスポーン Y座標: {groundLevel:F2}");
                }
                break;

            case FallLimitBehavior.Destroy:
                // オブジェクトを破棄
                Debug.Log($"[ApplyGravity] オブジェクトを破棄: {gameObject.name}");
                Destroy(gameObject);
                break;
        }
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

    /// <summary>
    /// 再び落下させる（リセット用）
    /// </summary>
    public void ResetFall()
    {
        isGrounded = false;
        velocity = Vector3.zero;
        hasReachedFallLimit = false;

        if (showDebugLog)
        {
            Debug.Log($"[ApplyGravity] リセット");
        }
    }

    /// <summary>
    /// 初速度を設定する
    /// </summary>
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
        isGrounded = false;
        hasReachedFallLimit = false;
    }

    /// <summary>
    /// 現在の速度を取得
    /// </summary>
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    /// <summary>
    /// 地面に着いているかを取得
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// 現在のY座標を取得
    /// </summary>
    public float GetCurrentHeight()
    {
        return transform.position.y;
    }

    /// <summary>
    /// 地面からの高さを取得
    /// </summary>
    public float GetHeightFromGround()
    {
        return transform.position.y - groundLevel;
    }
}
