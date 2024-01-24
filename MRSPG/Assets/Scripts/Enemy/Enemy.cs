using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySetting _enemy;
    public float speed;
    private GameObject _player;
    private Vector3 _target;

    public void Start()
    {
        _player = GameObject.Find("Player/PlayerObj");
        SetTag(_enemy.type);
    }
    public void Update()
    {

        _target = _player.transform.position;
        float _distFromPlr = Vector3.Distance(transform.position, _target);
        float enemy_speed = speed * Time.deltaTime;

        if(CheckForPlayer(transform.position, 5, _player.GetComponent<Collider>()) && !CheckForPlayer(transform.position, 5, _player.GetComponent<Collider>()))
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, enemy_speed);
            transform.LookAt(_target);
        }
        if(CheckForPlayer(transform.position, 2, _player.GetComponent<Collider>()))
        {
            transform.LookAt(_target);
        }


        /*if (CheckForEnemies(transform.position, 10))
        {
            Debug.Log("There are enemies within the big radius.");
        }*/
    }

    #region Define Enemy
    private string SetTag(EnemyType _enemytype)
    {
        switch(_enemytype)
        {
            case EnemyType.Standard:
                Debug.Log("Standard");
                gameObject.name = "StandardEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Heavy:
                Debug.Log("Heavy");
                gameObject.name = "HeavyEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Ranged:
                Debug.Log("Ranged");
                gameObject.name = "RangedEnemy";
                gameObject.tag = "Enemy";
                break;

        }
        return null;
    }

    #endregion

    #region Enemy Radius
    private bool CheckForPlayer(Vector3 center, float radius, Collider plr)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if(obj == plr)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForEnemies(Vector3 center, float radius)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach(var obj in collider)
        {
            if(obj.tag == "Enemy")
            {
                obj.GetComponent<Enemy>().speed = 2;
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2);
        Gizmos.DrawWireSphere(transform.position, 5);
    }
    #endregion
}
