
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private GameDirector gameDirector;
    [SerializeField] private GameObject ExplosionEffect;
    [SerializeField] private Animator animator;

    [Header("サウンド")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip airDodgeSound;

    [Header("変数")]
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxAirDodge = 1;
    [SerializeField] private float dodgeSpeed = 2f;

    [Header("接地判定")]
    [SerializeField] private float groundRayDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("アニメーション")]
    [SerializeField] private float rollingDuration = 0.35f;

    [Header("向きの反転スプライト")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("空中移動")]
    [SerializeField, Range(0f, 1f)]
    private float airMoveMultiplier = 0.5f;

    private Rigidbody2D rb;
    private Vector2 move;
    private bool isGround;
    private bool isRolling;
    private int currentAirDodge;
    private float defaultGravity;
    private string currentAnimation = "";

    //タイムアップの際に連続で爆発しないようにする変数
    private bool exploded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentAirDodge = 0;
        defaultGravity = rb.gravityScale;

        if (gameDirector == null)
            gameDirector = FindFirstObjectByType<GameDirector>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ChangeAnimation("playerIdle");

        exploded = false;
    }

    void Update()
    {
        if (!GameDirector.Instance.isGaming)
            return;

        CheckGround();

        move = moveAction.action.ReadValue<Vector2>();


        if (!isRolling)
        {
            // 空中なら移動速度に倍率をかける
            float currentMoveSpeed = isGround
                ? moveSpeed
                : moveSpeed * airMoveMultiplier;

            // 反対方向の入力で空中回避の慣性をキャンセル
            if (Mathf.Abs(move.x) > 0.01f &&
                Mathf.Abs(rb.linearVelocity.x) > 0.01f &&
                Mathf.Sign(move.x) != Mathf.Sign(rb.linearVelocity.x))
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            transform.position +=
                Vector3.right * move.x * currentMoveSpeed * Time.deltaTime;
        }


        // 移動方向に応じて左右反転
        if (spriteRenderer != null)
        {
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;
        }

        if (jumpAction.action.WasPressedThisFrame())
        {
            if (isGround)
            {
                Jump();
            }
            else if (currentAirDodge < maxAirDodge && !isRolling)
            {
                currentAirDodge++;
                StartCoroutine(AirDodge());
            }
        }

        UpdateAnimation();

        if (!GameDirector.Instance.isLiving && exploded == false)
        {
            exploded = true;
            Instantiate(ExplosionEffect);
        }
    }

    // 効果音再生
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // アニメーション切り替え
    private void UpdateAnimation()
    {
        if (isRolling)
            return;

        if (isGround)
        {
            if (Mathf.Abs(move.x) > 0.01f)
                ChangeAnimation("playerRun");
            else
                ChangeAnimation("playerIdle");

            return;
        }

        if (rb.linearVelocity.y > 0.01f)
        {
            ChangeAnimation("playerJump");
        }
        else if (rb.linearVelocity.y < -0.01f)
        {
            ChangeAnimation("Falling");
        }
    }

    private void ChangeAnimation(string animationName)
    {
        if (animator == null || currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName, 0, 0f);
    }


    private void CheckGround()
    {
        // プレイヤーの左右から下向きにRayを飛ばす
        float rayOffset = 0.2f;

        Vector2 leftOrigin = (Vector2)transform.position
            + Vector2.left * rayOffset;

        Vector2 rightOrigin = (Vector2)transform.position
            + Vector2.right * rayOffset;

        RaycastHit2D leftHit = Physics2D.Raycast(
            leftOrigin,
            Vector2.down,
            groundRayDistance,
            groundLayer
        );

        RaycastHit2D rightHit = Physics2D.Raycast(
            rightOrigin,
            Vector2.down,
            groundRayDistance,
            groundLayer
        );

        bool wasGround = isGround;
        isGround = leftHit.collider != null
                || rightHit.collider != null;

        // 着地したら空中回避回数をリセット
        if (isGround && !wasGround)
        {
            currentAirDodge = 0;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Attack")) && exploded == false)
        {
            exploded = true;
            gameDirector.Restart();
            Instantiate(ExplosionEffect);
        }

        if (collision.gameObject.CompareTag("Explosion"))
        {
            StartCoroutine(DestroyOffset());
        }

        if (collision.gameObject.CompareTag("Boss") && exploded == false)
        {
            exploded = true;
            Instantiate(ExplosionEffect);
            GameDirector.Instance.isGaming = false;
            gameDirector.Clear();
        }
    }

    // ジャンプ
    private void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        // ジャンプした瞬間に再生
        PlaySound(jumpSound);
    }

    // 空中回避
    private IEnumerator AirDodge()
    {
        isRolling = true;
        ChangeAnimation("Rolling");

        // ロール開始時に再生
        PlaySound(airDodgeSound);

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(
            move.x * dodgeSpeed * 2f,
            0f
        );

        yield return new WaitForSeconds(rollingDuration);

        rb.gravityScale = defaultGravity;
        isRolling = false;

        UpdateAnimation();
    }

    //すぐにゲームオーバー処理を行うとフリーズする可能性があったので回避
    private IEnumerator DestroyOffset()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundRayDistance
        );
    }
}
