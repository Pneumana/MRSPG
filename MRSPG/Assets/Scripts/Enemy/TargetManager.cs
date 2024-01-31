using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script goes on the player controller, and is going to use a raycast to detect the closest enemy.
/// Here this will have a target enemy and have a radius around it to slow down surrounding enemies.
/// </summary>
public class TargetManager : MonoBehaviour
{
    public static Transform player;
    private Vector3 distance;
    [SerializeField] private List<Enemy> enemiesInRange = new List<Enemy>();

    // Update is called once per frame
    /*void Update()
    {
        if (CheckForEnemies(transform.position, 5))
        {
            enemiesInRange.Remove(gameObject);
            Debug.Log("There are enemies within the big radius.");
            foreach (GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<Enemy>().speed = 1f;
            }
        }
    }
    private bool CheckForEnemies(Vector3 center, float radius)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if (obj.tag == "Enemy")
            {
                if (!enemiesInRange.Contains(obj.gameObject))
                {
                    enemiesInRange.Add(obj.gameObject);
                }
                return true;
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2);
        Gizmos.DrawWireSphere(transform.position, 5);
    }*/
}
