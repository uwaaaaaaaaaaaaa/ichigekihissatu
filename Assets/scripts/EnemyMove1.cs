using System.Runtime.CompilerServices;
using UnityEngine;

//Lv1：左に動くだけの敵
public class EnemyMove1 : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float moveSpeed = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.Instance.isGaming) { 
        //一定の速度で左に動く
        transform.position += transform.right * -moveSpeed * Time.deltaTime;
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
}
