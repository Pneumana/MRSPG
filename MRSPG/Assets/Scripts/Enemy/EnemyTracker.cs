using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public List<GameObject> ActiveEnemiesInScene = new List<GameObject>();
    public GameObject portal;
    public static EnemyTracker inst;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
        {
            if (inst != this)
                Destroy(gameObject);
        }
        portal.SetActive(false);
    }


    private void Update()
    {
        CheckEnemyActiveState();
    }
    void CheckEnemyActiveState()
    {
        if(ActiveEnemiesInScene.Count == 0)
        {
            portal.SetActive(true);
        }
    }
}
