using NUnit.Framework.Constraints;
using UnityEngine;

//Lv3：ついてきて一定範囲に入ると攻撃する敵
public class EnemyMove3 : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject attackEffect;

    [Header("設定")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float checkRadius = 3f;    //プレイヤーの接近チェック用
    [SerializeField] private float attackCharge = 2f;   //接近したときに何秒で攻撃するか
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;

    //プレイヤーの判定中用変数
    private bool isPlayerInArea = false;
    //攻撃中かどうかを記録する変数
    private bool isAttack = false;
    //プレイヤーが判定範囲に入ってからの秒数格納用
    private float chargeTimer = 0;
    //プレイヤーの位置格納用
    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        //変数などの初期化
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        isPlayerInArea = false;
        isAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.Instance.isGaming)
        {
            //自分を中心に円形のRayを発生
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadius);
            //Playerタグが範囲内にいないか捜索
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Player"))
                {
                    isPlayerInArea = true;
                    break;
                }
            }
            if (!isAttack)
            {
                if (isPlayerInArea)
                {
                    // プレイヤーが範囲内にいる場合、タイマーを加算
                    chargeTimer += Time.deltaTime;
                    // 指定したチャージ時間に達したらエフェクトを生成
                    if (chargeTimer >= attackCharge)
                    {
                        ExecuteAttack();
                    }
                }
            }
            else
            {
                // プレイヤーが範囲外に出たらタイマーをリセット
                chargeTimer = 0f;
            }
            if (!isAttack)
            {
                Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
                transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            }

        }
        }

    //爆破オブジェクトに触れると消える
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
    }

    //AttackEffectを出す処理
    private void ExecuteAttack()
    {
        isAttack = true;
        Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY, 0);
        // 自分+オフセットの位置にAttackEffectを生成
        if (attackEffect != null)
        {
            Instantiate(attackEffect, spawnPosition, Quaternion.identity);
        }

        Invoke(nameof(ResetAttack), 2.0f);
    }

    //攻撃タイマーとフラグリセット
    private void ResetAttack()
    {
        isAttack = false;
        isPlayerInArea = false;
        chargeTimer = 0f;
    }

    //デバッグ用（Rayの表示）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
