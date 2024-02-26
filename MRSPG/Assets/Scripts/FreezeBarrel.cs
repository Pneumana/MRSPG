using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBarrel : MonoBehaviour
{
    #region Variables

    int _freezeRadius = 3;
    int _maxHealth = 1;
    public int currentHealth;


    [SerializeField]
    FreezeBarrel freezeReset;

    #endregion

    private void Awake ( )
    {
        freezeReset=GameObject.Find("Freeze Barrel").GetComponent<FreezeBarrel>();

        if ( freezeReset == null )
        {
            Debug.LogError ( "Freeze Barrel is NULL" );
        }

        currentHealth = _maxHealth;
    }


    private void OnCollisionEnter ( Collision detonate )
    {
        if ( detonate.collider.tag == ( "Enemy" ) )
        {
            Detonated ( );
        }
    }

    public void Detonated ( )
    {
        currentHealth -= 1;

        if ( currentHealth == 0 )
        {
            DetecAfterDetonated ( );
            StartCoroutine ( PauseBeforeGone ( ) );
        }
    }

    void DetecAfterDetonated ( )
    {
        Collider [ ] colliders = Physics.OverlapSphere ( transform.position , _freezeRadius );

        foreach(var collider in colliders )
        {
            Freeze ( );
            Debug.Log ( "Everything is Frozen" );
            return;
        }
    }

    void Freeze ( )
    {
        GameObject.Find ( "Player" ).GetComponent<Rigidbody> ( ).constraints = RigidbodyConstraints.FreezeAll;
        GameObject.FindGameObjectWithTag("Enemy").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine ( Frozen ( ) );
    }

    
    IEnumerator Frozen ( )
    {
        yield return new WaitForSeconds ( 3 );
        GameObject.Find ( "Player" ).GetComponent<Rigidbody> ( ).constraints = RigidbodyConstraints.None;
        GameObject.FindGameObjectWithTag ( "Enemy" ).GetComponent<Rigidbody> ( ).constraints = RigidbodyConstraints.None;
        Debug.Log ( "Everything is Unfrozen" );
    }
    

    IEnumerator PauseBeforeGone ( )
    {
        yield return new WaitForSeconds ( .5f );
        Destroy ( this.gameObject );
    }

}
