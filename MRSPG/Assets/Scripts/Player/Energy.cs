using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    #region variables
    int _maxEnergy = 50;
    public int currentEnergy;

    [SerializeField]
    Image _energyImg, _gunRdy, _teleportRdy;

    [SerializeField]
    Sprite [ ] _energySprite;

    #endregion

    private void Awake ( )
    {
        //Gets all Values needed for the start of the game
        currentEnergy = _maxEnergy;
        UIUpdateEnergy();
    }

    private void Update ( )
    {
        //Makes sure Energy Level does not go over the Max allowed Level
        
        
    }

    public void LoseEnergy (int amount )
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, _maxEnergy);
        UIUpdateEnergy();
    }
      
    //adds energy after every enemy killed
    private void OnCollisionExit ( Collision enemy )
    {
        if ( enemy.collider.name == "StandardEnemy")
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(10); }
            else { GainEnergy(5); }
            Debug.Log("Killed a standard enemy.");
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="HeavyEnemy" )
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(20); }
            else { GainEnergy(10); }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="RangedEnemy" )
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(10); }
            else { GainEnergy(5); }
            Destroy ( enemy.gameObject );
        }
    }
    /// <summary>
    /// standard: 10/5, ranged:10,5, heavy: 20/10
    /// </summary>
    public void GainEnergy(int energy) 
    {
        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, _maxEnergy);
/*        if (currentEnergy > 50)
        {
            currentEnergy = 50;
        }*/
        UIUpdateEnergy();
    }
    [ContextMenu("UpdateUI")]
    void UIUpdateEnergy ( )
    {
        _energyImg.fillAmount = currentEnergy/50f;
        if(currentEnergy == 50)
            _gunRdy.gameObject.SetActive(false);
        else
        {
            _gunRdy.gameObject.SetActive(true);
        }
        if (currentEnergy >= 20)
            _teleportRdy.gameObject.SetActive(false);
        else
        {
            _teleportRdy.gameObject.SetActive(true);
        }
        /*if ( currentEnergy == 50 )
        {
            _energyImg.sprite = _energySprite [ 0 ];
            _gunRdy.gameObject.SetActive ( true );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 45 )
        {
            _energyImg.sprite = _energySprite [ 1 ];
            _gunRdy.gameObject.SetActive(false);
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 40 )
        {
            _energyImg.sprite = _energySprite [ 2 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 35 )
        {
            _energyImg.sprite = _energySprite [ 3 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 30 )
        {
            _energyImg.sprite = _energySprite [ 4 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 25 )
        {
            _energyImg.sprite = _energySprite [ 5 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 20 )
        {
            _energyImg.sprite = _energySprite [ 6 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 15 )
        {
            _energyImg.sprite = _energySprite [ 7 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 10 )
        {
            _energyImg.sprite = _energySprite [ 8 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 5 )
        {
            _energyImg.sprite = _energySprite [ 9 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 0 )
        {
            _energyImg.sprite = _energySprite [ 10 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy .gameObject.SetActive ( false );
        }
        else { return; }*/
    }

}
