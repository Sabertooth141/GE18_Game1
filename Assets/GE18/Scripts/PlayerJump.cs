using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// スペースキーでジャンプする機能
/// ApplyGravityスクリプトと連携して動作
/// </summary>
public class PlayerJump : MonoBehaviour
{
    [Header("ジャンプ設定")]
    [Tooltip("ジャンプの初速度")]
    public float jumpForce = 8f;

    [Tooltip("ジャンプ可能な最大回数（1で単発ジャンプ、2で二段ジャンプ）")]
    public int maxJumpCount = 1;

    [Header("参照")]
    [Tooltip("ApplyGravityコンポーネント（自動取得）")]
    public ApplyGravity gravityComponent;

    [Header("デバッグ")]
    [Tooltip("ジャンプ情報をログに表示")]
    public bool showDebugLog = false;

    // 現在のジャンプ回数
    private int currentJumpCount = 0;

    // 前フレームの接地状態
    private bool wasGroundedLastFrame = false;

    // ジャンプ入力フラグ
    private bool jumpInputPressed = false;

    void Start()
    {
        // ApplyGravityコンポーネントを自動取得
        if (gravityComponent == null)
        {
            gravityComponent = GetComponent<ApplyGravity>();

            if (gravityComponent == null)
            {
                Debug.LogError("[PlayerJump] ApplyGravityコンポーネントが見つかりません！同じGameObjectにアタッチしてください。");
            }
        }
    }

    // 入力処理はUpdateで
    void Update()
    {
        // スペースキーが押されたかチェック
        if (Keyboard.current.spaceKey.isPressed)
        {
            jumpInputPressed = true;
        }
    }

    // 物理処理はFixedUpdateで
    void FixedUpdate()
    {
        if (gravityComponent == null)
            return;

        // 接地状態を確認
        bool isGrounded = gravityComponent.IsGrounded();

        // 着地したらジャンプ回数をリセット
        if (isGrounded && !wasGroundedLastFrame)
        {
            currentJumpCount = 0;

            if (showDebugLog)
            {
                Debug.Log("[PlayerJump] 着地 - ジャンプ回数リセット");
            }
        }

        // ジャンプ入力があった場合
        if (jumpInputPressed)
        {
            jumpInputPressed = false; // フラグをリセット
            TryJump();
        }

        // 前フレームの接地状態を保存
        wasGroundedLastFrame = isGrounded;
    }

    /// <summary>
    /// ジャンプを試みる
    /// </summary>
    private void TryJump()
    {
        // 地上にいる場合、または空中ジャンプ可能な場合
        if (CanJump())
        {
            // ジャンプ実行
            Vector3 currentVelocity = gravityComponent.GetVelocity();

            // Y方向の速度をジャンプ力に設定（X, Z方向は維持）
            currentVelocity.y = jumpForce;

            // SetVelocityを呼ぶと自動的にisGroundedがfalseになる
            gravityComponent.SetVelocity(currentVelocity);

            currentJumpCount++;

            if (showDebugLog)
            {
                Debug.Log($"[PlayerJump] ジャンプ！ ({currentJumpCount}/{maxJumpCount}) 初速: {jumpForce}");
            }
        }
        else
        {
            if (showDebugLog)
            {
                Debug.Log($"[PlayerJump] ジャンプ不可 - すでに{maxJumpCount}回ジャンプ済み");
            }
        }
    }

    /// <summary>
    /// ジャンプ回数を強制リセット（外部から呼び出し可能）
    /// </summary>
    public void ResetJumpCount()
    {
        currentJumpCount = 0;

        if (showDebugLog)
        {
            Debug.Log("[PlayerJump] ジャンプ回数を手動でリセット");
        }
    }

    /// <summary>
    /// 現在のジャンプ回数を取得
    /// </summary>
    public int GetCurrentJumpCount()
    {
        return currentJumpCount;
    }

    /// <summary>
    /// まだジャンプ可能かチェック
    /// </summary>
    public bool CanJump()
    {
        if (gravityComponent == null)
            return false;

        return gravityComponent.IsGrounded() || currentJumpCount < maxJumpCount;
    }
}
