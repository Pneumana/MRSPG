using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBody : MonoBehaviour
{
    GameObject player;
    [SerializeField] private EnemySetting _enemy;
    private int health;
    private float speed;
    private int maxHealth;

    public bool pushedBack;
    public bool disablePathfinding;

    public bool bounceOffPlayer;

    Rigidbody rb;
    public NavMeshAgent me;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        me = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PlayerObj");
        SetEnemyData(_enemy);
    }
    void SetEnemyData(EnemySetting _enemy)
    {
        health = _enemy.EnemyHealth;
        speed = _enemy.speed;
    }

    public void ModifyHealth(int mod)
    {
        health -= mod;
        
    }

    private void Update()
    {
        if (pushedBack)
        {
            if (Mathf.Abs(rb.velocity.magnitude) < 0.1f && !disablePathfinding)
            {
                if (me != null)
                {
                    if (!me.enabled)
                    {
                        Debug.Log("recovered");
                        me.enabled = true;
                    }
                }
                pushedBack = false;
                Debug.Log("should be recovering");
            }
        }

    }

    public void DisablePathfinding()
    {
        me.enabled = false;
        disablePathfinding = true;
    }
    public void EnablePathfinding()
    {
        disablePathfinding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && pushedBack)
        {
            Debug.Log(health);
            ModifyHealth(-5);
        }
        if(collision.gameObject == player)
        {
            if (!InputControls.instance.canDash)
            {
                Shoved(-transform.forward * 7);
            }
            else
            {
                if (bounceOffPlayer)
                {
                    GetComponent<Rigidbody>().velocity = -transform.forward * 7;
                }
            }
        }
        //do ground detection/falling here
    }

    void Shoved(Vector3 dir, ForceMode mode = ForceMode.Impulse)
    {
        Debug.Log("shoved");
        pushedBack = true;
        rb.AddForce(dir, mode);
    }

    public void Recover()
    {
        me.enabled = true;
        Debug.Log("recovered");
    }
}
