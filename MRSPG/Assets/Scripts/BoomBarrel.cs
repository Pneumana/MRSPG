using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoomBarrel : MonoBehaviour
{
    #region Variables

    public int _boomRadius = 3;
    int _maxHealth = 1;
    public int currentHealth;

    [SerializeField]
    Health _playerHealth;

    [SerializeField]
    BoomBarrel _boomAgain;

    [SerializeField]
    GameObject _explosion;

    #endregion

    private void Awake ( )
    {
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

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
            TryExplode( );
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "MeleeHitbox")
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
        Collider [ ] colliders = Physics.OverlapSphere ( transform.position , _boomRadius, LayerMask.GetMask("Enemy", "Player", "Ground") );
        foreach( Collider collider in colliders )
        {
            Debug.Log(collider.gameObject.name + " was hit by explosion from " + gameObject.name, collider.gameObject);
            var enemyBody = collider.GetComponent<EnemyBody>();
            var barrel = collider.GetComponent<BoomBarrel>();
            if (enemyBody != null)
            {
                enemyBody.ModifyHealth(5, EnemyBody.DamageTypes.Explosive);
            }
            if (barrel != null )
            {
                if(barrel.gameObject != gameObject)
                {
                barrel.TryExplode();
                }
            }
        }
    }
    
    void DealDamage ( )
    {
        _playerHealth.LoseHealth ( 5 );

        Debug.Log ( "Enemy Took 5 Damage" );
    }

    IEnumerator Explode ( )
    {

        yield return new WaitForSeconds ( 0.5f );
        _explosion = Instantiate ( _explosion , transform.position , Quaternion.identity );
        DetectionAfterExplosion();

        yield return new WaitForSeconds ( 0.6f );
        Destroy ( this.gameObject );
    }


}
