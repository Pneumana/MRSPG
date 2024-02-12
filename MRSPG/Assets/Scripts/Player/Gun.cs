using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
public class Gun : MonoBehaviour
{
    #region Variables

    public LockOnSystem targeting;
    public Enegry energy;
    public GameObject bulletPrefab;
    public Controller control;


    #endregion

    private void Start ( )
    {
        //energy = GameObject.Find ( "Player" ).GetComponent<Enegry> ( );

        if ( energy == null )
        {
            Debug.LogError ( "Player is NULL" );
        }
        targeting = GameObject.Find ( "TimeScaler" ).GetComponent<LockOnSystem> ( );

        if ( targeting == null )
        {
            Debug.LogError ( "TimeScaler is NULL" );
        }

        control=GameObject.Find("Controller Detection").GetComponent<Controller> ( );

        if ( control == null )
        {
            Debug.LogError ( "Controller Detection is NULL" );
        }
    }

    private void Update ( )
    {
        if ( control.controls.Gameplay.Fire.WasPressedThisFrame() && targeting.trackedEnemy!=null && energy.currentEnergy == 50 )
        {
            var bullet = Instantiate ( bulletPrefab , transform.position , Quaternion.identity );
            bullet.transform.forward = targeting.trackedEnemy.transform.position - transform.position;
            Debug.DrawLine(bullet.transform.position, bullet.transform.position + bullet.transform.forward, Color.magenta, 10);
            //energy.LoseEnergy ( 0 );
        }
    }
}
