using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Utilize the data from the EnemySetting script to get all enemy data.
/// Will keep track and play through attack cycles.
/// </summary>

public class Enemy : MonoBehaviour
{
    #region Variables

    private Transform enemyObj;
    [SerializeField] private EnemySetting _enemy;
    Metronome metronome;
    List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Enemy Parameters")]
    Rigidbody rb;
    Vector3 targetPos;
    Vector3 lookatvector;
    int health;
    float speed;
    float TimeBetweenAttacks;
    float ChargeTime;
    ParticleSystem ChargeParticle;
    bool CanAttack = true;
    bool isGrounded;
    Animator animations;
    [Header("Target Parameters")]
    GameObject _player;
    Energy energy;
    Health _playerhealth;
    bool playerInRange;
    float maxdistance = 6f;

    [Header("Damage Numbers")]
    int dashDamage = 5;

    [Header("Wall Effects")]
    bool hasCollided;
    DecalPainter painter;

    #endregion

    #region Define Enemy

    private void SetEnemyData(EnemySetting _enemy)
    {
        rb = gameObject.GetComponent<Rigidbody>();
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        _player = GameObject.Find("Player/PlayerObj");
        energy = GameObject.Find("Player").GetComponent<Energy>();
        painter = GameObject.Find("DecalPainter").GetComponent<DecalPainter>();
        EnemyType _enemytype = _enemy.type;
        switch (_enemytype)
        {
            case EnemyType.Standard:
                gameObject.name = "StandardEnemy";
                gameObject.tag = "Enemy";
                animations = _enemy.Animations;
                break;
            case EnemyType.Heavy:
                gameObject.name = "HeavyEnemy";
                gameObject.tag = "Enemy";
                animations = _enemy.Animations;
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

    public void Start()
    {
        SetEnemyData(_enemy);
        if(_player != null)
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
            hasCollided = false;
            enemyObj.LookAt(lookatvector);
            float distanceToPlayer = targetPos.magnitude;
            if (distanceToPlayer > 2f)
            {
                float adjustedSpeed = Mathf.Lerp(0, speed, Mathf.Clamp01(distanceToPlayer / maxdistance));

                Vector3 move = targetPos.normalized * adjustedSpeed * Time.fixedDeltaTime;
                rb.MovePosition(transform.position + move);
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
            StartCoroutine(StartAttack(_enemy.pattern));
        }
        #region Ground Check + Falling rate
        if (Physics.Raycast(transform.position, Vector3.down, 2))
        {
            isGrounded = true;
        }
        if(!isGrounded)
        {
            rb.drag = 0;
        }
        else if(isGrounded)
        {
            rb.drag = 3;
        }
        #endregion
        #region Slow Other Enemies:
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
        #endregion
        Death();
    }

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
    }
    private void Lunge()
    {
        Debug.Log("Used the lunge function");
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
    }
    private void HeavyAttack(int Damage)
    {
        Debug.Log("Used the heavy attack function");
        _playerhealth.LoseHealth(Damage);
    }
    private void Load()
    {
        Debug.Log("Used the load function");
    }
    private void Shoot()
    {
        Debug.Log("Used the shoot function");
        GameObject bullet = Instantiate(gameObject, _player.transform.position, Quaternion.identity);
    }

    private IEnumerator StartAttack(Attack[] pattern)
    {
        CanAttack = false;
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
        CanAttack = true;
    }
    #endregion

    #region Damage Conditions:
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && !hasCollided)
        {
            hasCollided = true;
            Debug.Log(health);
            health -= dashDamage;
            ContactPoint contact = collision.contacts[0];
            Vector3 point = contact.point;
            point.y += 0.1f;
            Vector3 normal = contact.normal;
            StartCoroutine(painter.PaintDecal(point, normal, collision));
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "DeathPanel")
        {
            health -= health;
        }
    }

    #endregion
}