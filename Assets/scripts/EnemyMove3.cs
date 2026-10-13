
using UnityEngine;

// Lv3：ついてきて一定範囲に入ると攻撃する敵
public class EnemyMove3 : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("設定")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float checkRadius = 3f;
    [SerializeField] private float attackCharge = 2f;
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;

    private Transform player;
    private bool isPlayerInArea;
    private bool isAttack;
    private float chargeTimer;
    private string currentAnimation = "";

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 子オブジェクトのAnimatorを自動取得
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // 子オブジェクトのSpriteRendererを自動取得
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        isPlayerInArea = false;
        isAttack = false;

        ChangeAnimation("damonWalk");
    }

    void Update()
    {
        if (!GameDirector.Instance.isGaming || player == null)
            return;

        // プレイヤーが攻撃範囲内にいるか判定
        Collider2D[] hitColliders =
            Physics2D.OverlapCircleAll(transform.position, checkRadius);

        isPlayerInArea = false;

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                isPlayerInArea = true;
                break;
            }
        }

        // プレイヤーの方向を取得
        Vector2 direction =
            ((Vector2)player.position - (Vector2)transform.position).normalized;

        // 左右反転：子オブジェクトのスプライトだけを反転
        if (spriteRenderer != null)
        {
            if (direction.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }

        if (isAttack)
        {
            // 攻撃中は移動しない
            return;
        }

        // 攻撃チャージ
        if (isPlayerInArea)
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= attackCharge)
            {
                ExecuteAttack();
                return;
            }
        }
        else
        {
            // 範囲外ならチャージをリセット
            chargeTimer = 0f;
        }

        // 通常時は追跡しながら歩く
        ChangeAnimation("damonWalk");

        transform.position +=
            (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    // アニメーション変更
    private void ChangeAnimation(string animationName)
    {
        if (animator == null || currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName, 0, 0f);
    }

    // 爆破オブジェクトに触れると消える
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
    }

    // 攻撃処理
    private void ExecuteAttack()
    {
        isAttack = true;
        chargeTimer = 0f;

        // 攻撃アニメーションを再生
        ChangeAnimation("damonAttack");

        Vector3 spawnPosition;

        if (player.position.x - transform.position.x <= 0)
        {
            spawnPosition =
                transform.position + new Vector3(offsetX, offsetY, 0);
        }
        else
        {
            spawnPosition =
                transform.position + new Vector3(-offsetX, offsetY, 0);
        }

        // 攻撃エフェクト生成
        if (attackEffect != null)
        {
            Instantiate(attackEffect, spawnPosition, Quaternion.identity);
        }

        Invoke(nameof(ResetAttack), attackDuration);
    }

    // 攻撃終了
    private void ResetAttack()
    {
        isAttack = false;
        isPlayerInArea = false;
        chargeTimer = 0f;

        ChangeAnimation("damonWalk");
    }

    // デバッグ用：攻撃範囲を表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
