using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CircleStrafePlayerAI : MonoBehaviour
{

    bool aggrod = true;

    public float strafeRadiusMin;
    public float strafeRadiusMax;
    public float strafeAngle;

    public float lungeDistance;
    public float lungeSpeed;

    bool lungeing;
    public float lungeCooldown;
    float lungeCD;

    GameObject player;
    NavMeshAgent me;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        me = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        var distToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (aggrod)
        {
            //add a check here for nearby CircleStrafePlayerAIs
            if (distToPlayer < strafeRadiusMax && Metronome.inst.IsOnBeat())
            {
                if (!lungeing)
                {
                    if (lungeCD <= 0)
                    {
                        StartCoroutine(Lunge());
                    }
                    else
                    {
                        lungeCD -= Time.deltaTime;
                    }
                }

            }
            var distToTarget = Vector3.Distance(transform.position, me.pathEndPosition);
            if(distToTarget <= 0.55f && !lungeing)
            {
                //re-set 
                var rolledRadius = Random.Range(strafeRadiusMin, strafeRadiusMax);
                strafeAngle += Random.Range(-45,45);
                me.destination = player.transform.position + new Vector3(Mathf.Cos(strafeAngle * Mathf.Deg2Rad) * rolledRadius, 0, Mathf.Sin(strafeAngle * Mathf.Deg2Rad) * rolledRadius);
                


            }
            
        }
    }

    IEnumerator Lunge()
    {
        me.enabled = false;
        lungeing = true;
        //transform.forward = player.transform.position - transform.position;
        float currentLunge = 0;
        GetComponent<Rigidbody>().AddForce(((player.transform.position - transform.position) + Vector3.up) * lungeSpeed, ForceMode.Impulse);
        do
        {
            //transform.position += transform.forward * (Time.deltaTime * lungeSpeed);
            currentLunge += Time.deltaTime * lungeSpeed;
            yield return new WaitForSeconds(0);
        }while (currentLunge <lungeDistance) ;
        me.enabled = true;
        lungeing=false;
        lungeCD = lungeCooldown;
        yield return null;
    }



}
