using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBody : MonoBehaviour
{

    Vector3 startPosition;
    GameObject player;
    public EnemySetting _enemy;
    public int health;
    private int maxHealth;
    [SerializeField] float dashImpact = 3;

    public bool grounded;
    public bool pushedBack;
    public bool disablePathfinding;

    public bool gravityAffected = true;

    public float airTime = 0;

    public bool bounceOffPlayer;

    private bool DoWallDamage;

    float dragBeforeFall = -1;

    Rigidbody rb;
    public NavMeshAgent me;
    Transform groundCheck;
    [HideInInspector]public BattleBounds bounds;
    [HideInInspector]public List<EnemyAbsenceTrigger> triggerList = new List<EnemyAbsenceTrigger>();

    public List<DamageTypes> Immunities = new List<DamageTypes>();

    public enum DamageTypes
    {
        Basic,
        Explosive
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        me = GetComponent<NavMeshAgent>();
        groundCheck = transform.Find("GroundCheck");
        if(groundCheck!=null)
            groundCheck.transform.position += Vector3.up * Time.deltaTime;
        player = GameObject.Find("PlayerObj");
        if(_enemy!=null)
            SetEnemyData(_enemy);
        startPosition = transform.position;
    }
    void SetEnemyData(EnemySetting _enemy)
    {
        health = _enemy.EnemyHealth;
    }

    public void ModifyHealth(int mod, DamageTypes type = DamageTypes.Basic)
    {
        if(Immunities.Contains(type))
            return;
        health -= mod;
        StartCoroutine(Wait(1f));
        if(this.GetComponent<Animator>()!=null)
            this.GetComponent<Animator>().SetBool("TakeDamage", true);
        if (Metronome.inst.IsOnBeat(true))
        {
            ComboManager.inst.AddEvent("On Beat Attack", 15);
        }

        if(health <= 0)
        {
            Die();
        }

    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (this.GetComponent<Animator>() != null)
            this.GetComponent<Animator>().SetBool("TakeDamage", false);
    }
    void Die()
    {
        if (bounds != null)
        { bounds.defeated++; }
        /*Debug.Log("The enemy has died");
        if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(10); }
        else { energy.GainEnergy(5); }*/

/*        foreach (EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this);
        }*/

        foreach (EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this);
        }
        disablePathfinding = true;
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
    private void Update()
    {
        //add some sort of airborne check?
        if (gravityAffected)
        {
            if(!grounded)
                grounded = Physics.Raycast(groundCheck.position, Vector3.down, (-rb.velocity.y*Time.deltaTime) + Time.deltaTime * 2, LayerMask.GetMask("Ground", "Default"));
        Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.down * ((-rb.velocity.y * Time.deltaTime) + Time.deltaTime * 2)), Color.red, 10);
        if (!grounded)
        {
                if(dragBeforeFall==-1)
                    dragBeforeFall = rb.drag;
                rb.drag = 0;
            airTime += Time.deltaTime;
            DisablePathfinding();
        }
        else
        {
            if(airTime > 1)
            {
                    airTime = 0;
                    Debug.Log("floor splat");

                //take fall damage
                ModifyHealth(1);
                    EnablePathfinding();
                    if(me != null)
                        me.enabled = true;
                    rb.isKinematic = true;
                    Debug.Log(gameObject.name + " recovered from fall");
                }
                else if(airTime > 0)
                {
                    airTime = 0;
                    EnablePathfinding();
                    if (me != null)
                        me.enabled = true;
                    rb.isKinematic = true;
                    Debug.Log(gameObject.name + " recovered from fall");
                }
                
            }
            if (dragBeforeFall != -1)
            {
                rb.drag = dragBeforeFall;
                dragBeforeFall = -1;
            }


        }
        if (health <= 0)
        {
            Die();
        }
            

        if (pushedBack)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + rb.velocity.normalized, rb.velocity.normalized, out hit, rb.velocity.magnitude * Time.deltaTime) && DoWallDamage)
            {
                Debug.DrawLine(transform.position + rb.velocity.normalized, hit.point, Color.white, 10);
                Debug.Log(hit.collider.name + " ouched " + gameObject.name, hit.collider.gameObject);

                ModifyHealth(5);
                rb.velocity = Vector3.zero;
                Vector3 point = hit.point;
                point.y += 0.1f;
                Vector3 normal = hit.normal;
                if (GameObject.Find("DecalPainter") != null)
                {
                    DecalPainter painter = GameObject.Find("DecalPainter").GetComponent<DecalPainter>();
                    StartCoroutine(painter.PaintDecal(point, normal, hit.collider));
                }
            }
            if (Mathf.Abs(rb.velocity.magnitude) < 0.1f && !disablePathfinding)
            {
                if (me != null)
                {
                    if (!me.enabled && grounded && gravityAffected || !me.enabled && !gravityAffected)
                    {
                        me.enabled = true;
                        rb.isKinematic = true;
                        Debug.Log(gameObject.name + " recovered from pushback");
                    }
                }
                pushedBack = false;
                //Debug.Log("should be recovering");
            }
        }

    }

    public void DisablePathfinding()
    {
        if (me != null)
            me.enabled = false;
        rb.isKinematic = false;
        disablePathfinding = true;
    }
    public void EnablePathfinding()
    {
        disablePathfinding = false;
    }

   /* private void OnCollisionEnter(Collision collision)
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
            Debug.Log("Painted decal");
        }
    }*/
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "DeathPanel")
        {
            Die();
        }
    }

    public void HitByPlayerDash(Transform player)
    {
        Debug.Log(gameObject.name + " pushed by player");
        //var dir = player.position - transform.position;
        if (InputControls.instance.dashTime > 0)
        {
            Shoved(player.forward * dashImpact, "Dash");
        }
        else
        {
            if (bounceOffPlayer)
            {
                GetComponent<Rigidbody>().velocity = -transform.forward * dashImpact;
            }
        }
    }
    public void Shoved(Vector3 dir, string source, ForceMode mode = ForceMode.Impulse)
    {
        if (!pushedBack)
        {

        }
        Debug.Log(gameObject.name + " shoved");
        pushedBack = true;
        rb.isKinematic = false;
        DoWallDamage = (source == "Dash");
        if (source == "Dash") { rb.AddForce(dir, mode); }
        else { rb.velocity = dir; }
    }

    public void Recover()
    {
        if (me != null)
            me.enabled = true;
        //rb.isKinematic = true;
        //Debug.Log(gameObject.name + "recovered");
    }


    public void Respawn()
    {
        transform.position = startPosition;
        health = _enemy.EnemyHealth;

        foreach (EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this);
        }

        ModifyHealth(0);
    }

    private void OnDestroy()
    {
       
    }
}
