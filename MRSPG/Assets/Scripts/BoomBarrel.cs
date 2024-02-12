using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoomBarrel : MonoBehaviour
{
    #region Variables

    int _boomRadius = 3;
    int _maxHealth = 1;
    public int currentHealth;

    [SerializeField]
    Health _playerHealth;
    [SerializeField]
    EnemySetting _enemyHealth;


    #endregion

    private void Awake ( )
    {
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

        _enemyHealth = GameObject.FindWithTag ( "Enemy" ).GetComponent<EnemySetting> ( );

        if (_enemyHealth == null )
        {
            Debug.LogError ( "EnemySetting is NULL" );
        }

        currentHealth = _maxHealth;
    }

    private void OnCollisionEnter ( Collision boom )
    {
        if ( boom.collider.tag == ( "Enemy" ) )
        {
            Explode ( );
        }
    }

    void Explode ( )
    {
        if ( currentHealth == 0 )
        {
            DetectionAfterExplosion ( );
            Destroy ( this.gameObject );
        }
    }

    void DetectionAfterExplosion ( )
    {
        var colliders = Physics.OverlapSphere ( transform.position , _boomRadius );

        foreach(var collider in colliders )
        {
            DealDamage ( );
        }
    }

    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );
        _enemyHealth.EnemyHealth -= 5;
    }


}
