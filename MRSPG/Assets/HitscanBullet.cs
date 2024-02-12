using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HitscanBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit groundCast;
        RaycastHit[] enemyCast;

        int groundMask = 1 << 7;
        int enemyMask = 1 << 6;

        groundMask = ~groundMask;
        enemyMask = ~enemyMask;

        //Physics.Raycast(transform.position, transform.forward, out groundCast, Mathf.Infinity);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 100, Color.blue, 10);
        if (Physics.Raycast(transform.position, transform.forward, out groundCast, Mathf.Infinity, groundMask))
        {
            Debug.Log("ground target = " + groundCast.distance);
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, groundCast.point);
            //Debug.DrawLine(transform.position, groundCast.point, Color.blue, 10);
            enemyCast = Physics.RaycastAll(transform.position, transform.forward, groundCast.distance);
            if (enemyCast.Length > 0)
            {
                Debug.Log("Hit enemy x " + enemyCast.Length + " " + enemyCast[0].collider.gameObject.name, enemyCast[0].collider.gameObject);
                Debug.DrawLine(transform.position, groundCast.point, Color.red, 10);
                foreach (RaycastHit hit in enemyCast)
                {
                    if (hit.collider != null)
                    {
                        var AI = hit.collider.gameObject.GetComponent<CircleStrafePlayerAI>();
                        if (AI != null)
                        {
                            AI.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                            AI.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x, 0.1f, transform.forward.z) * 15, ForceMode.Impulse);
                        }


                    }
                }
            }
            else
            {
                Debug.DrawLine(transform.position, groundCast.point, Color.green, 10);
            }
            
            //Debug.Log("Hit ground");
            //Debug.DrawLine(transform.position, groundCast.point, Color.blue, 10);
        }
        Destroy(gameObject, 10f);
    }
}
