using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;

/// <summary>
/// Utilize the data from the EnemySetting script to get all enemy data.
/// Will keep track and play through attack cycles.
/// </summary>
public class BasicEnemyAI : MonoBehaviour
{
    #region Variables

    private Transform enemyObj;
    public Animator animations;
    Metronome metronome;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Enemy Parameters")]
    private Rigidbody rb;
    private Vector3 targetPos;
    private Vector3 lookatvector;
    private int health;
    private float speed;
    private float TimeBetweenAttacks;
    private float ChargeTime;
    private ParticleSystem ChargeParticle;
    private bool CanAttack = true;

    [Header("Target Parameters")]
    private GameObject _player;
    private Energy energy;
    private Health _playerhealth;
    private bool playerInRange;
    private float maxdistance = 6f;

    [Header("Damage Numbers")]
    private int dashDamage = 5;



    #endregion
    public bool aggrod;
    EnemyBody body;

    public void Start()
    {
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        _player = GameObject.Find("Player/PlayerObj");
        energy = GameObject.Find("Player").GetComponent<Energy>();
        body = GetComponent<EnemyBody>();
        enemyObj = gameObject.transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        //SetEnemyData(_enemy);
        if (_player != null)
        {
            _playerhealth = GameObject.Find("Player").GetComponent<Health>();
        }else if (_player == null) { Debug.Log("<color=red>Error: </color> Player was not found.");  }
        if(enemyObj.gameObject.GetComponent<ParticleSystem>() != null)
        {
            ChargeParticle = enemyObj.gameObject.GetComponent<ParticleSystem>();
            ChargeParticle.Stop();
        }
    }

    private void FixedUpdate()
    {
        if(CheckForPlayer(transform.position, maxdistance, _player.GetComponent<Collider>()))
        {
            if(body.me.enabled)
                body.me.destination = _player.transform.position;
            aggrod = true;
            enemyObj.LookAt(lookatvector);
            float distanceToPlayer = targetPos.magnitude;
            if (distanceToPlayer > 2f)
            {
                float adjustedSpeed = Mathf.Lerp(0, speed, Mathf.Clamp01(distanceToPlayer / maxdistance));

                Vector3 move = targetPos.normalized * adjustedSpeed * Time.fixedDeltaTime;
                //rb.MovePosition(transform.position + move);

            }
        }
    }

    public void Update()
    {
        targetPos = _player.transform.position - transform.position;
        lookatvector = _player.transform.position;
        lookatvector.y = transform.position.y;
        playerInRange = CheckForPlayer(transform.position, 2, _player.GetComponent<Collider>());

        if(playerInRange && metronome.IsOnBeat() && CanAttack)
        {
            StartCoroutine(StartAttack(body._enemy.pattern));
        }
        if (aggrod)
        {
            if (body.me.enabled)
            {
                body.me.destination = _player.transform.position;
            }
            var distToTarget = Vector3.Distance(transform.position, body.me.pathEndPosition);
            if(distToTarget <= 1.0f)
            {
                if (body.me.enabled)
                {
                    //body.me.destination = _player.transform.position;
                }
            }
        }
        /*Death();*/

        //#region Slow Other Enemies:
        /* if (CheckForEnemies(transform.position, 5))
         {
             enemiesInRange.Remove(gameObject);
             foreach(GameObject enemy in enemiesInRange)
             {
                 enemy.GetComponent<Enemy>().speed = 1f;
             }
         }
         else
         {
             foreach(GameObject enemy in enemiesInRange)
             {
                 enemy.GetComponent<Enemy>().speed = 6f;
             }
             enemiesInRange.Clear();
         }*/
        //#endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            Debug.Log(health);
            health -= dashDamage;
        }
    }

    #region Define Enemy
    private void SetEnemyData(EnemySetting _enemy)
    {
        rb = gameObject.GetComponent<Rigidbody>();
        EnemyType _enemytype = _enemy.type;
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        _player = GameObject.Find("Player/PlayerObj");
        energy = GameObject.Find("Player").GetComponent<Energy>();
        switch (_enemytype)
        {
            case EnemyType.Standard:
                gameObject.name = "StandardEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Heavy:
                gameObject.name = "HeavyEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Ranged: 
                gameObject.name = "RangedEnemy";
                gameObject.tag = "Enemy";
                break;

        }
        health = _enemy.EnemyHealth;
        speed = _enemy.speed;
        TimeBetweenAttacks = _enemy.TimeBetweenAttacks;
        ChargeTime = _enemy.ChargeTime;
        enemyObj = gameObject.transform.GetChild(0);
    }

    #endregion

    #region Enemy Radius
    private bool CheckForPlayer(Vector3 center, float radius, Collider plr)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if(obj == plr)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForEnemies(Vector3 center, float radius)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach(var obj in collider)
        {
            if(obj.tag == "Enemy")
            {
                if(!enemiesInRange.Contains(obj.gameObject))
                {
                    enemiesInRange.Add(obj.gameObject);
                }
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2);
        Gizmos.DrawWireSphere(transform.position, maxdistance);
    }
    #endregion

    #region Attack Types
    private IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        animations.SetBool("InRange", false);
    }
    private IEnumerator Charge(float seconds)
    {

        float begginingspeed = speed;
        speed = 0f;
        ChargeParticle.Play();
        yield return new WaitForSeconds(seconds);
        ChargeParticle.Stop();
        Debug.Log("Charged!");
        speed = begginingspeed;
        body.EnablePathfinding();
        body.Recover();
    }
    private void LightAttack(int Damage)
    {
        Debug.Log("Used the light attack function");
        if(animations != null)
        {

        animations.SetBool("InRange", true);
        }
        _playerhealth.LoseHealth(Damage);
        StartCoroutine(Waiter(1f));
        body.EnablePathfinding();
        body.Recover();
    }
    private void HeavyAttack(int Damage)
    {
        Debug.Log("Used the heavy attack function");
        _playerhealth.LoseHealth(Damage);
        body.EnablePathfinding();
        body.Recover();
    }
    private void Load()
    {
        Debug.Log("Used the load function");
    }
    private void Shoot()
    {
        Debug.Log("Used the shoot function");
    }
    /*private void Lunge()
    {
        StartCoroutine(Lunge());
    }*/

    private IEnumerator StartAttack(Attack[] pattern)
    {
        CanAttack = false;
        body.DisablePathfinding();


        foreach (Attack attack in pattern)
        {
            if(playerInRange)
            {
                switch (attack)
                {
                    default:
                        break;
                    case Attack.Charge:
                        StartCoroutine(Charge(1f));
                        break;
                    case Attack.Light:
                        LightAttack(1);
                        break;
                    case Attack.Heavy:
                        HeavyAttack(3);
                        break;
                    case Attack.Load:
                        Load();
                        break;
                    case Attack.Shoot:
                        Shoot();
                        break;
                }
                yield return new WaitForSeconds(TimeBetweenAttacks);
            }
        }

        //body.me.enabled = true;
        CanAttack = true;
    }
    #endregion

    /*#region Death Conditions:
    void Death()
    {
        if (health <= 0)
        {
            Debug.Log("The enemy has died");
            if (energy.currentEnergy < 50)
            {
                energy.GainEnergy(_enemy.EnergyGainedOnBeat, _enemy.EnergyGainedOffBeat);
            }
            Destroy(gameObject);
        }
    }
    #endregion*/
}
