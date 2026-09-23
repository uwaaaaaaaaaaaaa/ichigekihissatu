using UnityEngine;

public class Boss : MonoBehaviour
{
    //とりあえず現段階は爆破エフェクトに当たったら消えるだけ
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
    }
}
