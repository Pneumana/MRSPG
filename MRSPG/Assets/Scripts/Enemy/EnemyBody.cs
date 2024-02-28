using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBody : MonoBehaviour
{
    GameObject player;
    public EnemySetting _enemy;
    private int health;
    private float speed;
    private int maxHealth;
    private float dashImpact = 3f;

    public bool pushedBack;
    public bool disablePathfinding;

    public bool bounceOffPlayer;

    Rigidbody rb;
    public NavMeshAgent me;

    [HideInInspector]public List<EnemyAbsenceTrigger> triggerList = new List<EnemyAbsenceTrigger>();

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
        if(health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        foreach(EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this);
        }
        disablePathfinding = true;
        Destroy(gameObject);
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
            ModifyHealth(5);
            ContactPoint contact = collision.contacts[0];
            Vector3 point = contact.point;
            point.y += 0.1f;
            Vector3 normal = contact.normal;
            DecalPainter painter = GameObject.Find("DecalPainter").GetComponent<DecalPainter>();
            StartCoroutine(painter.PaintDecal(point, normal, collision));
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "DeathPanel")
        {
            Die();
        }
    }

    public void HitByPlayerDash()
    {
        Debug.Log(gameObject.name + " pushed by player");
        if (!InputControls.instance.canDash)
        {
            Shoved(-transform.forward * dashImpact);
        }
        else
        {
            if (bounceOffPlayer)
            {
                GetComponent<Rigidbody>().velocity = -transform.forward * dashImpact;
            }
        }
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
        Debug.Log(gameObject.name + "recovered");
    }
}
