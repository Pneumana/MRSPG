using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This script goes on the player controller, and is going to use a raycast to detect the closest enemy.
/// Here this will have a target enemy and have a radius around it to slow down surrounding enemies.
/// </summary>
public class TargetManager : MonoBehaviour
{
    public GameObject closestEnemy;
    public float gizmosize;
    public List<GameObject> enemiesInRange;

    void Update()
    {
        CheckForEnemies(transform.position, gizmosize);
        closestEnemy = GetClosestEnemy(enemiesInRange);
        if (CheckForEnemies(transform.position, gizmosize))
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy != closestEnemy)
                {
                    closestEnemy.GetComponent<NavMeshAgent>().speed = closestEnemy.GetComponent<Enemy>()._enemy.NavMeshSlowedSpeed;
                    Debug.Log("Slowed: " + closestEnemy.GetComponent<NavMeshAgent>().speed);
                }
                else
                {
                    closestEnemy.GetComponent<NavMeshAgent>().speed = closestEnemy.GetComponent<Enemy>()._enemy.NavMeshSpeed;
                    Debug.Log("Regular: " + closestEnemy.GetComponent<NavMeshAgent>().speed);
                }
            }
        }
        if (!CheckForEnemies(transform.position, gizmosize))
        {
            /*foreach (GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<NavMeshAgent>().speed = enemy.GetComponent<EnemySetting>().NavMeshSpeed;
            }*/
            enemiesInRange.Clear();
        }


    }

    GameObject GetClosestEnemy(List<GameObject> enemies)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    public bool CheckForEnemies(Vector3 center, float radius)
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
        Gizmos.DrawWireSphere(transform.position, gizmosize);
    }
}
