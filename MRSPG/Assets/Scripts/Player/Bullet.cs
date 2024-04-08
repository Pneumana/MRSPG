using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region  Variables

    float _speed = 5;

    [SerializeField]      
    LockOnSystem _targeting;
    [SerializeField]
    Energy _energy;
    [SerializeField]
    Temp_Boss_health _tbh;
    [SerializeField]
    BoomBarrel _boom;
    [SerializeField]
    EnemyBody _enemy;
    [SerializeField]
    Transform _target;


    #endregion

    private void Awake ( )
    {
        _energy = GameObject.Find ( "PlayerObj" ).GetComponent<Energy> ( );

        if ( _energy == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

        _targeting = GameObject.Find ( "TimeScaler" ).GetComponent<LockOnSystem> ( );

        if ( _targeting == null )
        {
            Debug.LogError ( "TimeScaler is NULL" );
        }

        _target = _targeting.trackedEnemy.transform;
    }

    private void FixedUpdate ( )
    {
       transform.position = Vector3.MoveTowards ( transform.position , _target.transform.position , _speed * Time.fixedDeltaTime );
    }

    private void OnCollisionEnter ( Collision shot )
    {
        if ( shot.collider.name == "Boss" )
        {
            _tbh.LoseHealth ( 5 );
            Debug.Log ( "Boss took 5 damage" );
            Destroy ( this.gameObject );
        }
        else if (shot.collider.tag=="Enemy")
        {
            shot.gameObject.TryGetComponent<EnemyBody> ( out EnemyBody enemy );
            if ( enemy == null )
            {
                Debug.LogError ( "EnemyBody Code is NULL" );
            }

            shot.gameObject.TryGetComponent<BoomBarrel> ( out BoomBarrel bang );
            if ( bang == null )
            {
                Debug.LogError ( "BoomBarrel Code is NULL" );
            }

            if(enemy!=null)
            {
                _enemy.ModifyHealth ( 20 );
                Destroy ( this.gameObject );
            }
            else if ( bang != null )
            {
                _boom.TryExplode ( );
                Destroy ( this.gameObject );
            }
        }
    }
}
