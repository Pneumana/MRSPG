using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public bool destroyed;
    public int damage = 1;

    public Transform homingTarget;
    public float maxTurnDelta;
    public float acceleration;
    bool skippedFirstFrame;
    public LayerMask collisions;
    public Vector3 initDir;

    Vector3 prevpos;
    Vector3 deltaChange;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("start pos = " + transform.position, gameObject);
        prevpos = transform.position;
        initDir = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        deltaChange = prevpos - transform.position;
        if (deltaChange.magnitude > speed)
        {
            Debug.DrawLine(transform.position + Vector3.down, transform.position + Vector3.up, Color.magenta, 10);
            Debug.DrawLine(transform.position + Vector3.left, transform.position + Vector3.right, Color.magenta, 10);
            Debug.DrawLine(transform.position + Vector3.forward, transform.position + Vector3.back, Color.magenta, 10);
            transform.position = prevpos;
            transform.forward = initDir;
        }
        prevpos = transform.position;
        

        if (!destroyed)
        {
            if (acceleration != 0)
                speed += acceleration*Time.deltaTime;


            RaycastHit forwardCast;
            //Debug.Log(speed * Time.deltaTime + " is the length of the speed cast");
            Physics.Raycast(transform.position, transform.forward, out forwardCast, speed * Time.deltaTime,  collisions);
            if (forwardCast.collider == null)
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                //Debug.Log("hit " + forwardCast.collider.gameObject + " @ " + forwardCast.point, forwardCast.collider.gameObject);
                //Debug.Log("warping from " + transform.position + " to " + forwardCast.point, gameObject);
                if (Vector3.Distance(transform.position, forwardCast.point) <= speed*Time.deltaTime)
                    transform.position = forwardCast.point;

            }
            //raycast forward,if raycast.collider is null, do the folliwing
            //else set position to raycast.point
            if (homingTarget != null)
            {
                var _direction = (homingTarget.position - transform.position).normalized;
                var _lookRotation = Quaternion.LookRotation(_direction);


                if (!skippedFirstFrame)
                {
                    //Debug.Log(_direction);
                    skippedFirstFrame = true;
                }
                else
                {
                    transform.forward = Vector3.Slerp(transform.forward, _direction, Time.deltaTime * maxTurnDelta);
                }
            }
        }
        else
        {
            var list = GetComponentsInChildren<ParticleSystem>();
            int remainingParticles = 0;
            foreach (ParticleSystem ps in list)
            {
                remainingParticles += ps.particleCount;
            }
            if (remainingParticles == 0)
                Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("enemybullet hit " + collision.gameObject.name, collision.gameObject);
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red, 10);
        if (collision.gameObject.name == "PlayerObj")
        {
            //hurt player
            FindFirstObjectByType<Health>().LoseHealth(1);
        }
        else if (collision.gameObject.GetComponent<EnemyBody>() != null)
        {
            collision.gameObject.GetComponent<EnemyBody>().ModifyHealth(damage);
        }
        transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        destroyed = true;
        GetComponent<Collider>().enabled = false;
        var list = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in list)
        {
            var emm = ps.emission;
            emm.rateOverTimeMultiplier = 0;
        }
        var impactFX = transform.Find("Impact");
        if (impactFX != null)
        {
            var emm = impactFX.GetComponent<ParticleSystem>().emission;
            emm.rateOverTimeMultiplier = 1;
            impactFX.GetComponent<ParticleSystem>().Play();
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.forward * speed * Time.deltaTime), Color.red, Time.deltaTime);
    }
}
