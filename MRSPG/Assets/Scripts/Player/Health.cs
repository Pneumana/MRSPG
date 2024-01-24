using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region variables

    int _maxHealth = 5;
    public int currentHealth;



    #endregion

    private void Awake()
    {
        currentHealth = _maxHealth;
    }

    public void LoseHealth(int amount)
    {
        currentHealth = -amount;
    }
}