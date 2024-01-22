using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySetting EnemySetting;
    public float speed;
    private GameObject _player;

    public void Awake()
    {
        _player = GameObject.Find("Player/PlayerObj");
    }
    public void Update()
    {
        Vector3 _target = _player.transform.position;
        float _distFromPlr = Vector3.Distance(transform.position, _target);
        float enemy_speed = speed * Time.deltaTime;

        if(CheckForPlayer(transform.position, 10, _player.GetComponent<Collider>()) && !CheckForPlayer(transform.position, 5, _player.GetComponent<Collider>()))
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, enemy_speed);
        }
        if(CheckForPlayer(transform.position, 5, _player.GetComponent<Collider>()))
        {
            Debug.Log("YES!");
        }
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5);
        Gizmos.DrawWireSphere(transform.position, 10);
    }
    #endregion
}
