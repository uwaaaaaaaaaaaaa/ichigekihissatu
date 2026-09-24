using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputActionReference moveAction;       //インプットシステムのアタッチ
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private GameDirector gameDirector;
    [SerializeField] private GameObject ExplosionEffect;

    [Header("変数")]
    [SerializeField] private float jumpForce = 3f;      //ジャンプの強さ
    [SerializeField] private float moveSpeed = 2f;      //移動速度
    [SerializeField] private int maxAirDodge = 1;       //空中回避の上限回数

    private Rigidbody2D rb;
    Vector2 move;                   //移動する方向を決める変数
    private bool isGround;          //接地判定
    private int currentryAirDodge;  //空中回避の回数判定
    private float defaultGravity;   //自身の重力を格納しておく

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentryAirDodge = 0;
        defaultGravity = rb.gravityScale;
        //生成されたときにGameDirectorを認識する用
        gameDirector = FindFirstObjectByType<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.Instance.isGaming)
        {
            move = moveAction.action.ReadValue<Vector2>();
            transform.position += Vector3.right * move.x * moveSpeed * Time.deltaTime;

            if (isGround && jumpAction.action.WasPressedThisFrame())
            {
                Debug.Log("ジャンプ");
                Jump();
            }
            else if (!isGround && jumpAction.action.WasPressedThisFrame() && (currentryAirDodge < maxAirDodge))
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

    //接地判定用
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            currentryAirDodge = 0;
        }
        //ゲームオーバー用
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Attack"))
        {
            gameDirector.Restart();
            Instantiate(ExplosionEffect);
        }
        //爆破オブジェクトに触れると消える
        if (collision.gameObject.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
        //クリア判定のためにボスは別扱い
        if (collision.gameObject.CompareTag("Boss"))
        {
            Instantiate(ExplosionEffect);
            gameDirector.Clear();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
    }


    //ジャンプ用
    void Jump() {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    //空中回避用
    IEnumerator AirDodge()
    {
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(move.x * moveSpeed * 2f, 0f);
        yield return new WaitForSeconds(0.1f);
        rb.gravityScale = defaultGravity;
    }

    


}
