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

            var distToTarget = Vector3.Distance(transform.position, me.pathEndPosition);
            if(distToTarget <= 0.55f)
            {
                //re-set 
                var rolledRadius = Random.Range(strafeRadiusMin, strafeRadiusMax);
                strafeAngle += Random.Range(-45,45);
                me.destination = player.transform.position + new Vector3(Mathf.Cos(strafeAngle * Mathf.Deg2Rad) * rolledRadius, 0, Mathf.Sin(strafeAngle * Mathf.Deg2Rad) * rolledRadius);
            }
            
        }
    }
}
