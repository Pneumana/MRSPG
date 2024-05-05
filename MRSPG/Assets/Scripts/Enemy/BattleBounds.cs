using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleBounds : MonoBehaviour
{
    public Transform player;
    [Tooltip("The yellow circle is the size of the boundary")]
    public float boundSize;
    [Tooltip("Add the enemies within the boundary")]
    public List<GameObject> enemies = new List<GameObject>();
    [SerializeField] Transform effectiveRange;

    [SerializeField]
    ParticleSystem damageBuildup, damageBurst;

    public float distance;
    public bool running;
    public bool inBattle;
    private TargetManager targetManager;

    public int defeated;

    [Header("Set Main Boundary")]
    public bool PlayerWithinBoundary;

    private void Awake()
    {
        if (player == null)
            player = GameObject.Find("PlayerObj").transform;
        foreach(Enemy enemy in GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            //Debug.Log("compairing dist from " + enemy.gameObject.name + " to " + name + " (" + Vector3.Distance(enemy.gameObject.transform.position, transform.position) + ")");
            if(Vector3.Distance(enemy.gameObject.transform.position, transform.position) <= boundSize)
            {
                //Debug.Log("this is an acceptable distance");
                var body = enemy.gameObject.GetComponent<EnemyBody>();
                if (body!=null)
                {
                    if(body.bounds == null)
                    {
                        body.bounds = this;
                        enemies.Add(body.gameObject);
                    }/*
                    else
                    {
                        Debug.Log( enemy.gameObject.name + " was already claimed by " + body.bounds.name);
                    }*/
                }
            }
            else
            {
                Debug.Log("object is too far away");
            }
        }
        targetManager = GameObject.FindAnyObjectByType<TargetManager>();
        effectiveRange.localScale = new Vector3(boundSize * 2, boundSize * 2, boundSize * 2);
        //SavedEnemies = enemies;
    }

    private void Update()
    {
        effectiveRange.GetComponent<MeshRenderer>().material.SetVector("_playerPosition", player.position);
        if(enemies.Count > defeated)
        {
            distance = Vector3.Distance(transform.position, player.position);
            if(distance < boundSize)
            {
                inBattle = true;
                foreach(GameObject enemy in enemies)
                {
                    if (enemy != null)
                    {
                        if (!enemy.GetComponent<Enemy>().aggro && enemy.activeInHierarchy)
                            enemy.GetComponent<Enemy>().aggro = true;
                    }
                }
                targetManager.battlebounds = this;
                damageBuildup.Stop();
            }
            if(inBattle && distance > boundSize && !PlayerWithinBoundary)
            {
                damageBuildup.gameObject.SetActive(true);
                damageBuildup.transform.position = player.transform.position;
            }else if(inBattle || PlayerWithinBoundary)
            {
                damageBuildup.gameObject.SetActive(false);
                StopCoroutine(DrainHP());
            }
            if(inBattle && !running && distance > boundSize && !PlayerWithinBoundary)
            {
                if(targetManager.battlebounds == this)
                {
                    Debug.Log("The player is leaving the battle, " + distance);
                    StartCoroutine(DrainHP());
                }
            }
        }
        else { gameObject.SetActive(false);  }
       /* foreach(GameObject body in enemies)
        {
            if(body == null)
            {
                enemies.Remove(body);
            }
        }*/
    }

    public IEnumerator DrainHP()
    {
        running = true;
        damageBuildup.Play();
        yield return new WaitForSeconds(1f);
        if(distance > boundSize)
        {
            if (PlayerWithinBoundary)
            {
                yield break;
            }
            else
            {
                damageBurst.transform.position = player.transform.position + Vector3.down;
                damageBurst.Play();
                player.parent.GetComponent<Health>().LoseHealth(1);
            }
            //play hurt particles on player
        }
        yield return new WaitForSeconds(.5f);
        running = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundSize);
    }
}
