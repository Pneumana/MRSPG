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
    BoomBarrel _boomAgain;

    #endregion

    private void Awake ( )
    {
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

        _boomAgain= GameObject.Find("Boom Barrel").GetComponent<BoomBarrel>();

        if ( _boomAgain == null )
        {
            Debug.LogError ( "Boom Barrel is NULL" );
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

    public void Explode ( )
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
        Collider [ ] colliders = Physics.OverlapSphere ( transform.position , _boomRadius );

        foreach( var collider in colliders )
        {
            DealDamage ( );
            return;
        }
    }

    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );

        Debug.Log ( "Enemy Took 5 Damage" );

        if ( GameObject.Find ( "Boom Barrel" ) )
        {
            _boomAgain.Explode ( );
        }
    }

    IEnumerator PauseBeforeGone ( )
    {
        yield return new WaitForSeconds ( .5f );
        Destroy ( this.gameObject );
    }


}
