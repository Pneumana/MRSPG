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

    #endregion

    private void Awake ( )
    {
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
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
        currentHealth -= 1;

        if ( currentHealth == 0 )
        {
            DetectionAfterExplosion ( );
            StartCoroutine ( PauseBeforeGone ( ) );
        }
    }

    void DetectionAfterExplosion ( )
    {
        var colliders = Physics.OverlapSphere ( transform.position , _boomRadius );

        foreach ( var collider in colliders )
        {
            DealDamage ( );
        }
    }

    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );
        Debug.Log ( "Enemy Took 5 Damage" );
    }

    IEnumerator PauseBeforeGone ( )
    {
        yield return new WaitForSeconds ( .5f );
        Destroy ( this.gameObject );
    }

    private void OnDrawGizmos ( )
    {
        Gizmos.color=Color.red;
        Gizmos.DrawSphere(transform.position, _boomRadius );        
    }


}
