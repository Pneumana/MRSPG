using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Boss_health : MonoBehaviour
{

    #region variables

    int _maxHealth = 50;
    public int currentHealth;



    #endregion

    private void Awake ( )
    {
        currentHealth = _maxHealth;
    }

    public void LoseHealth ( int amount )
    {
        currentHealth = -amount;
    }

}
