using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enegry : MonoBehaviour
{
    #region variables
    int _maxEnergy = 5;
    int _standardEnemy;
    int _heavyEnemy;
    int _ranged;
    public int currentEnergy;


    #endregion

    private void Awake ( )
    {
        //Gets all Values needed for the start of the game
        _standardEnemy = 1;
        _heavyEnemy = 3;
        _ranged = 4;
        currentEnergy = _maxEnergy;
    }

    private void Update ( )
    {
        //Makes sure Energy Level does not go over the Max allowed Level
        currentEnergy = Mathf.Clamp ( currentEnergy , 0 , _maxEnergy );
    }

    public void LoseEnergy (int amount )
    {
        currentEnergy = -amount;
    }



    //adds energy after every enemy killed
    private void OnCollisionExit ( Collision enemy )
    {

        if ( enemy.collider.name == "StandardEnemy" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _standardEnemy;
            }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="HeavyEnemy" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _heavyEnemy;
            }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="RangedEnemy" )
        {
            if ( currentEnergy < 5 )
            {
                currentEnergy += _ranged;
            }
            Destroy ( enemy.gameObject );
        }
    }

}
