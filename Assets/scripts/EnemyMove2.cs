
using UnityEngine;

// Lv2：ついてくる敵
public class EnemyMove2 : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("スプライト")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 子オブジェクトも含めてSpriteRendererを取得
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (player == null) return;

        if (GameDirector.Instance.isGaming)
        {
            Vector2 direction =
                ((Vector2)player.position - (Vector2)transform.position).normalized;

            // プレイヤーに向かって移動
            transform.position +=
                (Vector3)direction * moveSpeed * Time.deltaTime;

            // 左右の移動方向に応じてスプライトを反転
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
        }
    }

    // 爆破オブジェクトに触れると消える
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
    }
}
