using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region  Variables
    Health _playerHealth;
    Energy _playerEnergy;

    [SerializeField]
    Image _healthImg, _energyImg;
    [SerializeField]
    Sprite [ ] _healthSprite;
    [SerializeField]
    Sprite [ ] _energySprite;
        
    #endregion


    private void Start ( )
    {
        _playerHealth = GameObject.Find ( "Player" ).GetComponent<Health> ( );
        _playerEnergy = GameObject.Find ( "Player" ).GetComponent<Energy> ( );

        if ( _playerHealth == null )
        {
            Debug.LogError ( "Player is NULL" );
        }
    }

    public void HealthUIUpdater ( int currentHealth )
    {
        _healthImg.sprite = _healthSprite [currentHealth];
    }

    public void EnergyUIUpdater( int currentEnergy )
    {
        _energyImg.sprite = _energySprite [currentEnergy];
    }


}
