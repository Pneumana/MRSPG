using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region Variables

    public LockOnSystem targeting;
    public Enegry energy;
    public GameObject bulletPrefab;


    #endregion

    private void Start ( )
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
    }

    private void Update ( )
    {

        if ( Input.GetKeyDown ( KeyCode.LeftControl ) && targeting.closestTarget != null && energy.currentEnergy == 50 )
        {
            Instantiate ( bulletPrefab , transform , false );
            energy.LoseEnergy ( 50 );
        }
    }
}
