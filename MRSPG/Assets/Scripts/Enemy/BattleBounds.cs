using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleBounds : MonoBehaviour
{
    public Transform player;
    public bool sphere;
    public bool box;
    [Header("Boundary Size")]
    public float boundSize;
    public float boxOffset;
    public Vector3 boxSize;
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

    [HideInInspector]public bool PlayerWithinBoundary;


    private void Awake()
    {
        if(sphere == false && box == false)
        {
            sphere = true;
        }
        if (player == null)
            player = GameObject.Find("PlayerObj").transform;
        enemies.Clear();
        foreach (Enemy enemy in GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if(sphere)
            {
                if(Vector3.Distance(enemy.gameObject.transform.position, transform.position) <= boundSize)
                {
                    var body = enemy.gameObject.GetComponent<EnemyBody>();
                    if (body!=null)
                    {
                        if(body.bounds == null)
                        {
                            body.bounds = this;
                            enemies.Add(body.gameObject);
                        }
                    }
                }
                effectiveRange.localScale = new Vector3(boundSize * 2, boundSize * 2, boundSize * 2);
            }
            else if(box)
            {
                if (Vector3.Distance(enemy.gameObject.transform.position, transform.position) <= boxSize.x && Vector3.Distance(enemy.gameObject.transform.position, transform.position) <= boxSize.z)
                {
                    var body = enemy.gameObject.GetComponent<EnemyBody>();
                    if (body != null)
                    {
                        if (body.bounds == null)
                        {
                            body.bounds = this;
                            enemies.Add(body.gameObject);
                        }
                    }
                }
                effectiveRange.localScale = new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2);
            }
        }
        targetManager = GameObject.FindAnyObjectByType<TargetManager>();
    }

    public void RespawnAll()
    {
        if(defeated != enemies.Count)
        {
            StopAllCoroutines();
            inBattle = false;
            defeated = 0;
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(true);
                enemy.GetComponent<EnemyBody>().Respawn();

            }
        }
        else
        {
            Debug.Log(gameObject.name + " was defeated, it cannot respawn its enemies");
            //bounds was defeated
        }
    }

    private void Update()
    {
        effectiveRange.GetComponent<MeshRenderer>().material.SetVector("_playerPosition", player.position);
        if(enemies.Count > defeated)
        {
            if (sphere)
            {
                distance = Vector3.Distance(transform.position, player.position);
                if (distance < boundSize)
                {
                    inBattle = true;
                    foreach (GameObject enemy in enemies)
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
                if (inBattle && distance > boundSize && !PlayerWithinBoundary)
                {
                    damageBuildup.gameObject.SetActive(true);
                    damageBuildup.transform.position = player.transform.position;
                }
                else if (inBattle || PlayerWithinBoundary)
                {
                    damageBuildup.gameObject.SetActive(false);
                    StopCoroutine(DrainHP());
                }
                if (inBattle && !running && distance > boundSize && !PlayerWithinBoundary)
                {
                    if (targetManager.battlebounds == this)
                    {
                        Debug.Log("The player is leaving the battle, " + distance);
                        StartCoroutine(DrainHP());
                    }
                }
            }else if(box)
            {
                distance = Vector3.Distance(transform.position, player.position);
                if (distance < boxSize.x && distance < boxSize.z)
                {
                    inBattle = true;
                    foreach (GameObject enemy in enemies)
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
                if (inBattle && distance < boxSize.x && distance < boxSize.z && !PlayerWithinBoundary)
                {
                    damageBuildup.gameObject.SetActive(true);
                    damageBuildup.transform.position = player.transform.position;
                }
                else if (inBattle || PlayerWithinBoundary)
                {
                    damageBuildup.gameObject.SetActive(false);
                    StopCoroutine(DrainHP());
                }
                if (inBattle && !running && distance < boxSize.x && distance < boxSize.z && !PlayerWithinBoundary)
                {
                    if (targetManager.battlebounds == this)
                    {
                        Debug.Log("The player is leaving the battle, " + distance);
                        StartCoroutine(DrainHP());
                    }
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

        Gizmos.color = Color.red;
        Vector3[] vertices = GetRotatedCubeVertices(transform.position + transform.forward * boxOffset, transform.rotation, boxSize);

        for (int i = 0; i < 4; i++)
        {
            int nextIndex = (i + 1) % 4;
            Gizmos.DrawLine(vertices[i], vertices[nextIndex]);
            Gizmos.DrawLine(vertices[i + 4], vertices[nextIndex + 4]);
            Gizmos.DrawLine(vertices[i], vertices[i + 4]);
        }
    }
    private Vector3[] GetRotatedCubeVertices(Vector3 position, Quaternion rotation, Vector3 size)
    {
        Vector3[] vertices = new Vector3[8];

        // Calculate vertices relative to the center of the cube
        Vector3 halfSize = size * 0.5f;

        vertices[0] = rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[1] = rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[2] = rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z) + position;
        vertices[3] = rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z) + position;

        vertices[4] = rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[5] = rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[6] = rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z) + position;
        vertices[7] = rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z) + position;

        return vertices;
    }
}
