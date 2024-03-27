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
    [SerializeField]
    ParticleSystem ps;

    #endregion

    private void Awake ( )
    {
        ps.Stop();
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

        //Formerly
        //_boomAgain= GameObject.Find("Boom Barrel").GetComponent<BoomBarrel>();
        //not sure if that line is needed but there's a null ref error happening because of "Boom Barrel" not existing
        _boomAgain = GetComponent<BoomBarrel>();

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
        currentHealth = 0;

        if ( currentHealth == 0 )
        {
            ps.Play ( );
            DetectionAfterExplosion ( );
            StartCoroutine ( PauseBeforeGone ( ) );
        }
    }

    void DetectionAfterExplosion ( )
    {
        Collider [ ] colliders = Physics.OverlapSphere ( transform.position , _boomRadius );

        foreach( var collider in colliders )
        {
            var enemyBody = collider.gameObject.GetComponent<EnemyBody>();
            var barrel = collider.gameObject.GetComponent<BoomBarrel>();
            if (enemyBody != null)
            {
                enemyBody.ModifyHealth(5);
            }
            if (barrel != null && barrel != this)
            {
                barrel.Explode();
            }
            //DealDamage ( );
            return;
        }
    }
    //the player would be taking damage every time this went off, so im chaning it to damage the player if they are close enough to be hurt by it.
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
