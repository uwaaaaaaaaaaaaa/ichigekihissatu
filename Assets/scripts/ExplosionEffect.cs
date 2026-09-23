using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour
{
    //プレイヤーの位置を特定するための
    private GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        StartCoroutine(Explosion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Explosion()
    {
        transform.position = Player.transform.position;
        //爆風を表示
        float duration = 1f;
        float timer = 0f;

        Vector3 startScale = gameObject.transform.localScale;

        // 1秒かけて小さくする
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            gameObject.transform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }
        Destroy(gameObject);
    }
}
