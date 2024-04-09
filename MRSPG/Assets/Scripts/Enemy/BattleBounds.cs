using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBounds : MonoBehaviour
{
    public Transform player;
    [Tooltip("The yellow circle is the size of the boundary")]
    public float boundSize;
    [Tooltip("Add the enemies within the boundary")]
    public List<EnemyBody> SavedEnemies;
    public List<EnemyBody> enemies = new List<EnemyBody>();
    [SerializeField]
    Transform effectiveRange;

    [SerializeField]
    ParticleSystem damageBuildup, damageBurst;

    public float distance;
    public bool running;
    public static bool inBattle;
    private TargetManager targetManager;

    private void Awake()
    {
        if (player == null)
            player = GameObject.Find("PlayerObj").transform;
        foreach(EnemyBody enemy in enemies)
        {
            enemy.bounds = this;
        }
        targetManager = GameObject.FindAnyObjectByType<TargetManager>();
        effectiveRange.localScale = new Vector3(boundSize * 2, boundSize * 2, boundSize * 2);
        SavedEnemies = enemies;
    }

    private void Update()
    {
        if(enemies.Count > 0)
        {
            distance = Vector3.Distance(transform.position, player.position);
            if(distance < boundSize)
            {
                inBattle = true;
                foreach(EnemyBody enemy in enemies)
                {
                    if(!enemy.gameObject.GetComponent<Enemy>().aggro)
                            enemy.gameObject.GetComponent<Enemy>().aggro = true;
                }
                targetManager.battlebounds = this;
                damageBuildup.Stop();
            }
            if(inBattle && distance > boundSize)
            {
                damageBuildup.transform.position = player.transform.position;
            }
            if(inBattle && !running && distance > boundSize)
            {
                if(targetManager.battlebounds == this)
                {
                    Debug.Log("The player is leaving the battle, " + distance);
                    StartCoroutine(DrainHP());
                }
            }
        }
        else { Destroy(this.gameObject);  }
        foreach(EnemyBody body in enemies)
        {
            if(body == null)
            {
                enemies.Remove(body);
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
