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
    [SerializeField]
    Transform effectiveRange;

    [SerializeField]
    ParticleSystem damageBuildup, damageBurst;

    public float distance;
    public bool running;
    public bool inBattle;

    private void Awake()
    {
        if (player == null)
            player = GameObject.Find("PlayerObj").transform;
        foreach(EnemyBody enemy in enemies)
        {
            enemy.bounds = this;
        }

        effectiveRange.localScale = new Vector3(boundSize * 2, boundSize * 2, boundSize * 2);

    }

    private void Update()
    {
        if(enemies.Count > 0)
        {
            distance = Vector3.Distance(transform.position, player.position);
            if(distance < boundSize)
            {
                inBattle = true;
                damageBuildup.Stop();
            }
            if(inBattle && distance > boundSize)
            {
                damageBuildup.transform.position = player.transform.position;
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
        damageBuildup.Play();
        yield return new WaitForSeconds(2f);
        if(distance > boundSize)
        {
            damageBurst.transform.position = player.transform.position + Vector3.down;
            damageBurst.Play();
            player.parent.GetComponent<Health>().LoseHealth(1);
            //play hurt particles on player
        }
        running = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundSize);
    }
}
