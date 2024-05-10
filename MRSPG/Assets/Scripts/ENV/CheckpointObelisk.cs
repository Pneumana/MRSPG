using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointObelisk : MonoBehaviour, IEnvTriggered
{
    public Vector3 spawnPosition;
    GameObject player;
    AnimateObelisk anim;
    Health health;
    bool activated;

    public List<GameObject> encountersAhead = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        health = player.GetComponent<Health>();
        RaycastHit hit;
        if(Physics.Raycast(transform.position+ transform.forward * 2, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            spawnPosition = hit.point;
            Debug.DrawLine(transform.position + transform.forward * 2, hit.point, Color.white, 10);
        }
        
        anim = GetComponent<AnimateObelisk>();
        //raycast from  forward down to get spawn position
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activated(float delay)
    {
        if (activated)
            return;
        activated = true;
        //Debug.Log("triggered");
        player.GetComponent<Health>().currentCheckpoint = this;
        if (anim != null)
            anim.StartCoroutine("AnimateWakeUp");
        //StartCoroutine(MoveLoop(delay));

        //Called lose health but it adds 1 health to the player here
        if(health.currentHealth < health._maxHealth)
        {
            health.LoseHealth(-1);
            StartCoroutine(health.HealHUD());
        }
    }

    public void RespawnReset()
    {
        Debug.Log("respawn reset");
        /*foreach(GameObject go in encountersAhead)
        {
            Debug.Log(go.name + " is resetting");
            go.SetActive(true);
            var bb = go.GetComponent<BattleBounds>();

            bb.StopAllCoroutines();
            bb.inBattle = false;
            if(bb.defeated!=bb.enemies.Count)
                bb.defeated = 0;
            foreach(GameObject enemy in bb.enemies)
            {
                enemy.SetActive(true);
                enemy.GetComponent<EnemyBody>().Respawn();

            }
        }*/
    }
}
