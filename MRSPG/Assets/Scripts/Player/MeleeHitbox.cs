using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
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
        if (!collision.gameObject.TryGetComponent<EnemyBody>(out var enemyBody)) { return; }
        if (!collision.gameObject.TryGetComponent<Enemy>(out var enemy)) { return; }
        enemyBody.ModifyHealth(-1);
        if (true /*timed on beat*/) { StopCoroutine(enemy.StartAttack(enemy._enemy.pattern)); }

    }
}
