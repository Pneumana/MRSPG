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

    DeathPlane dp;

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
        Explosive,
        Trap,
        Impact,
        DeathPlane
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

        dp = FindFirstObjectByType<DeathPlane>();
    }
    void SetEnemyData(EnemySetting _enemy)
    {
        health = _enemy.EnemyHealth;
        if(health >= 1 && _enemy.type != EnemyType.Crystal)
        {
            EnemyTracker.inst.ActiveEnemiesInScene.Add(gameObject);
        }
    }

    public void ModifyHealth(int mod, DamageTypes type = DamageTypes.Basic)
    {
        if(Immunities.Contains(type))
            return;
        health -= mod;
        //Sounds.instance.PlaySFX ( "Hit Marker" );
        StartCoroutine(Wait(1f));
        if(this.GetComponent<Animator>()!=null)
            this.GetComponent<Animator>().SetBool("TakeDamage", true);



        if (Metronome.inst.IsOnBeat(true))
        {
            ComboManager.inst.AddEvent("On Beat Attack", 15);
            //Sounds.instance.PlaySFX ( "On Beat" );
        }

        if(health <= 0)
        {
            Die(type);
        }

    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (this.GetComponent<Animator>() != null)
            this.GetComponent<Animator>().SetBool("TakeDamage", false);
    }
    void Die(DamageTypes type)
    {
        EnemyTracker.inst.ActiveEnemiesInScene.Remove(gameObject);
        Debug.Log("die");
        if (bounds != null)
        { bounds.defeated++; }
        var energy = GameObject.FindFirstObjectByType<Energy>();
        if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(10, transform.position); }

        switch (type)
        {
            case DamageTypes.Basic: ComboManager.inst.AddEvent("Sliced", 7); break;
            case DamageTypes.Explosive: ComboManager.inst.AddEvent("Exploded", 7); break;
            case DamageTypes.Trap: ComboManager.inst.AddEvent("Tricked", 8); break;
            case DamageTypes.Impact: ComboManager.inst.AddEvent("Splattered", 5); break;
            case DamageTypes.DeathPlane: ComboManager.inst.AddEvent("Cliffed", 3); break;
        }
        /*Debug.Log("The enemy has died");
        */

        /*        foreach (EnemyAbsenceTrigger trigger in triggerList)
                {
                    trigger.UpdateEnemyList(this);
                }*/

        foreach (EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this, true);
        }

        disablePathfinding = true;
        gameObject.SetActive(false);
        //LockOnSystem.LOS.UpdateEnemyList();
        if (LockOnSystem.LOS.trackedEnemy == gameObject)
        {
            if (!LockOnSystem.LOS.freeAim)
            {
                LockOnSystem.LOS.StopLockOn();
                LockOnSystem.LOS.StartLockOn();
            }
        }


        //Destroy(gameObject);
    }
    private void Update()
    {
        //add some sort of airborne check?
        if (gravityAffected)
        {
            if (!grounded)
                grounded = Physics.Raycast(groundCheck.position, Vector3.down, (rb.velocity.y * Time.deltaTime) + 0.3f, LayerMask.GetMask("Ground", "Default"));
            Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.down * ((rb.velocity.y * Time.deltaTime) + 0.3f)), Color.red, 10);
            if (!grounded)
            {
                if (dragBeforeFall == -1)
                    dragBeforeFall = rb.drag;
                rb.drag = 0;
                airTime += Time.deltaTime;
                DisablePathfinding();

            }

            else
            {
                if (airTime > 1)
                {
                    airTime = 0;
                    Debug.Log("floor splat");

                    //take fall damage
                    ModifyHealth(1, DamageTypes.Impact);
                    EnablePathfinding();
                    if (me != null)
                        me.enabled = true;
                    rb.isKinematic = true;
                    Debug.Log(gameObject.name + " recovered from fall");
                }
                else if (airTime > 0)
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

        if(dp!=null)
        {
            if (transform.position.y < dp.yStart)
                Die(DamageTypes.DeathPlane);
        }

        if (pushedBack)
        {
            RaycastHit hit;
            if (rb == null)
                return;
            if(Physics.Raycast(transform.position + rb.velocity.normalized, rb.velocity.normalized, out hit, rb.velocity.magnitude * Time.deltaTime) && DoWallDamage)
            {
                //Debug.DrawLine(transform.position + rb.velocity.normalized, hit.point, Color.white, 10);
                //Debug.Log(hit.collider.name + " ouched " + gameObject.name, hit.collider.gameObject);
                if (hit.collider.gameObject.CompareTag("Enemy")) { return; }
                ModifyHealth(5, DamageTypes.Impact);
                DoWallDamage = false;
                rb.velocity = Vector3.zero;
                Vector3 point = hit.point;
                point.y += 0.1f;
                Vector3 normal = hit.normal;
                if (GameObject.Find("DecalPainter") != null)
                {
                    DecalPainter painter = GameObject.Find("DecalPainter").GetComponent<DecalPainter>();
                    if(gameObject.activeSelf)
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
                        //Debug.Log(gameObject.name + " recovered from pushback");
                    }
                }
                pushedBack = false;
                rb.mass = 50;
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

    public void HitByPlayerDash(Transform player)
    {
        //Debug.Log(gameObject.name + " pushed by player", gameObject);
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
        if (pushedBack)
        {
            return;
        }
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.mass = _enemy.knockbackMass;
            rb.velocity = dir;
        }
        pushedBack = true;
        DoWallDamage = (source == "Dash");
        if (source == "Dash") {
            /*if (rb != null)
                rb.AddForce(dir, mode); */
        }
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
        gameObject.SetActive(true);
        EnablePathfinding();
        foreach (EnemyAbsenceTrigger trigger in triggerList)
        {
            trigger.UpdateEnemyList(this, false);
        }
        if(GetComponent<Enemy>()!=null)
        {
            GetComponent<Enemy>().Animations.Play("Base Layer.Idle", 0, 0f);
            GetComponent<Enemy>().CanAttack = true;
            //reset variables here
        }
        ModifyHealth(0);
        if (GetComponent<HealthBar>() != null)
            GetComponent<HealthBar>().Refresh();

        if (!EnemyTracker.inst.ActiveEnemiesInScene.Contains(gameObject))
        {
            EnemyTracker.inst.ActiveEnemiesInScene.Add(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Application.isPlaying)
            Debug.Log("<color=red>ENEMY</color> " + gameObject.name.ToUpper() + " <color=red>WAS DESTROYED!</color>");
    }
}
