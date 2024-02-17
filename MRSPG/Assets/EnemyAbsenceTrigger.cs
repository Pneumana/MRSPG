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
        enemies.Remove(removed);
        if(enemies.Count ==0)
            parent.TriggerAll();
    }
}
