using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private GameDirector gameDirector;
    [SerializeField] private GameObject ExplosionEffect;

    [Header("変数")]
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxAirDodge = 1;

    [Header("接地判定")]
    [SerializeField] private float groundRayDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    Vector2 move;
    private bool isGround;
    private int currentryAirDodge;
    private float defaultGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentryAirDodge = 0;
        defaultGravity = rb.gravityScale;

        gameDirector = FindFirstObjectByType<GameDirector>();
    }

    void Update()
    {
        if (GameDirector.Instance.isGaming)
        {
            // 接地判定
            CheckGround();

            move = moveAction.action.ReadValue<Vector2>();
            transform.position += Vector3.right * move.x * moveSpeed * Time.deltaTime;

            if (isGround && jumpAction.action.WasPressedThisFrame())
            {
                Debug.Log("ジャンプ");
                Jump();
            }
            else if (!isGround && jumpAction.action.WasPressedThisFrame() &&
                     (currentryAirDodge < maxAirDodge))
            {
                Debug.Log("空中回避");
                StartCoroutine(AirDodge());
                currentryAirDodge++;
            }

            if (!GameDirector.Instance.isLiving)
            {
                Instantiate(ExplosionEffect);
            }
        }
    }

    // Rayによる接地判定
    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundRayDistance,
            groundLayer
        );

        if (hit.collider != null)
        {
            if (!isGround)
            {
                // 空中から地面に着いた瞬間
                currentryAirDodge = 0;
            }

            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    // ゲームオーバー・爆破・クリア判定用
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ゲームオーバー用
        if (collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Attack"))
        {
            gameDirector.Restart();
            Instantiate(ExplosionEffect);
        }

        // 爆破オブジェクトに触れると消える
        if (collision.gameObject.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }

        // クリア判定のためにボスは別扱い
        if (collision.gameObject.CompareTag("Boss"))
        {
            Instantiate(ExplosionEffect);
            GameDirector.Instance.isGaming = false;
            gameDirector.Clear();
        }
    }

    // ジャンプ用
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // 空中回避用
    IEnumerator AirDodge()
    {
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(move.x * moveSpeed * 2f, 0f);

        yield return new WaitForSeconds(0.1f);

        rb.gravityScale = defaultGravity;
    }

    // RayをSceneビューで確認する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundRayDistance
        );
    }
}