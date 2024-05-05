using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public List<GameObject> ActiveEnemiesInScene = new List<GameObject>();
    public GameObject portal;
    public static EnemyTracker inst;

    [SerializeField] Animator anim;

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
        anim.enabled = false;
    }


    private void Update()
    {
        CheckEnemyActiveState();
    }
    void CheckEnemyActiveState()
    {
        if(ActiveEnemiesInScene.Count == 0)
        {
            if(anim != null)
            {
                anim.enabled = true;
            }
            portal.SetActive(true);
        }
    }
}
