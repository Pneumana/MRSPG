using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enegry : MonoBehaviour
{
    #region variables
    int _maxEnergy = 50;
    public int currentEnergy;

    public Metronome metronome;

    #endregion

    private void Awake ( )
    {
        //Gets all Values needed for the start of the game
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
            if ( currentEnergy < 50 )
            {
                StandardEnemy ( );
            }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="HeavyEnemy" )
        {
            if ( currentEnergy < 50 )
            {
                HeavyEnemy ( );
            }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="RangedEnemy" )
        {
            if ( currentEnergy < 50 )
            {
                RangedEnemy ( );
            }
            Destroy ( enemy.gameObject );
        }
    }

    void StandardEnemy ( )
    {
        if ( metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
        else if ( !metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 5;
        }
    }

    void RangedEnemy ( )
    {
        if ( metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
        else if ( !metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 5;
        }
    }

    void HeavyEnemy ( )
    {
        if ( metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 20;
        }
        else if ( !metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
    }
}
