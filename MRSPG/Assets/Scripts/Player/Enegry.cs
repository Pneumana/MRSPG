using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enegry : MonoBehaviour
{
    #region variables
    int _maxEnergy = 50;
    public int currentEnergy;

    [SerializeField]
    Image _energyImg;

    [SerializeField]
    Sprite [ ] _energySprite;

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
        UIUpdateEnergy ( );       
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
        if ( Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
        else if ( !Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 5;
        }
    }

    void RangedEnemy ( )
    {
        if ( Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
        else if ( !Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 5;
        }
    }

    void HeavyEnemy ( )
    {
        if ( Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 20;
        }
        else if ( !Metronome.inst.IsOnBeat ( ) )
        {
            currentEnergy += 10;
        }
    }

    void UIUpdateEnergy ( )
    {
        if ( currentEnergy == 50 )
        {
            _energyImg.sprite = _energySprite [ 0 ];
        }
        else if ( currentEnergy == 45 )
        {
            _energyImg.sprite = _energySprite [ 1 ];
        }
        else if ( currentEnergy == 40 )
        {
            _energyImg.sprite = _energySprite [ 2 ];
        }
        else if ( currentEnergy == 35 )
        {
            _energyImg.sprite = _energySprite [ 3 ];
        }
        else if ( currentEnergy == 30 )
        {
            _energyImg.sprite = _energySprite [ 4 ];
        }
        else if ( currentEnergy == 25 )
        {
            _energyImg.sprite = _energySprite [ 5 ];
        }
        else if ( currentEnergy == 20 )
        {
            _energyImg.sprite = _energySprite [ 6 ];
        }
        else if ( currentEnergy == 15 )
        {
            _energyImg.sprite = _energySprite [ 7 ];
        }
        else if ( currentEnergy == 10 )
        {
            _energyImg.sprite = _energySprite [ 8 ];
        }
        else if ( currentEnergy == 5 )
        {
            _energyImg.sprite = _energySprite [ 9 ];
        }
        else if ( currentEnergy == 0 )
        {
            _energyImg.sprite = _energySprite [ 10 ];
        }
    }
}
