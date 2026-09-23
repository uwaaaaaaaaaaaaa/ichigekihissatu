using UnityEngine;
using System.Collections;

public class AttackEffectSlash : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(InstantfewSeconds());
    }


    IEnumerator InstantfewSeconds()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        yield break;
    }
}
