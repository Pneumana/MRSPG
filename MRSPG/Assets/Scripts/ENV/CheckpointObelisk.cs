using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointObelisk : MonoBehaviour, IEnvTriggered
{
    public Vector3 spawnPosition;
    GameObject player;
    AnimateObelisk anim;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        RaycastHit hit;
        if(Physics.Raycast(transform.position+ transform.forward, Vector3.down, out hit))
        {
            spawnPosition = hit.point;
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
        Debug.Log("triggered");
        player.GetComponent<Health>().currentCheckpoint = this;
        if (anim != null)
            anim.StartCoroutine("AnimateWakeUp");
        //StartCoroutine(MoveLoop(delay));
    }
}
