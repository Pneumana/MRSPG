using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region  Variables

    float _speed = 5;
    
    public LockOnSystem targeting;
    public Enegry energy;
    public Temp_Boss_health tbh;
    public Transform target;


    #endregion

    private void Awake ( )
    {
        energy = GameObject.Find ( "Player" ).GetComponent<Enegry> ( );

        if ( energy == null )
        {
            Debug.LogError ( "Player is NULL" );
        }

        targeting = GameObject.Find ( "TimeScaler" ).GetComponent<LockOnSystem> ( );

        if ( targeting == null )
        {
            Debug.LogError ( "TimeScaler is NULL" );
        }

        tbh=GameObject.Find("Boss").GetComponent<Temp_Boss_health> ( );

        if ( tbh == null )
        {
            Debug.LogError ( "Boss is NULL" );
        }

        target = targeting.closestTarget.transform;
    }

    private void FixedUpdate ( )
    {
        transform.position = Vector3.MoveTowards ( transform.position , target.transform.position , _speed * Time.fixedDeltaTime );
    }

    private void OnCollisionEnter ( Collision shot )
    {
        if ( shot.collider.name == "Boss" )
        {
            tbh.LoseHealth ( 5 );
        }
        else
        {
            Destroy ( shot.gameObject );
        }
    }
}
