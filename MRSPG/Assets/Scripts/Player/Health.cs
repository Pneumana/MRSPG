using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    #region variables

    int _maxHealth = 5;
    public int currentHealth;

    [SerializeField]
    Image _healthImg;

    [SerializeField]
    Sprite [ ] _healthSprite;

    [HideInInspector] public CheckpointObelisk currentCheckpoint;
    #endregion

    private void Awake()
    {
        currentHealth = _maxHealth;
    }

    private void Update ( )
    {
        
    }

    public void LoseHealth(int amount)
    {
        currentHealth -= amount;
        UIUpdateHealth();
    }

    void UIUpdateHealth ( )
    {
        if ( currentHealth == 5 )
        {
            _healthImg.sprite = _healthSprite [ 0 ];
        }
        else if ( currentHealth == 4 )
        {
            _healthImg.sprite = _healthSprite [ 1 ];
        }
        else if ( currentHealth == 3 )
        {
            _healthImg.sprite = _healthSprite [ 2 ];
        }
        else if ( currentHealth == 2 )
        {
            _healthImg.sprite = _healthSprite [ 3 ];
        }
        else if ( currentHealth == 1 )
        {
            _healthImg.sprite = _healthSprite [ 4 ];
        }
        else if ( currentHealth == 0 )
        {
            _healthImg.sprite = _healthSprite [ 5 ];
            Die();
        }
    }

    public void Die()
    {
        GameObject.Find("PlayerObj").transform.position = currentCheckpoint.spawnPosition + Vector3.up;
        currentHealth = 5;
        UIUpdateHealth();
    }

}