using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbsenceTrigger : MonoBehaviour
{
    EnvTrigger parent;
    public List<EnemyBody> enemies = new List<EnemyBody>();
    void Start()
    {
        parent = GetComponentInParent<EnvTrigger>();
        foreach(EnemyBody enemy in enemies)
        {
            enemy.triggerList.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateEnemyList(EnemyBody removed)
    {
        if (enemies.Contains(removed))
        {
            enemies.Remove(removed);
        }
        else
        {
            enemies.Add(removed);
        }
        if(enemies.Count == 0)
        {
            if(parent!=null)
                parent.TriggerAll();

        }

    }
}
