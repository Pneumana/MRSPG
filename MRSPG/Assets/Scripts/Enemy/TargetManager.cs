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
    [HideInInspector] public BattleBounds battlebounds;

    void Update()
    {
        if (CheckForEnemies(transform.position, gizmosize))
        {
            closestEnemy = GetClosestEnemy(enemiesInRange);
            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy != closestEnemy)
                {
                    try
                    {
                        enemy.GetComponent<NavMeshAgent>().speed = enemy.GetComponent<Enemy>()._enemy.NavMeshSlowedSpeed;
                        //Debug.Log("Slowed: " + closestEnemy.GetComponent<NavMeshAgent>().speed);
                    }
                    catch { /*Debug.Log(closestEnemy.name + " doesn't have a NavMeshAgent component");*/ }
                    
                }
                else
                {
                    try
                    {
                        enemy.GetComponent<NavMeshAgent>().speed = enemy.GetComponent<Enemy>()._enemy.NavMeshSpeed;
                        //Debug.Log("Regular: " + closestEnemy.GetComponent<NavMeshAgent>().speed);
                    }
                    catch { /*Debug.Log(closestEnemy.name + " doesn't have a NavMeshAgent component");*/ }
                   
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
        GameObject Closest = null;
        float closestDist = Mathf.Infinity;
        if (enemies.Count == 0) { return null; }
        foreach (GameObject currentTest in enemies)
        {
            float distToTarget = Vector3.Distance(currentTest.transform.position, transform.position);
            if (distToTarget < closestDist)
            {
                closestDist = distToTarget;
                Closest = currentTest;
            }
        }
        return Closest;
    }
    public bool CheckForEnemies(Vector3 center, float radius)
    {
        enemiesInRange.Clear();
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if (obj.CompareTag("Enemy"))
            {
                enemiesInRange.Add(obj.gameObject);
            }
        }
        if (enemiesInRange.Count > 0) { return true; }
        else { return false; }
    }
}
