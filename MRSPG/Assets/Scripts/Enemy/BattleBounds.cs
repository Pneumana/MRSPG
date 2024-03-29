using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBounds : MonoBehaviour
{
    public Transform player;
    [Tooltip("The yellow circle is the size of the boundary")]
    public float boundSize;
    [Tooltip("Add the enemies within the boundary")]
    public List<EnemyBody> enemies = new List<EnemyBody>();

    float distance;
    bool running;
    bool inBattle;

    private void Awake()
    {
        foreach(EnemyBody enemy in enemies)
        {
            enemy.bounds = this;
        }
    }

    private void Update()
    {
        if(enemies.Count > 0)
        {
            distance = Vector3.Distance(transform.position, player.position);
            if(distance < boundSize)
            {
                inBattle = true;
            }
            if(inBattle && !running && distance > boundSize)
            {
                Debug.Log("The player is leaving the battle, " + distance);
                StartCoroutine(DrainHP());
            }
        }
    }

    IEnumerator DrainHP()
    {
        running = true;
        yield return new WaitForSeconds(2f);
        if(distance > boundSize)
        {
            player.parent.GetComponent<Health>().LoseHealth(1);
        }
        running = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundSize);
    }
}
