using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("ê›íË")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackCharge = 2f;

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
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }
}
