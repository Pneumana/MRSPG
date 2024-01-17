using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enegry : MonoBehaviour
{
    #region variables
    int _maxEnergy = 5;    
    int _lightEnemy;
    int _heavyEnemy;
    int _boss;
    public int currentEnergy;

    #endregion

    private void Awake ( )
    {
        _lightEnemy = 1;
        _heavyEnemy = 3;
        _boss = 4;
        currentEnergy = _maxEnergy;
    }

    public void LoseEnergy ( int amount )
    {
        currentEnergy = -amount;
    }

    private void OnCollisionExit ( Collision enemy )
    {
        if ( enemy.collider.tag=="LightEnemy" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _lightEnemy;
                Debug.Log ( "1 Point Added" );
                Destroy ( enemy.gameObject );
            }
            
        }
      
        else if ( enemy.collider.tag=="HeavyEnemy" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _heavyEnemy;
                Debug.Log ( "3 Points Added" );
                Destroy ( enemy.gameObject );
            }
           
        }
      
        else if ( enemy.collider.tag=="Boss" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _boss;
                Debug.Log ( "4 Points Added" );
                Destroy ( enemy.gameObject );
            }       
                      
        }
    }

}
