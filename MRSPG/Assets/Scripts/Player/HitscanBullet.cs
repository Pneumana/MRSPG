using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HitscanBullet : MonoBehaviour
{
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hitscan bullet spawned");
        lr = GetComponent<LineRenderer>();
        RaycastHit groundCast;
        RaycastHit[] enemyCast;

        int groundMask = 1 << 7;
        int enemyMask = 1 << 6;

        groundMask = ~groundMask;
        enemyMask = ~enemyMask;

        //Physics.Raycast(transform.position, transform.forward, out groundCast, Mathf.Infinity);
        //GetComponent<LineRenderer>().SetPosition(0, transform.position);
        //GetComponent<LineRenderer>().SetPosition(1, transform.position + (transform.forward * 9999));
        if (Physics.Raycast(transform.position, transform.forward, out groundCast, 9999, LayerMask.GetMask("Ground")))
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * groundCast.distance, Color.blue, 10);
            Debug.Log("ground target = " + groundCast.distance);
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, groundCast.point);
            //Debug.DrawLine(transform.position, groundCast.point, Color.blue, 10);
            enemyCast = Physics.RaycastAll(transform.position, transform.forward, groundCast.distance, LayerMask.GetMask("Enemy"));
            if (enemyCast.Length > 0)
            {
                Debug.Log("Hit enemy x " + enemyCast.Length + " " + enemyCast[0].collider.gameObject.name, enemyCast[0].collider.gameObject);
                //Destroy(enemyCast[0].collider.gameObject);
                
                foreach (RaycastHit hit in enemyCast)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 10);
                    if (hit.collider != null)
                    {
                        var barrel = hit.collider.gameObject.GetComponent<BoomBarrel>(); ;
                        if (barrel != null)
                        {
                            barrel.TryExplode();
                            continue;
                        }
                        else
                        {
                            Debug.Log("no exploding barrel component on " + gameObject);
                        }

                        var body = hit.collider.gameObject.GetComponent<EnemyBody>();
                        if (body != null)
                        {
                            body.ModifyHealth(5);
                            Debug.Log("hurting " + body.gameObject.name);
                        }
                        //var AI = hit.collider.gameObject.GetComponent<CircleStrafePlayerAI>();
                        /*if (AI != null)
                        {
                            AI.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                            AI.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x, 0.1f, transform.forward.z) * 15, ForceMode.Impulse);
                        }*/


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
        else
        {
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position + (transform.forward * 9999));
            //Debug.DrawLine(transform.position, groundCast.point, Color.blue, 10);
            enemyCast = Physics.RaycastAll(transform.position, transform.forward, 9999, LayerMask.GetMask("Enemy"));
            if (enemyCast.Length > 0)
            {
                Debug.Log("Hit enemy x " + enemyCast.Length + " " + enemyCast[0].collider.gameObject.name, enemyCast[0].collider.gameObject);
                //enemyCast[0].collider.gameObject.SetActive(false);
                //Destroy(enemyCast[0].collider.gameObject);

                foreach (RaycastHit hit in enemyCast)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 10);
                    if (hit.collider != null)
                    {
                        var body = hit.collider.gameObject.GetComponent<EnemyBody>();
                        if (body != null)
                        {
                            body.ModifyHealth(5);
                            Debug.Log("hurting " + body.gameObject.name);
                        }
                        var AI = hit.collider.gameObject.GetComponent<CircleStrafePlayerAI>();
                        if (AI != null)
                        {
                            AI.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                            AI.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x, 0.1f, transform.forward.z) * 15, ForceMode.Impulse);
                        }


                    }
                }
            }
        }
        //Destroy(gameObject, 10f);
        StartCoroutine(Dissipate());
    }
    IEnumerator Dissipate()
    {
        float t = 0;
        do
        {
            t += Time.deltaTime * 2;
            lr.widthMultiplier = (t * 4) + 1;
            lr.material.SetFloat("_Fade", t);
            yield return new WaitForSeconds(0);
        } while (t < 1);
        Debug.Log("despawning hitscan bullet");
        Destroy(gameObject);
        yield return null;
    }
}
