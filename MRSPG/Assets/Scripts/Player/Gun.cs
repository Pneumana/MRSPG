using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
public class Gun : MonoBehaviour
{
    #region Variables

    public LockOnSystem targeting;
    public Energy energy;
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

    public IEnumerator ShootGun()
    {
        GameObject bullet = null;
        if (targeting.trackedEnemy != null && energy.currentEnergy == 50)
        {
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Debug.DrawLine(bullet.transform.position, bullet.transform.position + bullet.transform.forward, Color.magenta, 10);
            energy.LoseEnergy(50);
        }
        while(bullet.transform.forward != targeting.trackedEnemy.transform.position - transform.position)
        {
            bullet.transform.forward = targeting.trackedEnemy.transform.position - transform.position;
            yield return null;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log("The gun was shot");
        StartCoroutine(ShootGun());
    }
}

