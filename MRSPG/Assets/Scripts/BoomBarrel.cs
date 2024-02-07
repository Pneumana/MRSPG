using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBarrel : MonoBehaviour
{
    #region Variables

    int _boomRadius = 3;

    [SerializeField]
    Health _playerHealth;
    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject [ ] _enemies;
    [SerializeField]
    EnemySetting _enemyHealth;


    #endregion

    private void Awake ( )
    {
        _player = GameObject.Find ( "Player" );
        _playerHealth=GameObject.Find("Player").GetComponent<Health>();

        if ( _player || _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }
    }

    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );
        _enemyHealth.EnemyHealth -= 5;
        if(transform.name=="Boom Barrel" )
        {
            
        }
    }

    void DetectionForExplosion ( )
    {
        var colliders = Physics.OverlapSphere ( transform.position , _boomRadius );

        foreach(var collider in colliders )
        {
            DealDamage ( );
        }
    }



}
