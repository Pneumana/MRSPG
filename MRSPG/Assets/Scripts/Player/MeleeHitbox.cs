using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    //public EnemyBody enemyBody;
    void OnEnable()
    {
        StartCoroutine(EnableTime(0.4f));
    }

    IEnumerator EnableTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.GetComponent<EnemyBody>() == null) { return; }
        collision.gameObject.GetComponent<EnemyBody>().ModifyHealth(-1);
        Debug.Log("EnemyHit");
    }
}
