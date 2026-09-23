using UnityEngine;

//Lv2：ついてくる敵
public class EnemyMove2: MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float moveSpeed = 3f;

    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) { return; }
        if (GameDirector.Instance.isGaming)
        {
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
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
