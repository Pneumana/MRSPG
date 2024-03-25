using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.XR;

public class Gun : MonoBehaviour
{
    #region Variables

    public LockOnSystem targeting;
    public Energy energy;
    public GameObject bulletPrefab;
    public Controller control;


    #endregion

    [SerializeField] Animator animator;

    private void Start ( )
    {
        energy = GameObject.Find ( "Player" ).GetComponent<Energy> ( );

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

    private void Update()
    {
        ShootGun();
    }

    void ShootGun()
    {
        if (!control.controls.Gameplay.Fire.WasReleasedThisFrame())
            return;

        animator.SetTrigger("Shooting");
        //NEED THIS LINE!
        //forces the animator to be on frame 0 of the shooting anim otherwise the sword's tip will shoot the shot just anywhere.
        animator.Update(Time.deltaTime);



        //delay by
        GameObject bullet = null;
        targeting.UpdateTargetUI();
        Debug.Log(targeting.trackedEnemy);
        var lookDir = targeting.trackedEnemy.transform.position - transform.position;

        if (targeting.trackedEnemy != null && energy.currentEnergy == 50)
        {
            
            
    Debug.Log("shooting at " + targeting.trackedEnemy.name, targeting.trackedEnemy);
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.forward = targeting.trackedEnemy.transform.position - transform.position;
            Debug.DrawLine(bullet.transform.position, bullet.transform.position + bullet.transform.forward, Color.magenta, 10);
            energy.LoseEnergy(50);
        }
        //Invoke("RestOfShot", 0);
        GameObject.Find("Player").transform.forward = new Vector3(lookDir.x, 0, lookDir.z);


        /*if(targeting.trackedEnemy != null)
            if(bullet.transform.forward != targeting.trackedEnemy.transform.position - transform.position)
                bullet.transform.forward = targeting.trackedEnemy.transform.position - transform.position;*/
    }
    public void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log("The gun was shot");
        //ShootGun();
    }
}

