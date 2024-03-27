using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [SerializeField]
    Material material;

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

    /*    private void OnCollisionEnter ( Collision boom )
        {
            if ( boom.collider.tag == ( "Enemy" ) )
            {
                TryExplode( );
            }
            if (boom.gameObject.name == "MeleeHitbox")
            {
                TryExplode();
            }
        }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Enemy"))
        {
            TryExplode();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "MeleeHitbox")
        {
            TryExplode();
        }
    }

    public void TryExplode ( )
    {
        currentHealth = 0;

        if ( currentHealth == 0 )
        {
            StartCoroutine(Explode());
        }
    }

    void DetectionAfterExplosion ( )
    {
        var colliders = Physics.OverlapSphere ( transform.position , _boomRadius , LayerMask.GetMask("Enemy", "EnemyProjectiles", "Player"));
        Debug.Log(colliders.Length + " objects were hit by explosion");
        foreach( Collider collider in colliders )
        {
            Debug.Log(collider.gameObject.name + " was hit by explosion from " + gameObject.name, collider.gameObject);
            var enemyBody = collider.GetComponent<EnemyBody>();
            var barrel = collider.GetComponent<BoomBarrel>();
            if (enemyBody != null)
            {
                enemyBody.ModifyHealth(5);
            }
            if (barrel != null )
            {
                if(barrel.gameObject != gameObject)
                {

                    Debug.Log(gameObject.name + "'s explosion triggered " + barrel.gameObject.name);
                    barrel.TryExplode();
                }
            }
        }
    }
    //the player would be taking damage every time this went off, so im chaning it to damage the player if they are close enough to be hurt by it.
    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );

        Debug.Log ( "Enemy Took 5 Damage" );

/*        if ( GameObject.Find ( "Boom Barrel" ) )
        {
            _boomAgain.Explode ( );
        }*/
    }

    IEnumerator Explode ( )
    {
        Debug.Log(gameObject.name + " is starting explosion coroutine");
        ps.Stop();
        ps.Clear();
        yield return new WaitForSeconds ( 0.5f );

        ps.Play();
        DetectionAfterExplosion();



        yield return new WaitForSeconds(1f);
        Destroy ( this.gameObject );
    }


}
