using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerObj")
        {
            //hurt player
            GameObject.FindFirstObjectByType<Health>().LoseHealth(1);
        }
        if (other.gameObject.GetComponent<EnemyBody>() != null)
        {
            other.gameObject.GetComponent<EnemyBody>().ModifyHealth(1);
        }
    }
}
