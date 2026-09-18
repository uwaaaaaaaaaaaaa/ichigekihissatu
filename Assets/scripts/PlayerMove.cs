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
    [SerializeField] private GameDirector GameDirector;
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
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.action.ReadValue<Vector2>();
        transform.position += Vector3.right * move.x * moveSpeed * Time.deltaTime;

        if (isGround && jumpAction.action.WasPressedThisFrame())
        {
            Debug.Log("ジャンプ");
            Jump();
        }
        else if(!isGround && jumpAction.action.WasPressedThisFrame() && (currentryAirDodge < maxAirDodge))
        {
            Debug.Log("空中回避！");
            StartCoroutine(AirDodge());
            currentryAirDodge++;
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
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Attack"))
        {
            
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

    //自爆演出用（ゲームオーバー）
    IEnumerator Explosion()
    {
        Instantiate(ExplosionEffect);
        yield return new WaitForSeconds(1);
        Destroy(ExplosionEffect);
    }
}

/*
 IEnumerator Explosion()
{
    // 爆発を生成
    GameObject explosion = Instantiate(
        ExplosionEffect,
        transform.position,
        Quaternion.identity
    );

    float duration = 1f;
    float timer = 0f;

    Vector3 startScale = explosion.transform.localScale;

    // 1秒かけて小さくする
    while (timer < duration)
    {
        timer += Time.deltaTime;

        float t = timer / duration;

        explosion.transform.localScale =
            Vector3.Lerp(startScale, Vector3.zero, t);

        yield return null;
    }

    Destroy(explosion);
}
 */