using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DamageVolume : MonoBehaviour
{
    public EnemyBody.DamageTypes damageType = EnemyBody.DamageTypes.Trap;
    public EnemyBody owner;
    public int damage = 1;
    public bool useHurtList = false;
    List<GameObject> hurtList = new List<GameObject>();

    void Awake()
    {
        if (owner == null)
        {
            Transform currentPar = transform.parent;
            while (currentPar != null)
            {
                if (currentPar.GetComponent<EnemyBody>() != null)
                {
                    owner = currentPar.GetComponent<EnemyBody>();
                    break;
                }
                currentPar = currentPar.parent;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useHurtList)
        {
            if (!hurtList.Contains(other.gameObject))
            {
                bool used = false;
                if (other.gameObject.name == "PlayerObj")
                {
                    used = true;
                    //hurt player
                    GameObject.FindFirstObjectByType<Health>().LoseHealth(damage);
                }
                if (other.gameObject.GetComponent<EnemyBody>() != null)
                {
                    if (other.gameObject.GetComponent<EnemyBody>() != owner)
                    {
                        other.gameObject.GetComponent<EnemyBody>().ModifyHealth(damage, damageType);
                        used = true;
                    }
                }
                if (used)
                    hurtList.Add(other.gameObject);
            }
        }
        else
        {
            if (other.gameObject.name == "PlayerObj")
            {
                //hurt player
                GameObject.FindFirstObjectByType<Health>().LoseHealth(damage);
            }
            if (other.gameObject.GetComponent<EnemyBody>() != null)
            {
                if (other.gameObject.GetComponent<EnemyBody>() != owner)
                {
                    other.gameObject.GetComponent<EnemyBody>().ModifyHealth(damage, damageType);
                }
            }
        }

        
    }
    public void Clear()
    {
        //Debug.Log("cleared", gameObject);
        hurtList.Clear();
    }

    private void OnEnable()
    {
        Clear();
    }
}
